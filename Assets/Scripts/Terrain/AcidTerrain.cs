using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidTerrain : MonoBehaviour
{
    public GameObject Player;

    private bool isInAcid = false; // Variable para verificar si el jugador está en el ácido
    private float timer = 0.0f; // Temporizador inicializado en cero

    private void Update()
    {
        if (isInAcid)
        {
            timer += Time.deltaTime;

            if (timer >= 1.0f)
            {
                timer = 0.0f;
                PlayerController pc = Player.GetComponent<PlayerController>();
                if (pc != null && !pc.isImmune && !pc.isDying)
                    pc.GetDamage();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // El jugador está en el ácido
            isInAcid = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInAcid = false; // El jugador ya no está en el ácido
            timer = 0.0f; // Reiniciar el temporizador al salir del ácido
        }
    }
}
