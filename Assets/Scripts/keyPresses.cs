using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyPresses : MonoBehaviour
{
    public boatMovement bm;
    public angleScript aS;
    public DistanceCalculations ds;
    public offTheMap otm;
    public TypeofDocking td;
    // Update is called once per frame
    private void Awake()
    {
        Physics.gravity = new Vector3(0.0f, 0.0f, 9.8f);
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.W)) {
            bm.moveForward();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            bm.moveLeft();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            bm.moveDown();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            bm.moveRight();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            bm.moveDirectlyLeft();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            bm.moveDirectlyRight();
        }

        ds.taskComplete(bm.boaty.transform.position);
        //float angle = aS.angleBetweenObjects(bm.boat.transform.position, ds.getCurrentDock());
        //Debug.Log(angle.ToString());
        if (otm.istheobjectofftheMap(bm.boaty)) 
        {
            Debug.Log("Reset me plese");
        }
        bool onLand = bm.boatAshore(); //Can use the onland to reset it you want
        bool towards = td.travellingTowardsDock(bm.boaty.transform.position, ds.getCurrentDock(), bm.lastAction);
        Debug.Log(towards);
    }
}
