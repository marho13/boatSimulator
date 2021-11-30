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
    public lineWrite lw;
    public collisionSpeed cs;
    [SerializeField] Vector2 softMax;
    [SerializeField] Vector3 pos2;
    // Update is called once per frame
    private void Awake()
    {
        Physics.gravity = new Vector3(0.0f, 0.0f, 9.8f);
    }
    void FixedUpdate()
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
        Vector2 distance = setArrows();
        //bm.printInfo(distance);
        //ds.taskComplete(bm.boaty.transform.position);
    }

    Vector2 setArrows()
    {
        lw.changePointOne(bm.boaty.transform.position + new Vector3(0.0f, 0.0f, -0.1f));

        Vector3 currDock = ds.getCurrentDock();
        Vector2 pos1 = new Vector2(bm.boaty.transform.position.x - currDock.x, bm.boaty.transform.position.y - currDock.y);
        softMax = calcSoftMax(pos1.x, pos1.y);
        pos2 = new Vector3(bm.boaty.transform.position.x - softMax.x, bm.boaty.transform.position.y - softMax.y, bm.boaty.transform.position.z - 0.1f);
        lw.changePointTwo(pos2);
        return pos1;
    }

    public Vector2 calcSoftMax(float x, float y)
    {
        float sum = Mathf.Abs(x) + Mathf.Abs(y);
        x = x / sum;
        y = y / sum;
        return new Vector2(x, y);
    }
}
