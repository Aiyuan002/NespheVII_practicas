// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class dialogocharacter : MonoBehaviour
{
    public float interactionDistance = 2f; // Distancia de interacción
    public KeyCode interactionKey = KeyCode.F; // Tecla de interacción (cambiada a KeyCode.F para que sea coherente con InputSystem)
    public bool isNearNPC = false; // Indica si el jugador está cerca del NPC
    public GameObject text;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.Talk.performed += OnTalkPerformed;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Gameplay.Talk.performed -= OnTalkPerformed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = true;
            text.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = false;
            text.SetActive(false);
        }
    }

    private void OnTalkPerformed(InputAction.CallbackContext context)
    {
        if (isNearNPC)
        {
            Talk();
        }
    }

    private void Talk()
    {
        Debug.Log("HABLAA");
        // Aquí puedes realizar cualquier acción relacionada con el diálogo del NPC
    }
}
