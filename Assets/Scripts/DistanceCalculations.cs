using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCalculations : MonoBehaviour
{
    public GameObject checkpointList; ///The gameobject will all of the docks
    public int currentCheck = 0;
    public List<GameObject> checkpoints;
    public void Start()
    {
        checkpoints = getCheckpointList();
    }
    public float calcDistances(Vector3 start) 
    {
        return Vector3.Distance(start, checkpoints[currentCheck].transform.position);
    }

    public Vector3 getCurrentDock()
    {
        return checkpoints[currentCheck].transform.position;
    }

    public bool taskComplete(Vector3 boat) 
    {
        if (Vector3.Distance(boat, checkpoints[currentCheck].transform.position) < 1.0f)
        {
            if (currentCheck == checkpoints.Count) 
            {
                currentCheck = 0;
                return true;
            }

            else
            {
                currentCheck += 1;
                return true;
            }
        }
        else 
        {
            return false;
        }
    }

    public List<GameObject> getCheckpointList() 
    {
        return checkpointList.GetAllChilds();
    }
}
public static class ClassExtension
{
    public static List<GameObject> GetAllChilds(this GameObject Go)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            list.Add(Go.transform.GetChild(i).gameObject);
        }
        return list;
    }
}
