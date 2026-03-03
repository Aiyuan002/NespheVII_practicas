using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Piolet : MonoBehaviour
{
    public CharacterController scriptPlayer;
    public static bool piolet;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        piolet = false;
    } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PioletColeccionado();
            Destroy(gameObject);
        }
    }

    public void PioletColeccionado()
    {
        piolet = true;
        scriptPlayer.Climb();
    }

    
}
