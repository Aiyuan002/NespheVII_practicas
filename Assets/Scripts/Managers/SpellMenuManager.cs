using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpellMenuManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject spellMenuPanel;
    public GameObject meleeAttackButton;

    [Header("Iconos de habilidades (siempre visibles)")]
    public GameObject[] spellIcons;

    [Header("Iconos de objetos recogibles")]
    public GameObject bootIcon;
    public GameObject translatorIcon;
    public GameObject pioletIcon;

    [Header("Configuración Hover")]
    public float hoverScale = 1.2f;
    public float scaleSpeed = 10f;

    public bool isOpen = false;
    private int hoveredIndex = -1;

    private List<GameObject> activeIcons = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        if (meleeAttackButton != null)
        {
            var trigger = meleeAttackButton.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) =>
            {
                if (((PointerEventData)data).button == PointerEventData.InputButton.Middle)
                    OpenMenu();
            });
            trigger.triggers.Add(entry);
        }
        if (spellMenuPanel != null)
            spellMenuPanel.SetActive(true);

        SetIconActive(bootIcon, false);
        SetIconActive(translatorIcon, false);
        SetIconActive(pioletIcon, false);

        foreach (var icon in spellIcons)
            if (icon != null) RegisterScale(icon);

        if (spellMenuPanel != null)
            spellMenuPanel.SetActive(false);
    }

    private void RegisterScale(GameObject icon)
    {
        if (icon != null && !originalScales.ContainsKey(icon))
        {
            originalScales[icon] = icon.transform.localScale;
            SetTextsVisible(icon, false);
        }
    }

    void Update()
    {
      
        if (!isOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (GetHoveredIconIndex() == -1)
            {
                CloseMenu();
                return;
            }
        }

        // Lógica de Hover
        int newHovered = GetHoveredIconIndex();
        if (newHovered != hoveredIndex)
        {
            // Apagar textos del anterior
            if (hoveredIndex >= 0 && hoveredIndex < activeIcons.Count)
                SetTextsVisible(activeIcons[hoveredIndex], false);

            hoveredIndex = newHovered;

            // Encender textos del nuevo
            if (hoveredIndex >= 0 && hoveredIndex < activeIcons.Count)
                SetTextsVisible(activeIcons[hoveredIndex], true);
        }

        AnimateIconScales();
    }

    public void OpenMenu()
    {
        if (isOpen) return;

        isOpen = true;
        spellMenuPanel.SetActive(true);
        RebuildActiveIcons();
        PauseGameManager.Instance?.SetPausedBySpellMenu(true);

        hoveredIndex = -1;

        // Reset visual inmediato al abrir
        foreach (var icon in activeIcons)
        {
            if (icon != null && originalScales.ContainsKey(icon))
            {
                icon.transform.localScale = originalScales[icon];
                SetTextsVisible(icon, false);
            }
        }
    }

    public void CloseMenu()
    {
        isOpen = false;
        spellMenuPanel.SetActive(false);
        PauseGameManager.Instance?.SetPausedBySpellMenu(false);
    }

    private void RebuildActiveIcons()
    {
        activeIcons.Clear();
        foreach (var icon in spellIcons)
            if (icon != null && icon.activeInHierarchy) activeIcons.Add(icon);

        if (bootIcon != null && bootIcon.activeInHierarchy) activeIcons.Add(bootIcon);
        if (translatorIcon != null && translatorIcon.activeInHierarchy) activeIcons.Add(translatorIcon);
        if (pioletIcon != null && pioletIcon.activeInHierarchy) activeIcons.Add(pioletIcon);

        // Asegurar que todos los activos tienen su escala registrada
        foreach (var icon in activeIcons) RegisterScale(icon);
    }

    private int GetHoveredIconIndex()
    {
        for (int i = 0; i < activeIcons.Count; i++)
        {
            if (activeIcons[i] == null) continue;
            RectTransform rt = activeIcons[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            bool contains = RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, null);
            Debug.Log($"Icon {activeIcons[i].name} | mouse: {Input.mousePosition} | contains: {contains}");

            if (contains) return i;
        }
        return -1;
    }

    private void SetTextsVisible(GameObject icon, bool visible)
    {
        if (icon == null) return;
        foreach (Transform child in icon.transform)
        {
            // Activamos/Desactivamos todo lo que sea hijo (descripciones)
            child.gameObject.SetActive(visible);
        }
    }

    private void AnimateIconScales()
    {
        float dt = Time.unscaledDeltaTime;
        for (int i = 0; i < activeIcons.Count; i++)
        {
            if (activeIcons[i] == null || !originalScales.ContainsKey(activeIcons[i])) continue;

            Vector3 original = originalScales[activeIcons[i]];
            float targetMult = (i == hoveredIndex) ? hoverScale : 1f;
            Vector3 targetScale = original * targetMult;

            activeIcons[i].transform.localScale = Vector3.Lerp(
                activeIcons[i].transform.localScale,
                targetScale,
                scaleSpeed * dt
            );
        }
    }

    public void UnlockItem(CollectibleItem item)
    {
        switch (item)
        {
            case CollectibleItem.Boots: SetIconActive(bootIcon, true); break;
            case CollectibleItem.Translator: SetIconActive(translatorIcon, true); break;
            case CollectibleItem.Piolet: SetIconActive(pioletIcon, true); break;
        }
    }

    private void SetIconActive(GameObject icon, bool active)
    {
        if (icon == null) return;
        icon.SetActive(active);
        if (active) RegisterScale(icon);
    }

    public enum CollectibleItem { Boots, Translator, Piolet }
}