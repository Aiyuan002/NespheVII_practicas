using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntraTrigger : MonoBehaviour
{
    public bool entra;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            entra = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            entra = false;
        }
    }
}
