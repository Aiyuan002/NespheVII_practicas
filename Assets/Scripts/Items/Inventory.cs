using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public enum InventoryAddResult
{
    Success,
    InventoryFull,
    MaxStackReached
}

public class Inventory : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int maxSlots = 3;

    [Header("UI Feedback")]
    [SerializeField] private Animator feedbackAnimator;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDuration = 2f;

    [Header("Localized Messages")]
    [SerializeField] private LocalizedString inventoryFullMessage;
    [SerializeField] private LocalizedString maxStackReachedMessage;

    private readonly List<InventorySlot> slots = new List<InventorySlot>();
    private Coroutine feedbackCoroutine;

    public delegate void InventoryUpdate();
    public static event InventoryUpdate OnInventoryUpdate;

    public InventoryAddResult AddItem(Item newItem)
    {
        if (newItem == null)
            return InventoryAddResult.InventoryFull;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item.id == newItem.id)
            {
                if (slot.quantity < slot.item.maxStack)
                {
                    slot.quantity++;
                    OnInventoryUpdate?.Invoke();
                    return InventoryAddResult.Success;
                }
                else
                {
                    ShowFeedback(maxStackReachedMessage);
                    return InventoryAddResult.MaxStackReached;
                }
            }
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(newItem));

            Debug.Log($"<color=green>[INVENTARIO]</color> Ítem añadido. Slots ocupados: {slots.Count}. Lanzando evento OnInventoryUpdate.");
            
            OnInventoryUpdate?.Invoke();
            return InventoryAddResult.Success;
        }

        ShowFeedback(inventoryFullMessage);
        return InventoryAddResult.InventoryFull;
    }

    public List<InventorySlot> GetSlots()
    {
        return slots;
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return;

        slots[slotIndex].quantity--;

        if (slots[slotIndex].quantity <= 0)
            slots.RemoveAt(slotIndex);

        OnInventoryUpdate?.Invoke();
    }

    private void ShowFeedback(LocalizedString localizedMessage)
    {
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(ShowFeedbackRoutine(localizedMessage));
    }

    private IEnumerator ShowFeedbackRoutine(LocalizedString localizedMessage)
    {
        var handle = localizedMessage.GetLocalizedStringAsync();
        yield return handle;

        if (feedbackText != null)
            feedbackText.text = handle.Result;

        if (feedbackAnimator != null)
        {
            feedbackAnimator.gameObject.SetActive(true);
            feedbackAnimator.Play("Fullinventory", 0, 0f);
        }

        yield return new WaitForSeconds(feedbackDuration);
    }
}

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
