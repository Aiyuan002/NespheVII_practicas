using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class RadialMenu : MonoBehaviour
{
    [Header("Configuración UI")]
    [Tooltip("Iconos de cada habilidad en el mismo orden que en CharacterController.attack[]")]
    public List<Sprite> abilityIcons;
    [Tooltip("Prefab de segmento, que debe contener:\n  • Un componente Image para el icono\n  • Un child llamado \"Highlight\" con otro Image para el aro")]
    public GameObject segmentPrefab;
    [Tooltip("Radio (en píxeles) donde se colocan los iconos alrededor del centro")]
    public float radius = 150f;

    CanvasGroup cg;
    RectTransform rectT;
    Canvas rootCanvas;

    List<GameObject> segments = new List<GameObject>();
    int highlightedIdx = -1;  // índice actualmente bajo el cursor (mientras la rueda está abierta)
    int selectedIdx = 0;   // índice guardado al soltar ALT

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rectT = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();

        BuildMenu();
        Hide();
    }

    void Update()
    {
        // 1) Abrir / cerrar rueda con ALT
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            Show();

        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            // Al soltar ALT, guardamos la última posición resaltada
            if (highlightedIdx >= 0)
                selectedIdx = highlightedIdx;
            Hide();
        }

        // 2) Mientras la rueda esté visible, actualizamos el highlight según la posición del ratón
        if (cg.alpha > 0)
        {
            UpdateHighlightUnderMouse();
            return;
        }

        // 3) Cuando la rueda está oculta, clic derecho = ejecutar la habilidad seleccionada
        if (Input.GetMouseButtonDown(1))
            UseAbility(selectedIdx);
    }

    void BuildMenu()
    {
        int count = abilityIcons.Count;
        for (int i = 0; i < count; i++)
        {
            // Instanciar prefab
            GameObject seg = Instantiate(segmentPrefab, transform);
            segments.Add(seg);

            // Posición circular
            float angRad = (i * 360f / count) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad)) * radius;
            seg.GetComponent<RectTransform>().anchoredPosition = pos;

            // Asignar icono
            Image icon = seg.GetComponent<Image>();
            icon.sprite = abilityIcons[i];
            icon.preserveAspect = true;

            // Asegurarse de que el aro empiece desactivado
            var hi = seg.transform.Find("Highlight").GetComponent<Image>();
            hi.enabled = false;
        }
    }

    void UpdateHighlightUnderMouse()
    {
        // Convertir pantalla local dentro del RectTransform de la rueda
        Vector2 localMouse;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectT,
            Input.mousePosition,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out localMouse
        );

        // Calcular ángulo [0..360)
        float angle = Mathf.Atan2(localMouse.y, localMouse.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // Mapear ángulo a índice de segmento
        int count = segments.Count;
        int idx = Mathf.FloorToInt(angle / (360f / count));

        if (idx != highlightedIdx)
        {
            // Desactivar aro anterior
            if (highlightedIdx >= 0 && highlightedIdx < segments.Count)
                segments[highlightedIdx].transform.Find("Highlight").GetComponent<Image>().enabled = false;

            // Activar el nuevo
            highlightedIdx = idx;
            segments[idx].transform.Find("Highlight").GetComponent<Image>().enabled = true;
        }
    }

    void UseAbility(int idx)
    {
        if (idx < 0 || idx >= segments.Count) return;
        var cc = FindFirstObjectByType<CharacterController>();
        if (cc != null)
            cc.ExecuteSelectedAttack(idx);
    }

    public void Show()
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    public void Hide()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        // Limpiar cualquier highlight
        if (highlightedIdx >= 0 && highlightedIdx < segments.Count)
            segments[highlightedIdx].transform.Find("Highlight").GetComponent<Image>().enabled = false;
        highlightedIdx = -1;
    }
}
