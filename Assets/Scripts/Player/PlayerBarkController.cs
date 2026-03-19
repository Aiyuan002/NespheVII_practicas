using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBarkController : MonoBehaviour
{
    public enum BarkContext
    {
        Idle,
        PostEnemyEncounter,
        TransporterNoEnergy,
        MidBoss,
        FinalBoss
    }

    [Header("Refs")]
    [SerializeField] private CharacterController player;
    [SerializeField] private GameObject barkPanel;
    [SerializeField] private TextMeshProUGUI barkText;
    [SerializeField] private TextMeshProUGUI skipPrompt;

    [Header("Writing Effect")]
    [SerializeField] private bool useTypewriter = true;
    [SerializeField] private float charDelay = 0.02f;

    [Header("Idle Settings")]
    [SerializeField] private bool enableIdleBarks = true;
    [SerializeField] private float idleTimeBeforeFirstBark = 5f;
    [SerializeField] private float idleInterval = 8f;

    [Header("Auto Hide")]
    [SerializeField] private float baseShowSeconds = 1.5f;
    [SerializeField] private float secondsPerCharacter = 0.035f;
    [SerializeField] private float minShowSeconds = 1.5f;
    [SerializeField] private float maxShowSeconds = 8f;

    [Header("Anti-spam")]
    [SerializeField] private float globalCooldown = 1.0f;

    [Header("Phrases")]
    public LocalizedString[] idlePhrases;
    public LocalizedString[] postEnemyEncounterPhrases;
    public LocalizedString[] transporterNoEnergyPhrases;
    public LocalizedString[] midBossPhrases;
    public LocalizedString[] finalBossPhrases;

    private float idleTimer = 0f;
    private float idleIntervalTimer = 0f;

    private bool showing = false;
    private bool typing = false;

    private int idleIndex = 0;
    private int postEnemyIndex = 0;
    private int transporterIndex = 0;
    private int midBossIndex = 0;
    private int finalBossIndex = 0;

    private float lastShowTime = -999f;
    private Coroutine showRoutine;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        if (barkPanel != null)
            barkPanel.SetActive(false);

        if (skipPrompt != null)
            skipPrompt.enabled = false;
    }

    private void Update()
    {
        if (DialogueManager.I != null && DialogueManager.I.IsOpen)
        {
            if (showing)
                ForceHide();

            return;
        }

        if (showing && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ForceHide();
            return;
        }

        if (!enableIdleBarks || player == null)
            return;

        bool isIdle = player.IsIdleForBarks();

        if (!isIdle)
        {
            idleTimer = 0f;
            idleIntervalTimer = 0f;
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer < idleTimeBeforeFirstBark)
            return;

        idleIntervalTimer += Time.deltaTime;

        if (!showing && idleIntervalTimer >= idleInterval)
        {
            idleIntervalTimer = 0f;
            TryShowBark(BarkContext.Idle);
        }
    }

    public void TriggerPostEnemyEncounterBark()
        => TryShowBark(BarkContext.PostEnemyEncounter);

    public void TriggerTransporterNoEnergyBark()
        => TryShowBark(BarkContext.TransporterNoEnergy);

    public void TriggerMidBossBark()
        => TryShowBark(BarkContext.MidBoss);

    public void TriggerFinalBossBark()
        => TryShowBark(BarkContext.FinalBoss);

    private void TryShowBark(BarkContext context)
    {
        if (DialogueManager.I != null && DialogueManager.I.IsOpen)
            return;

        if (Time.time - lastShowTime < globalCooldown)
            return;

        LocalizedString phrase = GetNextPhrase(context);

        if (phrase.IsEmpty)
            return;

        string text = phrase.GetLocalizedString();

        if (string.IsNullOrWhiteSpace(text))
            return;

        showRoutine = StartCoroutine(ShowRoutine(text));
        lastShowTime = Time.time;
    }

    private LocalizedString GetNextPhrase(BarkContext context)
    {
        switch (context)
        {
            case BarkContext.Idle:
                return GetCycled(idlePhrases, ref idleIndex);

            case BarkContext.PostEnemyEncounter:
                return GetCycled(postEnemyEncounterPhrases, ref postEnemyIndex);

            case BarkContext.TransporterNoEnergy:
                return GetCycled(transporterNoEnergyPhrases, ref transporterIndex);

            case BarkContext.MidBoss:
                return GetCycled(midBossPhrases, ref midBossIndex);

            case BarkContext.FinalBoss:
                return GetCycled(finalBossPhrases, ref finalBossIndex);
        }

        return default;
    }

    private LocalizedString GetCycled(LocalizedString[] arr, ref int idx)
    {
        if (arr == null || arr.Length == 0)
            return default;

        if (idx < 0)
            idx = 0;

        if (idx >= arr.Length)
            idx = 0;

        LocalizedString chosen = arr[idx];

        idx = (idx + 1) % arr.Length;

        return chosen;
    }

    private IEnumerator ShowRoutine(string text)
    {
        showing = true;
        typing = false;

        if (barkPanel != null)
            barkPanel.SetActive(true);

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkSkipPrompt());

        if (!useTypewriter)
        {
            barkText.text = text;
        }
        else
        {
            typing = true;
            barkText.text = "";

            foreach (char c in text)
            {
                barkText.text += c;

                yield return new WaitForSeconds(charDelay);

                if (!showing)
                    yield break;
            }

            typing = false;
        }

        float t = baseShowSeconds + (text.Length * secondsPerCharacter);
        t = Mathf.Clamp(t, minShowSeconds, maxShowSeconds);

        float timer = 0f;

        while (timer < t)
        {
            timer += Time.deltaTime;

            if (!showing)
                yield break;

            yield return null;
        }

        ForceHide();
    }

    private IEnumerator BlinkSkipPrompt()
    {
        if (skipPrompt == null)
            yield break;

        while (true)
        {
            skipPrompt.enabled = !skipPrompt.enabled;
            yield return new WaitForSeconds(0.8f);
        }
    }

    private void ForceHide()
    {
        showing = false;
        typing = false;

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (skipPrompt != null)
            skipPrompt.enabled = false;

        if (barkText != null)
            barkText.text = "";

        if (barkPanel != null)
            barkPanel.SetActive(false);
    }
}