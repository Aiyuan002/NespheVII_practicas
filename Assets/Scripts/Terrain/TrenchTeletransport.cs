using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenchTeletransport : MonoBehaviour
{
    //EL lugar hasta el que me teletransporto si toco el foso/Trench
    public Transform Target;

    //Solo se teletransporta el player
    public GameObject Player;

    //Fundido a negro cuando se teletransporta para que no sea tan directo
    public GameObject fade;

    //Tiempo para que se vuelva a desactivar el panel
    private float fadeDuration = 1.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.transform.position = Target.transform.position;
            fade.SetActive(true);
            StartCoroutine(DisableFade());
        }

        IEnumerator DisableFade()
        {
            yield return new WaitForSeconds(fadeDuration);
            fade.SetActive(false);
        }
    }
}
