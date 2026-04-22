using System.Collections;
using UnityEngine;

public enum ItemType
{
    HealthPotion,
    ManaPotion,
    HealthPotion_Level2,
    ManaPotion_Level2,
    HealthPotion_Level3,
    ManaPotion_Level3,
    AntivenomPotion,
    TradePotion,
    Pilas,
    Gema_Amarrilla,
    Gema_Marron,
    Gema_Morada,
    Gema_Verde,
    Gema_Roja,
    Gema_Azul,
    Gema_Negra,
    Gema_Cian,
    SetaGreenHealth_1
}

public class Item : MonoBehaviour
{
    [Header("Item Data")]
    public int sellValue = 5;
    public ItemType type;
    public Sprite icon;
    public string id;
    public int maxStack = 1;

    [TextArea] public string beneficio;
    [TextArea] public string descripcion;
    public string precio;

    [Header("Drop")]
    public GameObject dropPrefab;
    [SerializeField] private float pickupBlockTimeAfterDrop = 1.25f;

    protected Rigidbody2D rb;
    protected Collider2D itemCollider;
    protected SpriteRenderer spriteRenderer;

    private bool hasBeenCollected = false;
    private bool pickupBlocked = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        itemCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if (rb != null)
        {
            rb.gravityScale = 1;
            rb.freezeRotation = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryPickup(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryPickup(collision.gameObject);
    }

    private void TryPickup(GameObject other)
    {
        if (hasBeenCollected || pickupBlocked)
            return;

        if (!other.CompareTag("Player"))
            return;

        Inventory inventory = other.GetComponent<Inventory>();
        Debug.Log($"<color=cyan>[ITEM]</color> {name} detectó al jugador. Intentando añadir al inventario...");
        if (inventory == null)
        {
            Debug.LogWarning("El Player no tiene componente Inventory.");
            return;
        }
        Debug.Log($"Intentando recoger {name} | id={id} | maxStack={maxStack}");
        InventoryAddResult result = inventory.AddItem(this);

        Debug.Log($"<color=cyan>[ITEM]</color> Resultado de añadir {name}: {result}");

        if (result == InventoryAddResult.Success)
        {
            CollectItem();
        }
        else
        {
            // No desaparece. Se queda en el suelo.
            Debug.Log($"No se pudo recoger {name}. Motivo: {result}");
        }
    }

    private void CollectItem()
    {
        Debug.Log($"<color=orange>[ALERTA]</color> {name} está siendo recolectada AHORA MISMO.");
        hasBeenCollected = true;

        if (rb != null)
            rb.simulated = false;

        if (itemCollider != null)
            itemCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    public virtual void Use()
    {
        Debug.Log($"Usando item: {type}");

        UIController ui = FindFirstObjectByType<UIController>();
        if (ui == null)
        {
            Debug.LogWarning("No se encontró UIController.");
            return;
        }

        switch (type)
        {
            case ItemType.HealthPotion:
                ui.RecoverHealth(25);
                break;

            case ItemType.ManaPotion:
                ui.RecoverEnergy(25);
                break;

            case ItemType.HealthPotion_Level2:
                ui.RecoverHealth(50);
                break;

            case ItemType.ManaPotion_Level2:
                ui.RecoverEnergy(50);
                break;

            case ItemType.HealthPotion_Level3:
                ui.RecoverHealth(75);
                break;

            case ItemType.ManaPotion_Level3:
                ui.RecoverEnergy(75);
                break;

            case ItemType.SetaGreenHealth_1:
                if (ui.lifes < 3)
                {
                    ui.lifes += 1;
                    ui.lifesText.text = ui.lifes.ToString();
                }
                break;

            case ItemType.AntivenomPotion:
            case ItemType.TradePotion:
            case ItemType.Pilas:
            case ItemType.Gema_Amarrilla:
            case ItemType.Gema_Marron:
            case ItemType.Gema_Morada:
            case ItemType.Gema_Verde:
            case ItemType.Gema_Roja:
            case ItemType.Gema_Azul:
            case ItemType.Gema_Negra:
            case ItemType.Gema_Cian:
                break;

            default:
                Debug.LogWarning("Tipo de ítem no reconocido: " + type);
                break;
        }
    }

    public virtual void Drop()
    {
        CharacterController player = FindFirstObjectByType<CharacterController>();
        if (player == null)
        {
            Debug.LogWarning("No se encontró CharacterController para soltar el item.");
            return;
        }

        if (dropPrefab == null)
        {
            Debug.LogWarning("No hay dropPrefab asignado.");
            return;
        }

        GameObject droppedItem = Instantiate(
            dropPrefab,
            player.transform.position,
            Quaternion.identity
        );

        Item droppedItemScript = droppedItem.GetComponent<Item>();
        if (droppedItemScript != null)
        {
            droppedItemScript.CopyItemDataFrom(this);
            droppedItemScript.StartCoroutine(
                droppedItemScript.EnablePickupAfterDelay(player.transform.position)
            );
        }
    }

    protected void CopyItemDataFrom(Item source)
    {
        type = source.type;
        icon = source.icon;
        id = source.id;
        maxStack = source.maxStack;
        beneficio = source.beneficio;
        descripcion = source.descripcion;
        precio = source.precio;
        sellValue = source.sellValue;
    }

    private IEnumerator EnablePickupAfterDelay(Vector3 spawnPosition)
    {
        pickupBlocked = true;
        hasBeenCollected = false;

        transform.position = spawnPosition;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (itemCollider != null)
            itemCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(pickupBlockTimeAfterDrop);
        pickupBlocked = false;
    }
}
