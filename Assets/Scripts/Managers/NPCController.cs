// NPCController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    static GameObject dialogBoxPrefab;
    public string dialogText;
    private bool isPlayerInRange = false;

    private void Start()
    {
        // Desactivar el cuadro de diálogo al inicio
        if (dialogBoxPrefab != null)
        {
            dialogBoxPrefab.SetActive(false);
        }
    }

    void Update()
    {
        // Si el jugador está en rango, mostrar el mensaje de pulsar F1
        if (isPlayerInRange)
        {
            // Aquí puedes mostrar un indicador visual o mensaje en la UI para indicar que se puede interactuar
        }
    }

    public void ShowDialog()
    {
        // Mostrar el cuadro de diálogo y empezar a mostrar el texto letra por letra
        if (dialogBoxPrefab != null)
        {
            dialogBoxPrefab.SetActive(true);
            StartCoroutine(ShowDialogText());
        }
    }

    IEnumerator ShowDialogText()
    {
        Text dialogTextComponent = dialogBoxPrefab.GetComponentInChildren<Text>();
        dialogTextComponent.text = "Bienvenido al Mundo de Hudeyrnas, soy Poppy";

        foreach (char letter in dialogText)
        {
            dialogTextComponent.text += letter;
            yield return new WaitForSeconds(0.1f); // Puedes ajustar el tiempo de espera entre letras
        }
    }

    // Resto de tu código
}
