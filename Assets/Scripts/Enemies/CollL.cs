using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Queda ocultar al player para la animación de DeadLeft.(SetActive false no funciona)
public class CollL : MonoBehaviour
{
    public UIController uiController;
    public GameObject Planta;
    public Animator animator;
    public GameObject Player;

    public bool canAttackL = false;

    void Start()
    {
        animator = Planta.GetComponent<Animator>();
        animator.SetBool("IdleAgain", false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            canAttackL = true;

            if (uiController.lifes <= 1)
            {
                animator.SetBool("AttackLeft", false);
                animator.SetTrigger("DeadLeft");
                animator.SetBool("IdleAgain", true);
            }
            else
            {
                animator.SetBool("AttackLeft", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            canAttackL = false;
            animator.SetBool("AttackLeft", false);
            animator.SetBool("IdleAgain", true);
        }
    }
}
