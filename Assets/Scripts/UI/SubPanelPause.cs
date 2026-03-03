using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubPanelPause : MonoBehaviour
{
    //Referencia al panel que quieres mostrar/ocultar
    public GameObject panel;
    public GameObject panel2;
    public GameObject panel3;

    //Variable para controlar el estado del panel
    public bool panelActive = false;

    public int panelNumber;

    public void TogglePanel()
    {
        panelActive = !panelActive;
        switch (panelNumber)
        {
            case 1:
                panel.SetActive(true);
                panel2.SetActive(false);
                panel3.SetActive(false);
                break;
            case 2:
                panel.SetActive(false);
                panel2.SetActive(true);
                panel3.SetActive(false);
                break;
            case 3:
                panel.SetActive(false);
                panel2.SetActive(false);
                panel3.SetActive(true);
                break;
            default:
                // Por si se pasa un número inválido, desactivar todos
                panel.SetActive(false);
                panel2.SetActive(false);
                panel3.SetActive(false);
                break;
        }
    }
}
