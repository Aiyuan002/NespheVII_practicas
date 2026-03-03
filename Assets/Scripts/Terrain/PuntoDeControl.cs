using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntoDeControl : MonoBehaviour
{
    public Transform player; // Referencia al objeto del jugador

    public void GuardarPartida()
    {
        // Guardar la posición del jugador en las preferencias del jugador
        PlayerPrefs.SetFloat("PlayerPosX", player.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.position.z);
        PlayerPrefs.Save(); // Guardar los cambios
    }
}