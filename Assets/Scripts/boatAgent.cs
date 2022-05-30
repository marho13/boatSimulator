using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class boatAgent : Agent
{
    public DistanceCalculations ds;
    public offTheMap otm;
    public TypeofDocking td;
    public Camera FrontFacingCamera;
    public boatMovement boaty;
    public lineWrite lw;
    public collisionSpeed cs;

    public List<float> state = new List<float>();
    public bool onRoad = true;
    public bool taskDone = false;
    public bool resetEnv = false;
    public bool taskComplete = false;
    public bool stucked = false;
    public int typeNumber = 0;
    public int straightCheckpoint = 0;
    public int nonStraightCheckpoint = 0;
    public float reward = 0.0f;
    public float prevRew = 0.0f;
    public Vector3 resetPosition = new Vector3(111.38f, 97.5f, 0.0f);
    // Start is called before the first frame update
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        td.spawnBoat(boaty.boaty, ds.getDockObject());
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        List<float> outputList = new List<float>();
        Vector2 distance = setArrows();

        float surge = Random.Range(-0.5f, 0.5f);
        float sway = Random.Range(-0.5f, 0.5f);
        //Distance to object
        outputList.Add(distance.x);
        outputList.Add(distance.y);
        //
        outputList.Add(boaty.boat.transform.eulerAngles.z);
        outputList.Add(boaty.boat.angularVelocity.z);
        outputList.Add(surge);
        outputList.Add(sway);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        reward = 0.0f;
        taskComplete = ds.taskComplete(boaty.boaty.transform.position);

        // Actions, size = 2
        float steering = actionBuffers.ContinuousActions[0];
        float acceleration = actionBuffers.ContinuousActions[1];
        float bucket = actionBuffers.ContinuousActions[2];

        Vector2 distance = setArrows();

        state = boaty.printInfo(distance);
        Vector3 output = steeringTranslation(steering, acceleration, bucket);

        rBody.AddForce(output * forceMultiplier);

        reward += cs.getReward();
        cs.resetRewards();
       

        taskOne(output, steering, acceleration); //Checking if the boat is moving towards the goal or not
        taskTwo(); //Check if the boat is off the road or not


        taskZero(taskComplete);
        if (taskComplete)
        {
            taskCompletion(); //Completed the task, and so get 1000 reward
            resetEnv = true;
            Vector2 newPos = td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
            boaty.boat.velocity = Vector3.zero;
            boaty.boat.angularVelocity = Vector3.zero;
            EndEpisode();
        }

        stillReward(); //General still reward to stop the boat from performing no actions

        if (resetEnv)
        {
            episodeReward();
            SetReward(reward);
            Vector2 newPos = td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
            boaty.boat.velocity = Vector3.zero;
            boaty.boat.angularVelocity = Vector3.zero;
            EndEpisode();
        }

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            SetReward(reward);
            Vector2 newPos = td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
            boaty.boat.velocity = Vector3.zero;
            boaty.boat.angularVelocity = Vector3.zero;
            EndEpisode();
        }

    }

    Vector3 steeringTranslation(float steering, float acceleration, float bucket)
    {
        if (bucket < 0.5)
        {
            bucket = 1.0f;
        }
        else
        {
            bucket = -1.0f;
        }
        Vector3 forward = boaty.rotationToOffset(bucket * acceleration, "straight");
        Vector3 side = boaty.rotationToOffset(steering, "sideways");
        Vector3 output = forward + side;

        return output;
    }

    void resetCheckpoints()
    {
        resetEnv = false;//Need to do anything?
    }


    //These next functions are the reward function
    void taskCompletion()
    {
        reward += 1000.0f;
    }
    void stillReward()
    {
        reward -= 0.001f;
    }

    void offRoad()
    {
        reward -= 0.01f;
    }

    void episodeReward()
    {
        reward -= 1.0f;
    }

    //Tasks 0-2 using array notation
    void taskZero(bool taskComplete)
    {
        stucked = boaty.stuck(boaty.boaty.transform.position);
        Debug.Log(taskComplete);
        if (taskComplete == true)
        {
            Vector2 newPos = td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
            boaty.boat.velocity = Vector3.zero;
            boaty.boat.angularVelocity = Vector3.zero;

            resetEnv = true;
            taskComplete = false;
        }
        else if (typeNumber == 0 && !resetEnv)
        {
            if (td.tooFarFromDock(boaty.boaty.transform.position, ds.getCurrentDock()))
            {
                taskZero(true);
            }
        }

        if (stucked)
        {
            Vector2 newPos = td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
            boaty.boat.velocity = Vector3.zero;
            boaty.boat.angularVelocity = Vector3.zero;

            resetEnv = true;
            taskComplete = false;
        }
    }

    void taskOne(Vector3 output, float steering, float acceleration)
    {
        if (typeNumber == 1)
        {
            bool towards = td.travellingTowardsDock(boaty.boaty.transform.position, ds.getCurrentDock(), output);
            if (towards)
            {
                boaty.step(output);
            }
            else
            {
                if ((Mathf.Abs(steering) > 0.5) & (acceleration < 0.25f))
                {
                    boaty.step(output);
                }
            }
        }

        else
        {
            boaty.step(output);
        }
    }

    void taskTwo()
    {
        if (boaty.boatAshore())
        {
            offRoad();
        }
        float dist = ds.calcDistances(boaty.boat.transform.position);
        reward += (dist - prevRew);
        prevRew = dist;
        if (otm.istheobjectofftheMap(boaty.boaty))
        {
            boaty.boaty.transform.position = resetPosition;
        }
        //Whatever we wish to do on task 2
    }

    Vector2 setArrows()
    {
        lw.changePointOne((boaty.boaty.transform.position + new Vector3(0.0f, 0.0f, -0.1f)));

        Vector3 currDock = ds.getCurrentDock();
        Vector2 pos1 = new Vector2(boaty.boaty.transform.position.x - currDock.x, boaty.boaty.transform.position.y - currDock.y);
        Vector2 softMax = calcSoftMax(pos1.x, pos1.y);
        Vector3 pos2 = new Vector3(boaty.boaty.transform.position.x - softMax.x, boaty.boaty.transform.position.y - softMax.y, boaty.boaty.transform.position.z - 0.1f);
        lw.changePointTwo(pos2);
        return pos1;
    }

    public Vector2 calcSoftMax(float x, float y)
    {
        float sum = Mathf.Abs(x) + Mathf.Abs(y);
        x = x / sum;
        y = y / sum;
        return new Vector2(x, y);
    }
}
