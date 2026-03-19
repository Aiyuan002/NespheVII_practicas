using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDialogueInteractor : MonoBehaviour
{
    [SerializeField] private GameObject interactPrompt; // "Pulsa F"
    [SerializeField] private CharacterController playerController; // tu script

    private PlayerControls controls;
    private NPCDialogue currentNpc;

    private void Awake()
    {
        controls = new PlayerControls();
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (playerController == null) playerController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.Talk.performed += OnTalk;
    }

    private void OnDisable()
    {
        controls.Gameplay.Talk.performed -= OnTalk;
        controls.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var npc = other.GetComponent<NPCDialogue>();
        if (npc == null) return;

        currentNpc = npc;
        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var npc = other.GetComponent<NPCDialogue>();
        if (npc == null || npc != currentNpc) return;

        currentNpc = null;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void OnTalk(InputAction.CallbackContext ctx)
    {
        if (currentNpc == null) return;
        if (DialogueManager.I != null && DialogueManager.I.IsOpen) return;

        if (interactPrompt != null) interactPrompt.SetActive(false);

        DialogueManager.I.OpenDialogue(currentNpc, playerController);
    }
}
