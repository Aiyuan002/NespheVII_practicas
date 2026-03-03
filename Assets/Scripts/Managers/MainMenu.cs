using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject[] menuItems;

    [SerializeField]
    public float retrasoEntreElementos = 0.25f; // Retraso entre la aparición de los elementos

    [SerializeField]
    public float fadeDuration = 0.25f; // Duración de la animación fade-in

    [SerializeField]
    public Image image1;

    [SerializeField]
    public Image image2;

    [SerializeField]
    public float tiempoImagen = 6f; // Tiempo que cada imagen permanece visible antes de hacer la transición

    [SerializeField]
    public float fadeDurationImagen = 1f; // Duración de la animación fade-in y fade-out de la imagen

    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip menuMusic;

    void Awake()
    {
        clickSFX.LoadAudioData();
    }

    public void Start()
    {
        AudioManager.Instance.PlayMusic(menuMusic);
        foreach (GameObject item in menuItems) //Para desactivar todos los elementos
        {
            CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = item.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f; //Cambia la opacidad a 0
        }

        StartCoroutine(ShowMenuItems());
        StartCoroutine(AlternarImagenes());
    }
    public void PlayClick()
    {
        AudioManager.Instance.PlaySFX(clickSFX);
    }

    public IEnumerator ShowMenuItems()
    {
        foreach (GameObject item in menuItems)
        {
            CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

            item.SetActive(true); //Activa el elemento

            float timeElapsed = 0f;
            while (timeElapsed < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f; //Se asegura de que la opacidad sea completa.

            yield return new WaitForSeconds(retrasoEntreElementos);
        }
    }

    public IEnumerator AlternarImagenes()
    {
        // Asegurarse de que ambas imágenes están activas desde el principio
        image1.gameObject.SetActive(true);
        image2.gameObject.SetActive(true);

        // Asegurarse de que ambas tienen CanvasGroup
        CanvasGroup cg1 = image1.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = image1.gameObject.AddComponent<CanvasGroup>();
        CanvasGroup cg2 = image2.GetComponent<CanvasGroup>();
        if (cg2 == null) cg2 = image2.gameObject.AddComponent<CanvasGroup>();

        // Empezar con image1 visible, image2 invisible
        cg1.alpha = 1f;
        cg2.alpha = 0f;

        while (true)
        {
            yield return Crossfade(cg1, cg2);
            yield return new WaitForSeconds(tiempoImagen);
            yield return Crossfade(cg2, cg1);
            yield return new WaitForSeconds(tiempoImagen);
        }
    }

    public IEnumerator Crossfade(CanvasGroup from, CanvasGroup to)
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDurationImagen)
        {
            float t = timeElapsed / fadeDurationImagen;
            from.alpha = Mathf.Lerp(1f, 0f, t);
            to.alpha = Mathf.Lerp(0f, 1f, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        from.alpha = 0f;
        to.alpha = 1f;
    }

    public IEnumerator FadeInImage(Image img)
    {
        float timeElapsed = 0f;
        CanvasGroup canvasGroup = img.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = img.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;

        while (timeElapsed < fadeDurationImagen)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / fadeDurationImagen);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOutImage(Image img)
    {
        float timeElapsed = 0f;
        CanvasGroup canvasGroup = img.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = img.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;

        while (timeElapsed < fadeDurationImagen)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDurationImagen);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Zone1 2.0");
    }

    public void Continue()
    {
        //Codigo para continuar la partida
    }

    public void Credits()
    {
        //Code for credits
    }

    public void Exit()
    {
        // UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
    }

}