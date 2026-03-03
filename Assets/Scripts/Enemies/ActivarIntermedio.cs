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
        //transition.SetActive(false);
        Destroy(gameObject);
    }
}
