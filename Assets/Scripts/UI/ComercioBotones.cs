using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComercioBotones : MonoBehaviour
{
    public GameObject attacks;
    public GameObject minimap;
    public GameObject textPulsaF;
    public GameObject PanelDialogo;
    public Text textDialogo;

    public void Salir()
    {
        PanelDialogo.SetActive(false);
        attacks.SetActive(true);
        minimap.SetActive(true);
        textDialogo.text = "";
        //CharacterController.ResetDialog();
    }
}
