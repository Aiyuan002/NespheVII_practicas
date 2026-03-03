using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotRoofChecker : MonoBehaviour
{
    public bool isRoof = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isRoof = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isRoof = false;
        }
    }
}
