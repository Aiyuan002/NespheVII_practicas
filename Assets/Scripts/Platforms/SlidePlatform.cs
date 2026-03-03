using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlatform : MonoBehaviour
{
    public bool auto = false;

    public float speed;

    public float minLimit;
    public float maxLimit;

    private bool movePlatform = false;

    private int vel = 1;

    private void Update()
    {
        if(auto || movePlatform)
        {
            if(transform.position.x >= maxLimit) { vel = -1; }
            if(transform.position.x <= minLimit) { vel = 1; }

            transform.Translate(new Vector2(vel, 0) * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.GetChild(transform.childCount - 1)).tag == "GroundCheck" && !auto)
        {
            movePlatform = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (movePlatform)
        {
            movePlatform = false;
        }
    }
}
