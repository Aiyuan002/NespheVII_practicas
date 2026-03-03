using System.Collections;
using UnityEngine;
using TMPro;

public class MercaderInteract : MonoBehaviour
{
    [Header("Panel de Diálogo (Caja de texto abajo)")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    [Header("Panel del Mercader (Comprar, Vender, Salir)")]
    public GameObject panelMercader;

    [Header("Panel de Compra")]
    public GameObject panelCompra;

    [Header("Panel de Venta")]
    public GameObject panelVenta;

    [Header("Texto que dirá el mercader")]
    [TextArea(2, 4)]
    public string textoMercader = "¡Hola! Soy el mercader.\n¿Desea comprar o vender algún objeto?";

    private bool jugadorCerca = false;
    private bool dialogoActivo = false;

    void Start()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelMercader != null) panelMercader.SetActive(false);
        if (panelCompra != null) panelCompra.SetActive(false);
        if (panelVenta != null) panelVenta.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.F) && !dialogoActivo)
        {
            StartCoroutine(MostrarDialogoYMenu());
        }
    }

    IEnumerator MostrarDialogoYMenu()
    {
        dialogoActivo = true;

        if (panelDialogo != null && textoDialogo != null)
        {
            panelDialogo.SetActive(true);
            textoDialogo.text = textoMercader;
        }

        yield return new WaitForSeconds(0.7f);

        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelMercader != null) panelMercader.SetActive(true);

        dialogoActivo = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (panelDialogo != null) panelDialogo.SetActive(false);
            if (panelMercader != null) panelMercader.SetActive(false);
            if (panelCompra != null) panelCompra.SetActive(false);
            if (panelVenta != null) panelVenta.SetActive(false);
            dialogoActivo = false;
        }
    }

    public void BotonSalir()
    {
        Debug.Log(">>> Se pulsó el botón SALIR");
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelMercader != null) panelMercader.SetActive(false);
        if (panelCompra != null) panelCompra.SetActive(false);
        if (panelVenta != null) panelVenta.SetActive(false);
        dialogoActivo = false;
    }

    public void BotonComprar()
    {
        Debug.Log("Has pulsado Comprar.");
        if (panelMercader != null) panelMercader.SetActive(false);
        if (panelCompra != null) panelCompra.SetActive(true);
    }

    public void BotonVender()
    {
        Debug.Log("Has pulsado Vender.");
        if (panelMercader != null) panelMercader.SetActive(false);
        if (panelVenta != null) panelVenta.SetActive(true);
    }
}
