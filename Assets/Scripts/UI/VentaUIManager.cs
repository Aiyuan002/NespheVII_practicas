using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VentaUIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelVenta;
    public Transform contenedorItems;
    public GameObject prefabBotonItem;      // Prefab con hijos: Text (TMP) e Image llamado "Icono"
    public TextMeshProUGUI textoPrecio;     // Muestra el precio del item (o el total preview)
    public Button botonVender;              // Vender 1 del slot seleccionado
    public Button botonVenderTodo;          // Vender todo en dos clics (preview + confirm)

    [Header("Lógica")]
    public Inventory playerInventory;
    public PlayerMoney playerMoney;

    private int indexSeleccionado = -1;
    private bool previewVenderTodo = false;
    private int totalPreview = 0;

    void Start()
    {
        // Conectar botones (asegúrate de asignarlos en el Inspector)
        botonVender.onClick.RemoveAllListeners();
        botonVender.onClick.AddListener(VenderItem);
        botonVenderTodo.onClick.RemoveAllListeners();
        botonVenderTodo.onClick.AddListener(VenderTodo);
    }

    void OnEnable()
    {
        RefrescarLista();
        previewVenderTodo = false;
        totalPreview = 0;
    }

    void RefrescarLista()
    {
        // 1) limpio viejos botones
        foreach (Transform t in contenedorItems)
            Destroy(t.gameObject);

        indexSeleccionado = -1;
        textoPrecio.text = "0";

        // 2) por cada slot creo un botón
        List<InventorySlot> slots = playerInventory.GetSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var btn = Instantiate(prefabBotonItem, contenedorItems, false);

            // A) texto = nombre y cantidad
            var texto = btn.GetComponentInChildren<TextMeshProUGUI>();
            texto.text = $"{slot.item.type} x{slot.quantity}";

            // B) icono
            var iconTransform = btn.transform.Find("Icono");
            if (iconTransform != null)
            {
                var img = iconTransform.GetComponent<Image>();
                if (img != null) img.sprite = slot.item.icon;
            }

            // C) listener para seleccionar
            var b = btn.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            int idx = i;
            b.onClick.AddListener(() => Seleccionar(idx));
        }
    }

    public void Seleccionar(int idx)
    {
        indexSeleccionado = idx;
        previewVenderTodo = false;
        totalPreview = 0;

        int precio = playerInventory.GetSlots()[idx].item.sellValue;
        textoPrecio.text = precio.ToString();
    }

    public void VenderItem()
    {
        if (indexSeleccionado < 0) return;

        var slot = playerInventory.GetSlots()[indexSeleccionado];
        int valor = slot.item.sellValue;

        // Quitar UNIDAD del slot seleccionado
        playerInventory.RemoveItem(indexSeleccionado);
        playerMoney.AñadirDinero(valor);

        RefrescarLista();
    }

    public void VenderTodo()
    {
        if (!previewVenderTodo)
        {
            // Primer clic,  calculo total y muestro preview
            int suma = 0;
            foreach (var slot in playerInventory.GetSlots())
                suma += slot.item.sellValue * slot.quantity;

            totalPreview = suma;
            textoPrecio.text = suma.ToString();
            previewVenderTodo = true;
        }
        else
        {
            // Segundo clic ,  ejecuto venta real
            if (totalPreview > 0)
            {
                playerMoney.AñadirDinero(totalPreview);

                // ELIMINO **todos** los slots reales, de atrás hacia delante
                // para que los índices no se desplacen mientras borro
                var realSlots = playerInventory.GetSlots();
                for (int i = realSlots.Count - 1; i >= 0; i--)
                {
                    
                    playerInventory.RemoveItem(i);
                }
            }

        
            RefrescarLista();
            textoPrecio.text = "0";
            previewVenderTodo = false;
            totalPreview = 0;
        }
    }

    public void CerrarPanelVenta()
    {
        panelVenta.SetActive(false);
    }
}
