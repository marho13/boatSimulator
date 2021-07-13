using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyPresses : MonoBehaviour
{
    public boatMovement bm;

    // Update is called once per frame
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
    }
}
