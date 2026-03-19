using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecogerBaterias : MonoBehaviour
{
    private RecargarAscensor scriptAscensor;

    private TextMeshPro inputKeycodeE;
    private bool oneTime = false;

    private bool isPlayerInRange = false;

    private void OnEnable()
    {
        CharacterController.OnPlayerInteract += InteractWithBattery;
    }

    private void OnDisable()
    {
        CharacterController.OnPlayerInteract -= InteractWithBattery;
    }

    void Start()
    {
        inputKeycodeE = GetComponentInChildren<TextMeshPro>();
        scriptAscensor = FindFirstObjectByType<RecargarAscensor>();
        inputKeycodeE.enabled = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inputKeycodeE.enabled = true;
            isPlayerInRange = true;
        }
    }

    private void InteractWithBattery()
    {
        if(!oneTime && isPlayerInRange)
        {
            oneTime = true;
            UIController uIController = FindFirstObjectByType<UIController>();
            uIController.ActiveEnergy();
            scriptAscensor.colleted++;
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        inputKeycodeE.enabled = false;
        isPlayerInRange = false;
    }
}
