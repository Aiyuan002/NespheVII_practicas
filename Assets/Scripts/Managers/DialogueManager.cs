using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager I { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject continuePrompt;

    [Header("Portraits")]
    [SerializeField] private Sprite protagonistPortrait;
    [SerializeField] private Sprite nativePortrait;

    [Header("Speaker Names")]
    [SerializeField] private LocalizedString protagonistName;
    [SerializeField] private LocalizedString nativeName;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset normalFont;
    [SerializeField] private TMP_FontAsset alienFont;

    [Header("Typewriter")]
    [SerializeField] private bool useTypewriter = true;
    [SerializeField] private float charDelay = 0.03f;

    public bool IsOpen { get; private set; }

    private Coroutine dialogueRoutine;
    private CharacterController currentPlayer;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

        if (panel != null)
            panel.SetActive(false);

        if (continuePrompt != null)
            continuePrompt.SetActive(false);
    }

    public void OpenDialogue(NPCDialogue npc, CharacterController playerController)
    {
        if (npc == null || playerController == null)
            return;

        bool hasTranslator = Traductor.I != null && Traductor.I.HasTranslator;
        DialogueLine[] lines = hasTranslator ? npc.withTranslatorLines : npc.noTranslatorLines;

        if (lines == null || lines.Length == 0)
            return;

        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        currentPlayer = playerController;
        dialogueRoutine = StartCoroutine(RunDialogue(lines));
    }

    private IEnumerator RunDialogue(DialogueLine[] lines)
    {
        IsOpen = true;

        if (currentPlayer != null)
            currentPlayer.SetControlEnabled(false);

        if (panel != null)
            panel.SetActive(true);

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

        for (int i = 0; i < lines.Length; i++)
        {
            DialogueLine currentLine = lines[i];

            ApplySpeaker(currentLine.speaker);
            ApplyTextStyle(currentLine.textStyle);

            string localizedText = currentLine.text.GetLocalizedString();
            yield return StartCoroutine(ShowLine(localizedText));

            if (continuePrompt != null)
                continuePrompt.SetActive(true);

            yield return StartCoroutine(WaitForAdvanceKey());

            if (continuePrompt != null)
                continuePrompt.SetActive(false);
        }

        CloseDialogue();
    }

    private IEnumerator ShowLine(string line)
    {
        if (dialogueText == null)
            yield break;

        if (!useTypewriter)
        {
            dialogueText.text = line;
            yield break;
        }

        dialogueText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(charDelay);
        }
    }

    private IEnumerator WaitForAdvanceKey()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.F))
                yield break;

            yield return null;
        }
    }

    private void ApplySpeaker(SpeakerId speaker)
    {
        switch (speaker)
        {
            case SpeakerId.Protagonist:
                if (portraitImage != null)
                    portraitImage.sprite = protagonistPortrait;

                if (speakerNameText != null)
                    speakerNameText.text = protagonistName.GetLocalizedString();
                break;

            case SpeakerId.Native:
                if (portraitImage != null)
                    portraitImage.sprite = nativePortrait;

                if (speakerNameText != null)
                    speakerNameText.text = nativeName.GetLocalizedString();
                break;
        }
    }

    private void ApplyTextStyle(DialogueTextStyle style)
    {
        if (dialogueText == null)
            return;

        switch (style)
        {
            case DialogueTextStyle.Alien:
                if (alienFont != null)
                    dialogueText.font = alienFont;
                break;

            default:
                if (normalFont != null)
                    dialogueText.font = normalFont;
                break;
        }
    }

    public void CloseDialogue()
    {
        if (!IsOpen)
            return;

        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        IsOpen = false;

        if (panel != null)
            panel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (speakerNameText != null)
            speakerNameText.text = "";

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

        if (currentPlayer != null)
            currentPlayer.SetControlEnabled(true);

        currentPlayer = null;
    }
}
