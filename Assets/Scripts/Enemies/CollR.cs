using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Queda ocultar al player para la animación de DeadRight. (SetActive false no funciona)
public class CollR : MonoBehaviour
{
    public UIController uiController;
    public GameObject Planta;
    public Animator animator;

    public GameObject Player;

    public bool canAttackR = true;

    void Start()
    {
        animator = Planta.GetComponent<Animator>();
        animator.SetBool("IdleAgain", false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            canAttackR = true;
            if (uiController.lifes <= 1)
            {
                animator.SetBool("AttackRight", false);
                animator.SetTrigger("DeadRight");
                animator.SetBool("IdleAgain", true);
            }
            else
            {
                animator.SetBool("AttackRight", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            canAttackR = false;
            animator.SetBool("AttackRight", false);
            animator.SetBool("IdleAgain", true);
        }
    }
}
