using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHighlighted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    bool highlighted;
    private SubPanelPause subPanelPause;
    private Button button;

    void Start()
    {
        subPanelPause = GetComponent<SubPanelPause>();
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlighted = true;
    }

    // Se llama cuando el cursor sale del área del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
        animator.SetBool("Change", false);
        //animator.Play("SonidoAnimHighlighted");
    }

    void Update()
    {
        if (highlighted && !subPanelPause.panelActive)
        {
            highlighted = false;
            animator.SetBool("Change", true);
            //  animator.Play("SonidoAnim");
        }
        else if (subPanelPause.panelActive)
        {
            highlighted = false;
            animator.SetBool("Change", false);
        }
    }
}
