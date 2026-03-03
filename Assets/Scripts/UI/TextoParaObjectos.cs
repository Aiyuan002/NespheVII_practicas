using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextoParaObjectos : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMeshProUGUI;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textMeshProUGUI.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textMeshProUGUI.enabled = false;
        }
    }
}
