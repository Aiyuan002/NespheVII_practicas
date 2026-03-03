using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int maxSlots = 3;

    private List<InventorySlot> slots = new List<InventorySlot>();

    public delegate void InventoryUpdate();
    public static event InventoryUpdate OnInventoryUpdate;

    [SerializeField]
    private Animator textFull;
    public bool isFull = false;

    public bool AddItem(Item newItem)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item.id == newItem.id && slot.quantity < newItem.maxStack)
            {
                slot.quantity++;

                Debug.Log($"Stack aumentado: {slot.item.name} (x{slot.quantity})");
                OnInventoryUpdate?.Invoke();

                return true;
            }
            else if (slot.item.id == newItem.id && slot.quantity >= newItem.maxStack)
            {
                Debug.Log("maximoAqui" + newItem.maxStack + " " + slot.quantity);
                OnInventoryUpdate?.Invoke();
                StartCoroutine(CourutineText());
                return false;
            }
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(newItem));
            Debug.Log($"Nuevo slot: {newItem.name}");
            OnInventoryUpdate?.Invoke();

            return true;
        }
        else
        {
            StartCoroutine(CourutineText());

            Debug.Log("Inventario lleno");
            return false;
        }
    }

    public List<InventorySlot> GetSlots()
    {
        return slots;
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < slots.Count)
        {
            slots[slotIndex].quantity--;

            // Si la cantidad llega a 0, eliminar el slot
            if (slots[slotIndex].quantity <= 0)
            {
                slots.RemoveAt(slotIndex);
                OnInventoryUpdate?.Invoke();
            }

            OnInventoryUpdate?.Invoke();
        }
    }

    /*  private bool IsInventoryFull()
      {
          // Si hay espacio para un nuevo slot, el inventario no está lleno.
          if (slots.Count < maxSlots)
              return false;
  
          // Recorremos todos los slots y verificamos si alguno tiene espacio para más items.
          foreach (InventorySlot slot in slots)
          {
              if (slot.quantity < slot.item.maxStack)
                  return false;
          }
          return true;
      }*/
    public IEnumerator CourutineText()
    {
        textFull.Play("Fullinventory");

        yield return new WaitForSeconds(2);
    }
}

// Clase para manejar los stacks
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;

    public InventorySlot(Item item)
    {
        this.item = item;
        quantity = 1;
    }
}
