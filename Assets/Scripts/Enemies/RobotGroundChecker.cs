using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGroundChecker : MonoBehaviour
{
    public bool isGrounded;
    public string test;
    private int groundContacts = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        test = collision.transform.tag;
        if (collision.CompareTag("Ground"))
        {
            groundContacts++;
            isGrounded = true; // Estás en el suelo si hay al menos un "Ground"
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundContacts--;
            if (groundContacts <= 0)
            {
                isGrounded = false;
                groundContacts = 0; // Asegura que no quede en negativo
            }
        }
    }
}
