using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using System;
using System.Security.AccessControl;
using System.Threading;
public class commandCentre : MonoBehaviour
{
    public DistanceCalculations ds;
    public offTheMap otm;
    public TypeofDocking td;
    public Camera FrontFacingCamera;
    public boatMovement boaty;
    public lineWrite lw;
    private SocketIOComponent _socket;

    public List<float> state = new List<float>();
    public bool onRoad = true;
    public bool taskDone = false;
    public bool resetEnv = false;
    public bool taskComplete = false;
    public int typeNumber = 0;
    public int straightCheckpoint = 0;
    public int nonStraightCheckpoint = 0;
    public float reward = 0.0f;
    public float prevRew = 0.0f;
    public Vector3 resetPosition = new Vector3(111.38f, 97.5f, 0.0f);

    // Use this for initialization
    void Start()
    {
        _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        _socket.On("open", OnOpen);
        _socket.On("steer", OnSteer);
        //_socket.On("manual", onManual);
        _socket.On("ready", OnReady);
        taskZero(true);
        prevRew = ds.calcDistances(boaty.boat.transform.position);
    }

    //These first functions (On*), perform tasks when called such as connect unity and our actor, steer and more
    void OnOpen(SocketIOEvent obj)
    {
        Debug.Log("Connection Open");
        EmitTelemetry(obj);
    }

    void OnSteer(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
        reward = 0.0f;
        taskComplete = ds.taskComplete(boaty.boaty.transform.position);

        float steering = float.Parse(jsonObject.GetField("steering_angle").str);
        float acceleration = float.Parse(jsonObject.GetField("acceleration").str);
        float bucket = float.Parse(jsonObject.GetField("bucket").str);

        Vector2 distance = setArrows();

        state = boaty.printInfo(distance);

        Vector3 output = steeringTranslation(steering, acceleration, bucket);

        taskOne(output, steering, acceleration); //Checking if the boat is moving towards the goal or not
        taskTwo(); //Check if the boat is off the road or not

        Thread.Sleep(17);//Sleep for 0.1 seconds so that the reward becomes good from the distance calculated
        taskZero(taskComplete);
        if (taskComplete) 
        {
            Debug.Log("Task completed");
            taskCompletion(); //Completed the task, and so get 1000 reward
            resetEnv = true;
        }

        stillReward(); //General still reward to stop the boat from performing no actions

        if (resetEnv)
        {
            episodeReward();
        }

        EmitTelemetry(obj);
    }

    //Translates the action for the user
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

    void EmitTelemetry(SocketIOEvent obj) //Sends the information back to the agent
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.Log("Attempting to Send...");
            // send only if it's not being manually driven
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera));
            //data["state"] = string.Join(",", state);

    
            data["reward"] = reward.ToString();
            data["checkpointStraight"] = straightCheckpoint.ToString();
            data["checkpointNonStraight"] = nonStraightCheckpoint.ToString();
            data["onRoad"] = onRoad.ToString();
            data["resetEnv"] = resetEnv.ToString();
            Debug.Log("Telemetry");
            _socket.Emit("telemetry", new JSONObject(data));

            if (resetEnv)
            {
                resetEnv = false;
                Debug.Log("reset");
                reward = 0.0f;
                resetEnv = false;
            }
            reward = 0.0f;
        });
    }
    

    void OnReady(SocketIOEvent obj)
    {
        JSONObject jsonObject = obj.data;
        resetEnv = false;
        resetCheckpoints();
        Debug.Log("Speeding up");
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
        if (typeNumber == 0 && taskComplete==true)
        {
            td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
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
    }

    void taskOne(Vector3 output, float steering, float acceleration)
    {
        if (typeNumber == 1)
        {
            Debug.Log("Task 1");
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
            Debug.Log("Stepping");
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
