using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChecker : MonoBehaviour
{
    private CharacterController parent;
    private Rigidbody2D rb;

    [SerializeField] private float rayLength = 0.15f;
    [SerializeField] private LayerMask wallMask;

    private void Start()
    {
        parent = GetComponentInParent<CharacterController>();
        rb = parent.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckWallsForRun();
    }

    private void CheckWallsForRun()
    {
        bool wallRight = Physics2D.Raycast(transform.position, Vector2.right, rayLength, wallMask);
        bool wallLeft  = Physics2D.Raycast(transform.position, Vector2.left,  rayLength, wallMask);

        Debug.DrawLine(transform.position, transform.position + Vector3.right * rayLength, wallRight ? Color.red : Color.green);
        Debug.DrawLine(transform.position, transform.position + Vector3.left  * rayLength, wallLeft  ? Color.red : Color.green);

        parent.wallOnRight = wallRight;
        parent.wallOnLeft  = wallLeft;

        float vx = rb.linearVelocity.x;
        bool movingIntoWall = (vx > 0.01f && wallRight) || (vx < -0.01f && wallLeft);
        parent.canRun = !movingIntoWall;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Climb")
        {
            parent.isWallTouching = true;
        }
        if (collision.transform.tag == "Walls")
        {
            parent.stopWalls = true;
        }
        if (collision.transform.tag == "Vine")
        {
            parent.isVineTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Climb")
        {
            parent.isWallTouching = false;
        }
        if (collision.transform.tag == "Walls")
        {
            parent.stopWalls = false;
        }

        if (collision.transform.tag == "Vine")
        {
            parent.isVineTouching = false;
        }
    }
}
