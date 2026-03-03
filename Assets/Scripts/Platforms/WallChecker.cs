using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChecker : MonoBehaviour
{
    private CharacterController parent;

    private void Start()
    {
        parent = GetComponentInParent<CharacterController>();
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
