using UnityEngine;

public class PauseGameManager : MonoBehaviour
{
public static PauseGameManager Instance { get; private set; }

    [SerializeField] private CharacterController playerController;

    private bool pausedByMenu;
    private bool pausedByMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerController == null)
            playerController = FindFirstObjectByType<CharacterController>();

        ApplyPauseState();
    }

    public void SetPausedByMenu(bool value)
    {
        pausedByMenu = value;
        ApplyPauseState();
    }

    public void SetPausedByMap(bool value)
    {
        pausedByMap = value;
        ApplyPauseState();
    }

    public bool IsPausedByMenu => pausedByMenu;
    public bool IsPausedByMap => pausedByMap;
    public bool IsGamePaused => pausedByMenu || pausedByMap;

    private void ApplyPauseState()
    {
        bool shouldFreeze = pausedByMenu || pausedByMap;

        Time.timeScale = shouldFreeze ? 0f : 1f;

        if (playerController != null)
            playerController.SetControlEnabled(!shouldFreeze);
    }
}
