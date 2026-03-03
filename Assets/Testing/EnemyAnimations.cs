using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    public Animator anim;
    public GameObject helmet;
    public Transform headPosition;

    public void IdleButton()
    {
        anim.Play("Idle");
        anim.SetBool("Alert", false);
    }

    public void ShootButton()
    {
        anim.Play("Shoot");
    }

    public void AlertButton()
    {
        anim.SetBool("Alert", !anim.GetBool("Alert"));
    }

    public void DieButton()
    {
        anim.SetTrigger("Dead");
    }

    public void DeathEvent()
    {
        Instantiate(helmet, headPosition.position, helmet.transform.rotation);
        Destroy(this.gameObject);
    }
}
