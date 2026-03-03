using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject descripcion;

    bool isActiveDes;

    [SerializeField]
    private Button[] slotButtons; // Arrastrar los botones de los slots en el Inspector

    [SerializeField]
    private Image[] slotIcons; // Arrastrar los componentes Image de los slots en el Inspector

    [SerializeField]
    private Sprite emptySlotSprite; // Sprite para slots vacíos
    private Inventory inventory; // Referencia al componente Inventory

    [SerializeField]
    private UIController UI; // Referencia al controlador de UI general

    [SerializeField]
    private TextMeshProUGUI[] stackCounters; // Contadores de cantidad por slot
    private int selectedSlot = -1; // Índice del slot seleccionado (-1 significa ninguno)
    public Color color; // Color para resaltar el slot seleccionado
    #region Inicialización
    private void Start()
    {
        // Obtener referencias a los componentes necesarios
        inventory = FindFirstObjectByType<Inventory>();
        UI = FindFirstObjectByType<UIController>();

        // Suscribirse al evento de actualización del inventario
        Inventory.OnInventoryUpdate += UpdateUI;

        // Asignar eventos a los botones de los slots
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i; // Capturar el índice para el closure
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index)); // Clic izquierdo

            // Configurar clic derecho mediante EventTrigger
            EventTrigger trigger =
                slotButtons[i].GetComponent<EventTrigger>()
                ?? slotButtons[i].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick,
            };
            entry.callback.AddListener(
                (data) =>
                {
                    if (((PointerEventData)data).button == PointerEventData.InputButton.Right)
                    {
                        OnSlotClickedRight(index); // Clic derecho
                    }
                }
            );
            trigger.triggers.Add(entry);
        }

        // Inicializar la UI
        UpdateUI();
    }
    #endregion

    /* public void OnPointerEnter(PointerEventData eventData)
     {
         // Verificar si el puntero está sobre un slotButton
         if (eventData.pointerEnter != null)
         {
             Button button = eventData.pointerEnter?.GetComponentInChildren<Button>();
             if (button != null && System.Array.IndexOf(slotButtons, button) != -1)
             {
                 int slotIndex = System.Array.IndexOf(slotButtons, button);
                 if (slotIndex >= 0 && slotIndex < inventory.GetSlots().Count)
                 {
                     InventorySlot slot = inventory.GetSlots()[slotIndex];
                     if (slot.item != null)
                     {
                         // Activar y actualizar la descripción
                         isActiveDes = true;
                         descripcion.SetActive(true);
 
                         TextMeshProUGUI descripcionText =
                             descripcion.GetComponentInChildren<TextMeshProUGUI>();
                         if (descripcionText != null)
                         {
                             descripcionText.text = $"Nombre: {slot.item.name}\nDescripción:";
                         }
                     }
                 }
             }
         }
     }*/

    public void OnPointerExit(PointerEventData eventData)
    {
        // Ocultar la descripción cuando el cursor sale
        isActiveDes = false;
        descripcion.SetActive(false);
    }

    #region Actualización
    private void Update()
    {
        // Seleccionar slots con teclas numéricas
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ToggleSlot(2);

        // Manejar activación con clic izquierdo si hay un slot seleccionado
        HandleActivationInput();
    }

    private void HandleActivationInput()
    {
        if (selectedSlot != -1 && Input.GetMouseButtonDown(0))
        {
            OnSlotClicked(selectedSlot);
        }
    }
    #endregion

    #region Selección de Slots
    void ToggleSlot(int index)
    {
        if (index >= 0 && index < inventory.GetSlots().Count)
        {
            InventorySlot slot = inventory.GetSlots()[index];
            if (slot.item != null)
            {
                if (selectedSlot == index)
                {
                    // Si el slot ya está seleccionado, deseleccionarlo
                    DeselectSlot();
                }
                else
                {
                    // Seleccionar un nuevo slot
                    SelectSlot(index);
                }
            }
            else
            {
                // Si el slot está vacío, deseleccionar cualquier slot seleccionado
                DeselectSlot();
            }
        }
    }

    void DeselectSlot()
    {
        if (selectedSlot != -1)
        {
            slotButtons[selectedSlot].image.color = Color.white; // Restablecer color
            selectedSlot = -1; // Ningún slot seleccionado
            descripcion.SetActive(false);
        }
    }

    void SelectSlot(int index)
    {
        if (selectedSlot != -1)
        {
            slotButtons[selectedSlot].image.color = Color.white; // Restablecer color del slot anterior
        }
        selectedSlot = index;

        RectTransform buttonRect = slotButtons[selectedSlot].GetComponent<RectTransform>();
        RectTransform descripcionRect = descripcion.GetComponent<RectTransform>();
        Vector2 anchoredPosition = buttonRect.anchoredPosition + new Vector2(-554f, 261f); // Desplazamiento fijo
        descripcionRect.anchoredPosition = anchoredPosition;

        if (index >= 0 && index < inventory.GetSlots().Count)
        {
            InventorySlot slot = inventory.GetSlots()[index];

            if (
                slot.item != null
                && (
                    slot.item.type == ItemType.HealthPotion
                    || slot.item.type == ItemType.HealthPotion_Level2
                    || slot.item.type == ItemType.HealthPotion_Level3
                    || slot.item.type == ItemType.ManaPotion
                    || slot.item.type == ItemType.ManaPotion_Level2
                    || slot.item.type == ItemType.ManaPotion_Level3
                    || slot.item.type == ItemType.Gema_Amarrilla
                    || slot.item.type == ItemType.Gema_Marron
                    || slot.item.type == ItemType.Gema_Morada
                    || slot.item.type == ItemType.Gema_Verde
                    || slot.item.type == ItemType.Gema_Roja
                    || slot.item.type == ItemType.Gema_Azul
                    || slot.item.type == ItemType.Gema_Negra
                    || slot.item.type == ItemType.Gema_Cian
                )
            )
            {
                // Activar y actualizar la descripción
                isActiveDes = true;
                descripcion.SetActive(true);

                TextMeshProUGUI descripcionText =
                    descripcion.GetComponentInChildren<TextMeshProUGUI>();
                if (descripcionText != null)
                {
                    descripcionText.text =
                        $"\r\n\r\n<color=#0a9a3a>{slot.item.beneficio}</color>\r\n\r\n\r\n<size=70%>{slot.item.descripcion}<size=90%><color=#E5BE01> {slot.item.precio}</color></size>)</size>";
                }
            }
        }
        slotButtons[selectedSlot].image.color = color; // Resaltar el nuevo slot
    }
    #endregion

    #region Limpieza
    private void OnDestroy()
    {
        // Desuscribirse del evento y limpiar listeners para evitar fugas de memoria
        Inventory.OnInventoryUpdate -= UpdateUI;
        foreach (var button in slotButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region Actualización de UI
    private void UpdateUI()
    {
        List<InventorySlot> slots = inventory.GetSlots();
        for (int i = 0; i < slotButtons.Length; i++)
        {
            bool slotOcupado = i < slots.Count;
            // Actualizar icono
            slotIcons[i].sprite = slotOcupado ? slots[i].item.icon : emptySlotSprite;

            // Actualizar contador de cantidad (si aplica)
            if (stackCounters != null && i < stackCounters.Length)
            {
                stackCounters[i].text =
                    slotOcupado && slots[i].quantity > 1 ? slots[i].quantity.ToString() : "";
            }

            // Habilitar/deshabilitar botón según si el slot está ocupado
            slotButtons[i].interactable = slotOcupado;
        }
    }
    #endregion

    #region Interacción con Slots
    private void OnSlotClicked(int slotIndex)
    {
        List<InventorySlot> slots = inventory.GetSlots();
        if (slotIndex < slots.Count && slots[slotIndex].item != null)
        {
            Item item = slots[slotIndex].item;
            Debug.Log("Slot clickeado con el botón izquierdo");

            // Usar pociones de salud si la vida no está llena
            if (
                (
                    item.type == ItemType.HealthPotion
                    || item.type == ItemType.HealthPotion_Level2
                    || item.type == ItemType.HealthPotion_Level3
                )
                && UI.healthSlider.value < UI.healthSlider.maxValue
            )
            {
                item.Use();
                inventory.RemoveItem(slotIndex);
                descripcion.SetActive(false);
            }
            // Usar pociones de maná si la energía no está llena
            else if (
                (
                    item.type == ItemType.ManaPotion
                    || item.type == ItemType.ManaPotion_Level2
                    || item.type == ItemType.ManaPotion_Level3
                )
                && UI.energySlider.value < UI.energySlider.maxValue
            )
            {
                item.Use();
                inventory.RemoveItem(slotIndex);
                descripcion.SetActive(false);
            }
            else
            {
                DeselectSlot(); // Deseleccionar si no se usa el item
            }
        }
    }

    private void OnSlotClickedRight(int slotIndex)
    {
        List<InventorySlot> slots = inventory.GetSlots();
        if (slotIndex < slots.Count && slots[slotIndex].item != null)
        {
            Item item = slots[slotIndex].item;
            Debug.Log("Slot clickeado con el botón derecho");

            // Soltar pociones (salud o maná)
            if (
                item.type == ItemType.HealthPotion
                || item.type == ItemType.HealthPotion_Level2
                || item.type == ItemType.HealthPotion_Level3
                || item.type == ItemType.ManaPotion
                || item.type == ItemType.ManaPotion_Level2
                || item.type == ItemType.ManaPotion_Level3
            )
            {
                item.Drop();
                inventory.RemoveItem(slotIndex);
                DeselectSlot();
                descripcion.SetActive(false);
            }
        }
    }
    #endregion
}
