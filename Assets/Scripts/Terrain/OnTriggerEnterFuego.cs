using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterFuego : MonoBehaviour
{
    public bool guardar = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Se guarda");
            guardar = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            guardar = false;
        }
    }
}
