using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public GameObject optionsPanel;
    private bool isPaused = false;
    private InputAction escapeAction;
    public Animator viñeta;

    [SerializeField]
    private GameObject[] subPanels;

    [Header("Exit")]
    public Button exitButton;

    private void Start()
    {
        optionsPanel.SetActive(false);
        escapeAction = new InputAction("Escape", binding: "<Keyboard>/escape");

        escapeAction.Enable();
        escapeAction.performed += _ => OnEscapePressed();
    }

    private void OnEscapePressed()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Detiene el tiempo del juego
        isPaused = true;
        optionsPanel.SetActive(true); // Muestra el panel de opciones
        viñeta.Play("AnimacionViñeta");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Reanuda el tiempo del juego
        isPaused = false;
        optionsPanel.SetActive(false); // Oculta el panel de opciones
        subPanels[0].SetActive(false);
        subPanels[1].SetActive(false);
        viñeta.Play("AnimacionViñetaInversa");
    }

    public void Exit()
    {
        //Salgo del juego
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Me salgo");
    }
}
