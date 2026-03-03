using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotClimbChecker : MonoBehaviour
{
    public bool canClimb;
    public bool climb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Climb")
        {
            canClimb = true;
            climb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Climb")
        {
            climb = false;
        }
    }
}
