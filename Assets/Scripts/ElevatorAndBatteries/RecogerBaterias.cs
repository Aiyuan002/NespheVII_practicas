using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecogerBaterias : MonoBehaviour
{
    private RecargarAscensor scriptAscensor;

    private TextMeshPro inputKeycodeE;
    private bool oneTime = false;

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
            if (Input.GetKeyDown(KeyCode.E) && !oneTime)
            {
                oneTime = true;
                /*  Traductor traductor = FindObjectOfType<Traductor>();
                  traductor.isActiveTranslate = true;*/
                UIController uIController = FindFirstObjectByType<UIController>();
                uIController.ActiveEnergy();
                scriptAscensor.colleted++;
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        inputKeycodeE.enabled = false;
    }
}
