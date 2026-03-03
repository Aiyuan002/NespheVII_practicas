using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator pantallaNegraAnimator;
    public string nombreAnimacionFadeOut = "FadeOut";
    public string nombreEscena = "Zone1 2.0";

    private bool yaTransicionando = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !yaTransicionando)
        {
            yaTransicionando = true;
            StartCoroutine(TransicionConFade());
        }
    }

    private System.Collections.IEnumerator TransicionConFade()
    {
        pantallaNegraAnimator.Play(nombreAnimacionFadeOut);

        // Guardamos que se ha hecho una transición
        PlayerPrefs.SetInt("TransicionActiva", 1);

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nombreEscena);
    }
}
