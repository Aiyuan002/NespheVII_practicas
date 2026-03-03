using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LodoAtaque : MonoBehaviour
{
    public GameObject triggerObj;
    EntraTrigger trigger;
    public Animator animator;
    Collider2D triggerShoot;
    public GameObject player;
    public Animator smoke;

    [Header("Mud Attributes")]
    public bool canAttack = false;
    public bool isAttack = false;

    [Header("Attack Cooldown")]
    public float attackCooldown; // Tiempo entre ataques
    private float attackTimer = 0f;

    [Header("UI")]
    public UIController uiController;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        trigger = triggerObj.GetComponent<EntraTrigger>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.entra)
        {
            animator.SetBool("EntraZona", true);
            smoke.Play("Humo");
            Attack();
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("LodoAtaque"))
        {
            animator.SetBool("EntraZona", false);
        }

        if (isAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                isAttack = false;
                attackTimer = 0f;
            }
        }
    }

    void Attack()
    {
        if (!isAttack)
        {
            isAttack = true;

            PushPlayer();
            GetDamagePlayer();
        }
    }

    private void PushPlayer()
    {
        Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();

        Vector2 pushDirection = (player.transform.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        float adjustedForce = 4f / (distance + 0.5f);

        rbPlayer.AddForce(pushDirection * adjustedForce, ForceMode2D.Impulse);
    }

    public void GetDamagePlayer()
    {
        if (trigger.entra)
        {
            SpriteRenderer[] srPlayer;
            CharacterController ccPlayer;
            ccPlayer = player.GetComponent<CharacterController>();
            srPlayer = player.GetComponentsInChildren<SpriteRenderer>();
            uiController.ConsumeHealth();

            if (uiController.lifes <= 0 && uiController.currentHealth <= 0)
            {
                Destroy(player);
            }
            else
            {
                uiController.ChangePlayerFace();

                ccPlayer.isImmune = true;

                ccPlayer.immuneTimer = 0;
                for (int i = 0; i < srPlayer.Length; i++)
                {
                    srPlayer[i].enabled = false;
                }
                ccPlayer.blinkTimer = 0;
                Debug.Log("daño");
            }
        }
    }

    void ExitZone()
    {
        trigger.entra = false;
    }
}
