using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatMovement : MonoBehaviour
{
    public GameObject boaty;
    public Rigidbody boat;
    public boatAshore ba;
    [SerializeField] GameObject backBoat;
    [SerializeField] GameObject backLeft;
    [SerializeField] GameObject backRight;
    [SerializeField] GameObject frontBoat;
    [SerializeField] GameObject frontLeft;
    [SerializeField] GameObject frontRight;


    public bool boatAshore() 
    {
        bool backy = (ba.boatOnLand(backBoat.transform.position, 0.05f, new Vector3(0.0f, 0.0f, 1.0f))) ;
        bool backleft = (ba.boatOnLand(backLeft.transform.position, 0.05f, new Vector3(0.0f, 0.0f, 1.0f))) ;
        bool backright = (ba.boatOnLand(backRight.transform.position, 0.05f, new Vector3(0.0f, 0.0f, 1.0f))) ;
        bool fronty = (ba.boatOnLand(frontBoat.transform.position, 0.05f, new Vector3(0.0f, 0.0f, 1.0f))) ;
        bool frontleft = (ba.boatOnLand(frontLeft.transform.position, 0.05f, new Vector3(0.0f, 0.0f, 1.0f))) ;
        bool frontright = (ba.boatOnLand(frontRight.transform.position, 0.05f, new Vector3(0.0f, 0.0f, 1.0f))) ;
        if (backy || backleft || backright || fronty || frontleft || frontright)
        {
            Debug.Log("Found land");
            return true;

        }
        return false;
    }
    public void moveRight()
    {
        Vector3 offSet = rotationToOffset(-1.0f, "sideways");
        boat.AddForceAtPosition(offSet, backBoat.transform.position);
    }

    public void moveLeft()
    {
        Vector3 offSet = rotationToOffset(1.0f, "sideways");
        boat.AddForceAtPosition(offSet, backBoat.transform.position);
    }

    public void moveForward()
    {
        Vector3 offSet = rotationToOffset(1.0f, "straight");
        boat.AddForceAtPosition(offSet, backBoat.transform.position);
    }

    public void moveDown()
    {
        Vector3 offSet = rotationToOffset(-1.0f, "straight");
        boat.AddForceAtPosition(offSet, backBoat.transform.position);
    }

    public void step(float steering, float acceleration, float bucket) {
        if (bucket < 0.5) {
            bucket = 1.0f;
        }
        else {
            bucket = -1.0f;
        }
        Vector3 forward = rotationToOffset(bucket*acceleration, "straight");
        Vector3 side = rotationToOffset(steering, "sideways");
        Vector3 output = forward + side;
        boat.AddForceAtPosition(output, backBoat.transform.position);
    }

    public Vector3 rotationToOffset(float multiplier, string straight) 
    {
        float angle = Mathf.Deg2Rad*backBoat.transform.eulerAngles.z;
        if (straight == "straight")
        {
            float x = (float)(Mathf.Cos(angle) * 10.0f) * multiplier;
            float y = (float)(Mathf.Sin(angle) * 10.0f) * multiplier;

            return new Vector3(-y, x, 0);
        }

        else 
        {
            float x = (float)(Mathf.Cos(angle) * 0.50f) * multiplier;
            float y = (float)(Mathf.Sin(angle) * 0.50f) * multiplier;

            return new Vector3(x, y, 0);
        }
    }
}
