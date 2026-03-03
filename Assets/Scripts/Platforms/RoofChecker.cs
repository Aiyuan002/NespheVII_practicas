using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofChecker : MonoBehaviour
{
    private CharacterController parent;

    private void Start()
    {
        parent = GetComponentInParent<CharacterController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Roof")
        {
            parent.isRoofTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Roof")
        {
            parent.isRoofTouching = false;
        }
    }
}
