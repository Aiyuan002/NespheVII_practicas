using System.Collections;
using System.Numerics;
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
}

public class Item : MonoBehaviour
{
    public int sellValue = 5; // Valor por defecto.
    public ItemType type;
    public Sprite icon;
    public string id;
    public int maxStack;

    public string beneficio;
    public string descripcion;
    public string precio;

    private Rigidbody2D rb;
    private Collider2D itemCollider;
    public GameObject dropPrefab;

    bool hasItem = false;
    bool canColleted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        itemCollider = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.gravityScale = 1;
            rb.freezeRotation = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasItem || canColleted)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("entra para querer pillarlo");
            //hasItem = true;
            Debug.Log("consumible 1 o 2");
            Inventory inventory = collision.gameObject.GetComponent<Inventory>();
            if (inventory.AddItem(this))
            {
                hasItem = true;
                Debug.Log("inventario full??");
                // Desactivar física y renderizado
                if (rb != null)
                    rb.simulated = false;
                itemCollider.enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public virtual void Use()
    {
        Debug.Log($"Usando item: {type}");
        //VIDA LEVEL 1
        UIController health = FindFirstObjectByType<UIController>();
        switch (type)
        {
            case ItemType.HealthPotion:
                health.RecoverHealth(25);
                break;
            case ItemType.ManaPotion:
                health.RecoverEnergy(25);
                break;
            case ItemType.HealthPotion_Level2:
                health.RecoverHealth(50);
                break;
            case ItemType.ManaPotion_Level2:
                health.RecoverEnergy(50);
                break;
            case ItemType.HealthPotion_Level3:
                Debug.Log("inventario deberia entrar");
                health.RecoverHealth(75);
                hasItem = false;
                break;
            case ItemType.ManaPotion_Level3:
                health.RecoverEnergy(75);
                break;
            case ItemType.AntivenomPotion:
                // Nada
                break;
            case ItemType.TradePotion:
                // Nada
                break;
            case ItemType.Pilas:
                // Nada
                break;
            case ItemType.Gema_Amarrilla:
                // Nada
                break;
            case ItemType.Gema_Marron:
                // Nada
                break;
            case ItemType.Gema_Morada:
                // Nada
                break;
            case ItemType.Gema_Verde:
                // Nada
                break;
            case ItemType.Gema_Roja:
                // Nada
                break;
            case ItemType.Gema_Azul:
                // Nada
                break;
            case ItemType.Gema_Negra:
                // Nada
                break;
            case ItemType.Gema_Cian:
                // Nada
                break;
            default:
                Debug.LogWarning("Tipo de ítem no reconocido: " + type);
                break;
        }
    }

    public virtual void Drop()
    {
        CharacterController player = FindFirstObjectByType<CharacterController>();

        switch (type)
        {
            case ItemType.HealthPotion:
            case ItemType.ManaPotion:
            case ItemType.HealthPotion_Level2:
            case ItemType.HealthPotion_Level3:
            case ItemType.ManaPotion_Level2:
            case ItemType.ManaPotion_Level3:
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
                GameObject droppedItem = Instantiate(
                    dropPrefab,
                    player.transform.position,
                    UnityEngine.Quaternion.identity
                );
                Item droppedItemScript = droppedItem.GetComponent<Item>();
                if (droppedItemScript != null)
                {
                    droppedItemScript.type = this.type;
                    droppedItemScript.icon = this.icon; // Si necesitas el icono
                    droppedItemScript.id = this.id; // Si necesitas el ID
                    droppedItemScript.maxStack = this.maxStack;
                    droppedItemScript.StartCoroutine(droppedItemScript.CourutineDrop(player)); // Llama a un método para activarlo
                }
                // EnableDrop(player);
                break;
            default:
                Debug.LogWarning("Tipo de ítem no reconocido: " + type);
                break;
        }
    }

    void EnableDrop(CharacterController player)
    {
        transform.position = player.GetTransform();
        itemCollider.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        rb.simulated = true;
        canColleted = true;
        StartCoroutine(CourutineDrop(player));
    }

    IEnumerator CourutineDrop(CharacterController player)
    {
        Rigidbody2D droppedRb = GetComponent<Rigidbody2D>();
        Collider2D droppedCollider = GetComponent<Collider2D>();
        SpriteRenderer droppedSprite = GetComponent<SpriteRenderer>();

        canColleted = true;
        transform.position = player.transform.position; // Usar transform.position directamente
        droppedCollider.enabled = true;
        droppedSprite.enabled = true;
        droppedRb.simulated = true;

        yield return new WaitForSeconds(2);
        canColleted = false;
        hasItem = false;
    }
}
