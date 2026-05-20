using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Piolet : MonoBehaviour
{
    public static bool piolet;

    private void OnEnable()
    {
        piolet = false;
    } 

    public void PioletColeccionado()
    {
        piolet = true;
    }
}
