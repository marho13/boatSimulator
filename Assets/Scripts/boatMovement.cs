using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatMovement : MonoBehaviour
{
    public Rigidbody boat;
    public Vector3 rightForce;
    public Vector3 leftForce;
    public Vector3 upForce;
    public Vector3 downForce;
    public Vector3 offSet;

    public float radius = -0.5f;
    public float maxForwardSpeed;
    public float maxSideSpeed;
    private void Awake()
    {
        rightForce = new Vector3(-2.0f, 0.0f, 0.0f);
        leftForce = new Vector3(2.0f, 0.0f, 0.0f);
        upForce = new Vector3(0.0f, 0.0f, 10.0f);
        downForce = new Vector3(0.0f, 0.0f, -10.0f);
        offSet = new Vector3(0.0f, 0.0f, 0.0f);
        boat.constraints = RigidbodyConstraints.FreezeRotationX;
    }

    public void moveRight()
    {
        //offSet = rotationToOffset();
        Vector3 right = boat.transform.position - offSet;
        boat.AddForceAtPosition(rightForce, right);
    }

    public void moveLeft()
    {
        //offSet = rotationToOffset();
        Vector3 left = boat.transform.position - offSet;
        boat.AddForceAtPosition(leftForce, left);
    }

    public void moveForward()
    {
        //offSet = rotationToOffset();
        Vector3 forward = boat.transform.position - offSet;
        boat.AddForceAtPosition(upForce, forward);
    }

    public void moveDown()
    {
        //offSet = rotationToOffset();
        Vector3 down = boat.transform.position - offSet;
        boat.AddForceAtPosition(downForce, down);
    }

    public void step(float steering, float acceleration, float bucket) {
        if (bucket < 0.5) {
            bucket = 1.0f;
        }
        else {
            bucket = -1.0f;
        }
        Vector3 forcy = new Vector3(steering * maxSideSpeed, 0.0f, acceleration * maxForwardSpeed * bucket);
        //offSet = rotationToOffset();
        boat.AddForceAtPosition(forcy, boat.transform.position - offSet);
    }

    public Vector3 rotationToOffset() 
    {
        float angle = (float)(boat.transform.rotation.y*Mathf.PI/180.0);
        float x = (float)(Mathf.Cos(angle) * radius);
        float y = (float)(Mathf.Sin(angle) * radius);

        Debug.Log((x + y) == radius);
        
        return new Vector3(y, 0.0f, x);
    }
}
