using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionSpeed : MonoBehaviour
{  
    float reward = 0.0f;
    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "docks")
        {
            float x = collision.relativeVelocity.x;
            float y = collision.relativeVelocity.x;
            if ((x + y) > 2.0f)
            {
                setReward(x * 10.0f, y * 10.0f);
            }
        }
        if (collision.gameObject.tag == "land")
        {
            float x = collision.relativeVelocity.x;
            float y = collision.relativeVelocity.x;
            if ((x + y) > 2.0f)
            {
                setReward(x * 10.0f, y * 10.0f);
            }
        }
    }

    public float getReward()
    {
        return reward;
    }

    void setReward(float x, float y)
    {
        reward = x + y;
    }
}
