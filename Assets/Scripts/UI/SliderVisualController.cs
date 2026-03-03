using UnityEngine;
using UnityEngine.EventSystems;

public class SliderVisualController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Animator sliderAnimator;
    public Animator handleAnimator;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetMoving(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetMoving(false);
    }

    private void SetMoving(bool state)
    {
        if (sliderAnimator) sliderAnimator.SetBool("isMoving", state);
    }
}
