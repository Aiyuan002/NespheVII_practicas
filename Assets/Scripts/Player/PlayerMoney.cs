using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoney : MonoBehaviour
{
    public int dinero = 0;
    public TextMeshProUGUI textoDinero;
    public Image iconoMoneda;     
    public float padding = 8f;    

    void Start() => ActualizarTexto();

    public void AñadirDinero(int cantidad)
    {
        dinero += cantidad;
        ActualizarTexto();
    }

    public bool QuitarDinero(int cantidad)
    {
        if (dinero >= cantidad)
        {
            dinero -= cantidad;
            ActualizarTexto();
            return true;
        }
        return false;
    }

    void ActualizarTexto()
    {
        // 1) actualiza el texto
        textoDinero.text = "Dinero: " + dinero;

        // 2) fuerza un layout rebuild para que TMP calcule correctamente su tamaño
        LayoutRebuilder.ForceRebuildLayoutImmediate(textoDinero.rectTransform);

        // 3) obtén el ancho que ocupa el texto
        float w = textoDinero.preferredWidth;

        // 4) recoloca el icono de la moneda a la derecha, añadiendo padding
        Vector2 pos = iconoMoneda.rectTransform.anchoredPosition;
        pos.x = textoDinero.rectTransform.anchoredPosition.x + w + padding;
        iconoMoneda.rectTransform.anchoredPosition = pos;
    }
}
