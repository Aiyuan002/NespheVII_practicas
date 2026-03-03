using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Text tutorialText;
    public GameObject skipButton;
    public SceneFader sceneFader;

    private string[] stepMessages = {
        "Usa A/D o ←/→ para moverte. Presiona shift izquierdo para correr.",
        "Presiona ESPACIO para saltar.",
        "Presiona S o ↓ para agacharte o Q para deslizarte.",
        "Presiona W o ↑ para escalar.",
        "Haz clic IZQUIERDO para disparar.",
        "Haz clic DERECHO para dar un puñetazo. Este ataque consume energía."
    };

    private bool[] stepCompleted;
    private int currentDisplayedStep = -1;

    void Start()
    {
        stepCompleted = new bool[stepMessages.Length];
        skipButton.SetActive(true);
        tutorialText.text = "";
    }

    public void ShowStepMessage(int index)
    {
        if (!stepCompleted[index])
        {
            tutorialText.text = stepMessages[index];
            currentDisplayedStep = index;
        }
    }

    public void HideMessageIfStillActive(int index)
    {
        if (currentDisplayedStep == index)
        {
            tutorialText.text = "";
            currentDisplayedStep = -1;
        }
    }

    public void CheckStepInput(int index)
    {
        if (stepCompleted[index]) return;

        switch (index)
        {
            case 0:
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                    Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                    CompleteStep(index);
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                    CompleteStep(index);
                break;
            case 2:
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Q))
                    CompleteStep(index);
                break;
            case 3:
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    CompleteStep(index);
                break;
            case 4:
                if (Input.GetMouseButtonDown(0))
                    CompleteStep(index);
                break;
            case 5:
                if (Input.GetMouseButtonDown(1))
                    CompleteStep(index);
                break;
        }
    }

    void CompleteStep(int index)
    {
        stepCompleted[index] = true;
        tutorialText.text = "";
        currentDisplayedStep = -1;

        if (AllStepsCompleted())
        {
            sceneFader.FadeToScene(1);
        }
    }

    bool AllStepsCompleted()
    {
        foreach (bool completed in stepCompleted)
        {
            if (!completed) return false;
        }
        return true;
    }

    public void SkipTutorial()
    {
        sceneFader.FadeToScene(1);
    }
}