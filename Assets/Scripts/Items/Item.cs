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
        Debug.Log("COLISION con: " + collision.gameObject.name +
              " | Tag: " + collision.gameObject.tag +
              " | Layer: " + LayerMask.LayerToName(collision.gameObject.layer));
        TryPickup(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER con: " + collision.gameObject.name +
              " | Tag: " + collision.gameObject.tag +
              " | Layer: " + LayerMask.LayerToName(collision.gameObject.layer));
        TryPickup(collision.gameObject);
    }

    private void TryPickup(GameObject other)
    {
        if (hasBeenCollected || pickupBlocked)
        {
            Debug.Log("No recoge: bloqueado o ya recogido");
            return;
        }
        if (!other.CompareTag("Player"))
        {
            Debug.Log("No recoge: no es Player");
            return;
        }

        Inventory inventory = other.GetComponentInParent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("El Player no tiene componente Inventory");
            return;
        }

        InventoryAddResult result = inventory.AddItem(this);
        Debug.Log("Resultado AddItem: " + result);

        if (result == InventoryAddResult.Success)
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        hasBeenCollected = true;
        if (rb != null) rb.simulated = false;
        if (itemCollider != null) itemCollider.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        AudioManager.Instance?.PlayPickupItem(AudioManager.Instance.sounds.pickupItem_02);
    }

    // --- MÉTODO CORREGIDO ---
    public virtual bool Use()
    {
        UIController ui = FindFirstObjectByType<UIController>();
        if (ui == null) return false;

        switch (type)
        {
            // Lógica de SALUD
            case ItemType.HealthPotion:
                if (ui.currentHealth >= ui.maxHealth) return false;
                ui.RecoverHealth(25);
                return true;

            case ItemType.HealthPotion_Level2:
                if (ui.currentHealth >= ui.maxHealth) return false;
                ui.RecoverHealth(50);
                return true;

            case ItemType.HealthPotion_Level3:
                if (ui.currentHealth >= ui.maxHealth) return false;
                ui.RecoverHealth(75);
                return true;

            // Lógica de ENERGÍA
            case ItemType.ManaPotion:
                if (ui.currentEnergy >= ui.maxEnergy) return false;
                ui.RecoverEnergy(25);
                return true;

            case ItemType.ManaPotion_Level2:
                if (ui.currentEnergy >= ui.maxEnergy) return false;
                ui.RecoverEnergy(50);
                return true;

            case ItemType.ManaPotion_Level3:
                if (ui.currentEnergy >= ui.maxEnergy) return false;
                ui.RecoverEnergy(75);
                return true;

            // Lógica de VIDAS (Seta)
            case ItemType.SetaGreenHealth_1:
                if (ui.lifes >= 3) return false;
                ui.lifes += 1;
                ui.lifesText.text = ui.lifes.ToString();
                DrugsManager drugsManager = FindFirstObjectByType<DrugsManager>();
                if (drugsManager != null) drugsManager.TriggerEffect();
                return true;

            // Otros ítems que no tienen restricción de "barra llena"
            case ItemType.AntivenomPotion:
            case ItemType.TradePotion:
            case ItemType.Pilas:
                // Aquí podrías añadir lógica específica si la necesitas
                return true;

            default:
                return true;
        }
    }

    public virtual void Drop()
    {
        if (dropPrefab == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("No se encontró ningún objeto con tag Player.");
            return;
        }

        float direction = player.transform.localScale.x >= 0 ? 1f : -1f;

        Vector3 spawnPos = player.transform.position + new Vector3(direction * 0.35f, 0.1f, 0f);

        GameObject droppedItem = Instantiate(dropPrefab, spawnPos, Quaternion.identity);

        Item droppedItemScript = droppedItem.GetComponent<Item>();

        if (droppedItemScript != null)
        {
            droppedItemScript.CopyItemDataFrom(this);
            droppedItemScript.StartCoroutine(droppedItemScript.EnablePickupAfterDelay(spawnPos));
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

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (itemCollider != null) itemCollider.enabled = true;
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(pickupBlockTimeAfterDrop);
        pickupBlocked = false;
    }
}