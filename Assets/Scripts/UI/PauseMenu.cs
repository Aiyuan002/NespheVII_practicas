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

        MenuPausa.SetActive(true);
        IsPaused = true;

        if (PauseGameManager.Instance != null)
            PauseGameManager.Instance.SetPausedByMenu(true);
    }

    public void Resume()
    {
        if (!IsPaused) return;

        MenuPausa.SetActive(false);

        if (MenuOpciones != null)
            MenuOpciones.SetActive(false);

        IsPaused = false;

        if (PauseGameManager.Instance != null)
            PauseGameManager.Instance.SetPausedByMenu(false);
    }

    public void PlayClick()
    {
        AudioManager.Instance.PlaySFX(clickSFX);
    }

    public void BackToMenu()
    {
        IsPaused = false;

        if (PauseGameManager.Instance != null)
            PauseGameManager.Instance.SetPausedByMenu(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
