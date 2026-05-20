using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
 
public class ItemPickupMessage : MonoBehaviour
{
    public static ItemPickupMessage I { get; private set; }
 
    [Header("Panels")]
    //public GameObject rootPanel;
    public GameObject bootPanel;
    public GameObject translatorPanel;
    public GameObject pioletPanel;
 
    [Header("Botones de continuar (opcional)")]
    [Tooltip("Si los botones ya están conectados en el Inspector del panel, déjalos vacíos aquí.")]
    public Button bootContinueButton;
    public Button translatorContinueButton;
    public Button pioletContinueButton;
 
    private bool waitingForInput = false;
 
    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }
 
    private void Start()
    {
        bootContinueButton?.onClick.AddListener(Continue);
        translatorContinueButton?.onClick.AddListener(Continue);
        pioletContinueButton?.onClick.AddListener(Continue);
    }
 
    private void Update()
    {
        if (!waitingForInput) return;
 
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            Continue();
    }
    public void ShowBootsMessage()   => StartCoroutine(ShowMessage(bootPanel));
    public void ShowTranslatorMessage() => StartCoroutine(ShowMessage(translatorPanel));
    public void ShowPioletMessage()  => StartCoroutine(ShowMessage(pioletPanel));
 

    private IEnumerator ShowMessage(GameObject panel)
    {
        yield return null;
 
        HideAllPanels();
        panel.SetActive(true);
 
        PauseGameManager.Instance?.SetPausedBySpellMenu(true);
        waitingForInput = true;
 
        yield return new WaitUntil(() => !waitingForInput);
    }
 
    public void Continue()
    {
        if (!waitingForInput) return;
 
        waitingForInput = false;
        HideAllPanels();
        PauseGameManager.Instance?.SetPausedBySpellMenu(false);
    }
 
    private void HideAllPanels()
    {
        bootPanel.SetActive(false);
        translatorPanel.SetActive(false);
        pioletPanel.SetActive(false);
    }
}