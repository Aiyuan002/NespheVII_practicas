using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.5f; //Para modificar la duración de la transicion.

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeOut(sceneIndex));
    }

    IEnumerator FadeIn()
    {
        float t = fadeDuration;
        Color c = fadeImage.color;

        while (t > 0)
        {
            t -= Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
    }

    IEnumerator FadeOut(int sceneIndex)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
