using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWallChecker : MonoBehaviour
{
    public bool isWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isWall = false;
        }
    }
}
