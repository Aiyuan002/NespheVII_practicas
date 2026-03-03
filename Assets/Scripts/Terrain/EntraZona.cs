using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntraZona : MonoBehaviour
{
    public bool enZona = false;
    public Transform player;
    public Transform playerStopDistance;
    // Start is called before the first frame update
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enZona = true;
            player.transform.position = collision.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStopDistance.transform.position = other.transform.position;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enZona = false;
        }
    }
}
