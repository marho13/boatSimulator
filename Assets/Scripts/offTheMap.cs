using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offTheMap : MonoBehaviour
{
    public bool istheobjectofftheMap(GameObject boat) 
    {
        Vector3 positions = boat.transform.position;
        if (lowestValue(positions.x)) 
        {
            return true;
        }
        if (highestValue(positions.x)) 
        {
            return true;
        }
        if (lowestValue(positions.y))
        {
            return true;
        }
        if (highestValue(positions.y))
        {
            return true;
        }
        return false;
    }

    public bool lowestValue(float coordinate) 
    {
        if (coordinate <= 0.0f) {
            return true;
        }
        return false;
    }

    public bool highestValue(float coordinate) 
    {
        if (coordinate >= 254.0f) 
        {
            return true;
        }
        return false;
    }
}
