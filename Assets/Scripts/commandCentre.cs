using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SocketIO;
using System;
using System.Threading;
using System.Security.AccessControl;
public class commandCentre : MonoBehaviour
{
    public DistanceCalculations ds;
    public offTheMap otm;
    public TypeofDocking td;
    public Camera FrontFacingCamera;
    public boatMovement boaty;
    private SocketIOComponent _socket;
    public bool onRoad = true;
    public bool taskDone = false;
    public bool resetEnv = false;
    public bool taskComplete = false;
    public int typeNumber = 0;
    public int straightCheckpoint = 0;
    public int nonStraightCheckpoint = 0;
    public float reward = 0.0f;
    public Vector3 resetPosition = new Vector3(111.38f, 97.5f, 0.0f);

    // Use this for initialization
    void Start()
    {
        _socket = GameObject.Find("SocketIO").GetComponent<SocketIOComponent>();
        _socket.On("open", OnOpen);
        _socket.On("steer", OnSteer);
        //_socket.On("manual", onManual);
        _socket.On("ready", OnReady);
        taskZero();
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
        Debug.Log(jsonObject);
        taskComplete = ds.taskComplete(boaty.boaty.transform.position);

        float steering = float.Parse(jsonObject.GetField("steering_angle").str);
        float acceleration = float.Parse(jsonObject.GetField("throttle").str);
        float bucket = float.Parse(jsonObject.GetField("bucket").str);

        Vector3 output = steeringTranslation(steering, acceleration, bucket);

        taskOne(output, steering, acceleration); //Checking if the boat is moving towards the goal or not
        taskTwo(); //Check if the boat is off the road or not

        Thread.Sleep(100);//Sleep for 0.1 seconds so that the reward becomes good from the distance calculated

        if (taskComplete) 
        {
            taskZero(); //If the boat has reached the docks reset it
            taskCompletion(); //Completed the task, and so get 1000 reward
            resetEnv = true;
        }
        stillReward(); //General still reward to stop the boat from performing no actions
        EmitTelemetry(obj);
        if (resetEnv)
        {
            episodeReward();
        }
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
            print("Attempting to Send...");
            // send only if it's not being manually driven
            if ((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.S)))
            {
                _socket.Emit("telemetry", new JSONObject());
            }
            else
            {
                // Collect Data from the Car
                Dictionary<string, string> data = new Dictionary<string, string>();
                //data["steering_angle"] = _carController.CurrentSteerAngle.ToString("N4");
                //data["throttle"] = _carController.AccelInput.ToString("N4");
                //data["speed"] = _carController.CurrentSpeed.ToString("N4");
                //data["image"] = Convert.ToBase64String(CameraHelper.CaptureFrame(FrontFacingCamera));
                data["reward"] = reward.ToString();
                data["checkpointStraight"] = straightCheckpoint.ToString();
                data["checkpointNonStraight"] = nonStraightCheckpoint.ToString();
                data["onRoad"] = onRoad.ToString();
                data["resetEnv"] = resetEnv.ToString();
                _socket.Emit("telemetry", new JSONObject(data));

                if (resetEnv)
                {
                    Debug.Log("reset");
                    resetCheckpoints();
                    reward = 0.0f;
                    resetEnv = false;
                }
                reward = 0.0f;
            }
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
        //Need to do anything?
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
    void taskZero()
    {
        if (typeNumber == 0)
        {
            td.spawnBoat(boaty.boaty, ds.getDockObject());
            resetPosition = boaty.boaty.transform.position;
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
        reward += ds.calcDistances(boaty.boat.transform.position);
        if (otm.istheobjectofftheMap(boaty.boaty))
        {
            boaty.boaty.transform.position = resetPosition;
        }
        //Whatever we wish to do on task 2
    }
}
