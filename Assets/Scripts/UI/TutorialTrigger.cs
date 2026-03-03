using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public int stepIndex;

    private bool playerInside = false;
    private TutorialManager tutorialManager;

    void Start()
    {
        tutorialManager = FindFirstObjectByType<TutorialManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && tutorialManager != null)
        {
            playerInside = true;
            tutorialManager.ShowStepMessage(stepIndex);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && tutorialManager != null)
        {
            playerInside = false;
            tutorialManager.HideMessageIfStillActive(stepIndex);
        }
    }

    void Update()
    {
        if (playerInside && tutorialManager != null)
        {
            tutorialManager.CheckStepInput(stepIndex);
        }
    }
}