using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TiendaBasica : MonoBehaviour
{
    [Header("Panel Compra")]
    public GameObject panelCompra;
    public Button botonComprarVida;
    public Button botonComprarMana;
    public Button botonSalir;

    [Header("Dinero")]
    public PlayerMoney playerMoney;        

    [Header("Inventario")]
    public Inventory playerInventory;      

    [Header("Prefabs en venta")]
    public GameObject prefabPocionVida;    
    public GameObject prefabPocionMana;    

    [Header("Costes")]
    public int precioVida = 250;
    public int precioMana = 250;

    bool puedeComprar = true;

    void Start()
    {
        // Asignamos los listeners
        botonComprarVida.onClick.AddListener(ComprarVida);
        botonComprarMana.onClick.AddListener(ComprarMana);
        botonSalir.onClick.AddListener(CerrarTienda);

        // Arranca oculto
        panelCompra.SetActive(false);
    }

    void ComprarVida()
    {
        if (!puedeComprar) return;

        // Quita el dinero: si no hay, abortamos
        if (!playerMoney.QuitarDinero(precioVida))
        {
            Debug.Log("No tienes suficiente dinero para la poción de vida.");
            return;
        }

        // Instanciamos la poción (inactiva, sólo como contenedor de datos)
        GameObject go = Instantiate(prefabPocionVida);
        go.SetActive(false);
        Item item = go.GetComponent<Item>();

       
        if (playerInventory.AddItem(item))
        {
            Debug.Log("Has comprado una poción de vida.");
            
        }
        else
        {
            Debug.Log("Inventario lleno o stack completo. Devolviendo dinero.");
            playerMoney.AñadirDinero(precioVida);
            Destroy(go);
        }

        StartCooldown();
    }

    void ComprarMana()
    {
        if (!puedeComprar) return;

        if (!playerMoney.QuitarDinero(precioMana))
        {
            Debug.Log("No tienes suficiente dinero para la poción de maná.");
            return;
        }

        GameObject go = Instantiate(prefabPocionMana);
        go.SetActive(false);
        Item item = go.GetComponent<Item>();

        if (playerInventory.AddItem(item))
        {
            Debug.Log("Has comprado una poción de maná.");
        }
        else
        {
            Debug.Log("Inventario lleno o stack completo. Devolviendo dinero.");
            playerMoney.AñadirDinero(precioMana);
            Destroy(go);
        }

        StartCooldown();
    }

    void StartCooldown()
    {
        puedeComprar = false;
        Invoke(nameof(EndCooldown), 0.2f);
    }

    void EndCooldown() => puedeComprar = true;

    void CerrarTienda()
    {
        panelCompra.SetActive(false);
    }
}
