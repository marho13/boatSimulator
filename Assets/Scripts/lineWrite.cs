using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineWrite : MonoBehaviour
{
    public LineRenderer lR;
    public void changePointOne(Vector3 newposition) 
    {
        lR.SetPosition(0, newposition);
    }
    public void changePointTwo(Vector3 newposition)
    {
        lR.SetPosition(1, newposition);
    }
}
