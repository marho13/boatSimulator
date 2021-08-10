using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angleScript : MonoBehaviour
{
    public float angleBetweenObjects(Vector3 start, Vector3 stop)
    {
        float distance = Vector3.Distance(start, stop);
        float tanNum = Mathf.Atan(distance);
        return Mathf.Rad2Deg*tanNum;
    }
}
