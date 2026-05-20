using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager I { get; private set; }

    [Header("Steps")]
    [SerializeField] private GameObject[] steps;

    [Header("Panels")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private GameObject askPanel;

    private PlayerMainAttack playerController;
    private int currentStep = 0;
    private bool isRunning = false;

    private const string PREFS_KEY = "tutorialDone";

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerMainAttack>();

        rootPanel.SetActive(false);

        //if (!PlayerPrefs.HasKey(PREFS_KEY))
        //{
            GameState.I.SetTutorial(true);
            PauseGameManager.Instance?.SetPausedByTutorial(true);
            rootPanel.SetActive(true);
            askPanel.SetActive(true);
            HideAllSteps();
       // }
    }

    private void Update()
    {
        if (!isRunning) return;

        if (Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            NextStep();
        }
    }

    // ─── Ask Panel ────────────────────────────────────────────────

    public void OnClickYes()
    {

        askPanel.SetActive(false);
        rootPanel.SetActive(false);
        GameState.I.SetTutorial(false);
        PauseGameManager.Instance?.SetPausedByTutorial(false);
        SaveTutorialDone();
    }

    public void OnClickNo()
    {
        askPanel.SetActive(false);
        StartTutorial();
    }

    // ─── Flujo ────────────────────────────────────────────────────

    private void StartTutorial()
    {
        isRunning = true;
        currentStep = 0;

        if (playerController != null)

        ShowCurrentStep();
    }

    public void NextStep()
    {
        currentStep++;

        if (currentStep < steps.Length)
        {
            ShowCurrentStep();
        }
        else
        {
            EndTutorial();
        }
    }

    private void ShowCurrentStep()
    {
        for (int i = 0; i < steps.Length; i++)
            steps[i].SetActive(i == currentStep);
    }

    private void HideAllSteps()
    {
        foreach (var step in steps)
            step.SetActive(false);
    }

    private void EndTutorial()
    {
        isRunning = false;
        HideAllSteps();
        rootPanel.SetActive(false);

        if (playerController != null)

            GameState.I.SetTutorial(false);
        PauseGameManager.Instance?.SetPausedByTutorial(false);
        SaveTutorialDone();
    }

    private void SaveTutorialDone()
    {
        PlayerPrefs.SetInt(PREFS_KEY, 1);
        PlayerPrefs.Save();
    }

    // ─── Reset para pruebas ───────────────────────────────────────

    [ContextMenu("Reset Tutorial PlayerPref")]
    private void ResetTutorialPref()
    {
        PlayerPrefs.DeleteKey(PREFS_KEY);
        Debug.Log("Tutorial reseteado.");
    }
}