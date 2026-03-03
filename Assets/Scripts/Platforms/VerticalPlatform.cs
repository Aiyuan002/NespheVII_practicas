using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    public bool auto = false;

    public float speed;

    public float minLimit;
    public float maxLimit;

    private bool movePlatform = false;

    private int vel = 1;

    private void Update()
    {
        if (auto || movePlatform)
        {
            if (transform.position.y >= maxLimit) { vel = -1; }
            if (transform.position.y <= minLimit) { vel = 1; }

            transform.Translate(new Vector2(0, vel) * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInChildren<GroundChecker>() && !auto)
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
