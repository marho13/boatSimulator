using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatAshore : MonoBehaviour
{
    public GameObject boat;
    public int layer_mask;
    private void Awake()
    {
        layer_mask = LayerMask.GetMask("land");
    }
    public bool boatOnLand(Vector3 startPos, float distance, Vector3 direction)
        {
            //varible to hold the detection info
            RaycastHit hit;
            //the end Pos which defaults to the startPos + distance 
            Vector3 endPos = startPos + (distance * direction);
            if (Physics.Raycast(startPos, direction, out hit, distance, layer_mask))
            {
                return true;
            }
        // 2 is the duration the line is drawn, afterwards its deleted
        return false;
        }
}
