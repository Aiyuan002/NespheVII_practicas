using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject MenuPausa;
    [SerializeField] private GameObject MenuOpciones;
    [SerializeField] private AudioClip clickSFX;

    public static bool IsPaused { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (IsPaused) return;

        Time.timeScale = 0f;
        MenuPausa.SetActive(true);
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsPaused) return;

        Time.timeScale = 1f;
        MenuPausa.SetActive(false);

        if (MenuOpciones != null)
            MenuOpciones.SetActive(false);

        IsPaused = false;
    }

    public void PlayClick()
    {
        AudioManager.Instance.PlaySFX(clickSFX);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}
