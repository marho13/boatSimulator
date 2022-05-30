using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeofDocking : MonoBehaviour
{
    public string task;
    public Vector2 zero = new Vector2(1, 0);
    public Vector2 ninety = new Vector2(0, 1);
    public Vector2 oneeighty = new Vector2(-1, 0);
    public Vector2 twoseventy = new Vector2(0, -1);
    public float Zero;
    public float Ninety;
    public float Eighty;
    public float Seventy;
    public bool tooFarFromDock(Vector3 boat, Vector3 dock) //Is used together with the spawn boat script to create a scenario
    {
        //If the boat goes too far away from the docks, then reset it (return true)
        if (Vector3.Distance(boat, dock) > 25.0f) 
        {
            return true;
        }
        return false;
    }
    //Checks cardinal direction and that the boat is travelling towards the docks, if so return true
    //Might also want to check if the boat is just turning meaning accel is less than 25% (could place this code in the boat movement though)
    public bool travellingTowardsDock(Vector3 boat, Vector3 target, Vector3 forces)
    {   
        //First check the find the cardinal direction which is closest 
        float x = target.x - boat.x;
        float y = target.y - boat.y;
        
        //Calculate the softmax values to get the distances
        Vector2 softmaxed = calcSoftMax(x, y);
        Vector2 forcemaxed = calcSoftMax(forces.x, forces.y);

        //Find the closest cardinal direction
        int boaty = closestCardinal(softmaxed);
        int forcy = closestCardinal(forcemaxed);

        //If they have the same cardinal direction which is closest, they are moving towards each other
        if (boaty == forcy)
        {
            return true;
        }
        
        else
        {
            float angleDifference = angleCalculation(y, x) - angleCalculation(forces.y, forces.x);
            if ((boaty == 0 & forcy == 1) || (boaty==0 & forcy == 3))
            {
                if ((angleDifference > -45.0) & (angleDifference < 45.0))
                {
                    return true;
                }
            }

            else if ((boaty == 1 & forcy == 0)||(boaty == 1 & forcy == 2))
            {
                if ((angleDifference > -45.0) & (angleDifference < 45.0))
                {
                    return true;
                }
            }

            else if ((boaty == 2 & forcy == 1)||(boaty == 2 & forcy == 3))
            {
                if ((angleDifference > -45.0) & (angleDifference < 45.0))
                {
                    return true;
                }
            }
            else if ((boaty == 3 & forcy == 2) || (boaty == 3 & forcy == 0))
            {
                if ((angleDifference > -45.0) & (angleDifference < 45.0))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public float angleCalculation(float y, float x)
    {
        float hypo = Mathf.Sqrt((y * y) + (x * x));
        float cos = Mathf.Acos(y/hypo);
        return Mathf.Rad2Deg*cos;
    }

    public Vector2 calcSoftMax(float x, float y)
    {
        float sum = x + y;
        x = x / sum;
        y = y / sum;
        return new Vector2(x, y);
    }

    public int closestCardinal(Vector2 softmaxed)
    {
        Zero = Vector2.Distance(softmaxed, zero);
        Ninety = Vector2.Distance(softmaxed, ninety);
        Eighty = Vector2.Distance(softmaxed, oneeighty);
        Seventy = Vector2.Distance(softmaxed, twoseventy);

        if ((Zero < Ninety) & (Zero < Eighty) & (Zero < Seventy)) 
        {
            return 0;
        }
        if (Ninety < Eighty & Ninety < Seventy)
        {
            return 1;
        }
        if (Eighty < Seventy)
        {
            return 2;
        }
        return 3;
    }

    public Vector2 spawnBoat(GameObject boat, GameObject dock)
    {
        Debug.Log(boat.transform.position);
        Debug.Log(dock.name);
        //Take the angle of the dock, add 180 to it (it is rotated that way)
        float angle = dock.transform.eulerAngles.z;
        angle += 180.0f;

        //Generate a distance the boat is to be away from the boat
        float length = Random.Range(10.0f, 15.0f);

        //The min and max angles which the boat can spawn in
        float maxAngle = angle + 45.0f;
        float minAngle = angle - 45.0f;
        //Generate the angle which the boat should be spawned at
        float finiteAngle = Random.Range(minAngle, maxAngle);
        //Translate the angle to positions
        Vector2 positionXY = positions(finiteAngle);
        Debug.Log(positionXY);
        //Scale those positions with the length
        float x = positionXY.x * length;
        float y = positionXY.y * length;
        //Return the X and Y positions for the boat to spawn in
        Debug.Log(x.ToString() + " " +  y.ToString());
        boat.transform.position = new Vector3(x + dock.transform.position.x, y + dock.transform.position.y, 0.0f);

        //Changes the boats rotation, to point it towards the dock
        float hyp = Mathf.Sqrt(Mathf.Pow(x - dock.transform.position.x, 2) + Mathf.Pow(y - dock.transform.position.y, 2));
        float Eulerangle = Mathf.Rad2Deg * (0-Mathf.Sin((y - dock.transform.position.y) / hyp));
        boat.transform.eulerAngles = new Vector3(0.0f, 0.0f, Eulerangle);
        return boat.transform.position;
    }

    public Vector2 positions(float angle)
    {
        float radianAngle = Mathf.Deg2Rad * angle;

        float x = Mathf.Cos(radianAngle);
        float y = Mathf.Sin(radianAngle);
        return new Vector2(x, y);
    }

}
