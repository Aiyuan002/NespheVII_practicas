using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;

public class ActivarIntermedio : MonoBehaviour
{
    public bool activar = false;

    public GameObject objectoAnimacion;
    public GameObject mainCamera;
    public GameObject transition;
    Animator transi;
    public Transform puntoDeSpawn;

    public GameObject minimapa;

    void Start()
    {
        transi = transition.GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
    if (other.CompareTag("Player"))
    {
        StartCoroutine(WaitCamera());
    }
    }

    IEnumerator WaitCamera()
    {
        transi.Play("TransicionEntreCamaras");
        yield return new WaitForSeconds(0.5f);

        activar = true;
        objectoAnimacion.SetActive(true);
        mainCamera.SetActive(false);

        minimapa.SetActive(false);

        // Esperar a que termine la animación de la cámara del boss
        yield return new WaitForSeconds(5f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = puntoDeSpawn.position;
        mainCamera.SetActive(true);
        objectoAnimacion.SetActive(false);

        minimapa.SetActive(true);

        Destroy(gameObject);
    }
}
