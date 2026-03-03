using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    //[SerializeField] private GameObject ButtonPause;
    [SerializeField] private GameObject MenuPausa;
    [SerializeField] private AudioClip clickSFX;

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        Time.timeScale = 0f;
        //ButtonPause.SetActive(false);
        MenuPausa.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        //ButtonPause.SetActive(true);
        MenuPausa.SetActive(false);
        isPaused = false;
    }
    public void PlayClick()
    {
        AudioManager.Instance.PlaySFX(clickSFX);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}
