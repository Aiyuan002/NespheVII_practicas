using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.15f;
    private Vector3 originalScale;

    void Start() => originalScale = transform.localScale;

    public void OnPointerEnter(PointerEventData e) =>
        transform.localScale = originalScale * hoverScale;

    public void OnPointerExit(PointerEventData e) =>
        transform.localScale = originalScale;
}