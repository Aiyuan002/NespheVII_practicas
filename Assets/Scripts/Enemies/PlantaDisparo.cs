using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantaDisparo : MonoBehaviour
{
    public GameObject projectile;
    public GameObject shootZone;
    public GameObject triggerObj;
    EntraTrigger trigger;
    public Animator animator;
    Collider2D triggerShoot;

    [Header("Plant Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    private float blinkTimer;
    public float blinkTime;
    private float immuneTimer;
    public float immuneTime;
    public GameObject recompensa;

    private SpriteRenderer sr;
    public bool isImmune;

    public UIController uiController;

    void Start()
    {
        trigger = triggerObj.GetComponent<EntraTrigger>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        triggerShoot = shootZone.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (isImmune)
        {
            DamageBlink();
        }

        animator.SetBool("EntraZona", trigger != null && trigger.entra);
    }

    public void DispararAzul()
    {
        Disparar(Quaternion.identity);
    }

    public void DispararRojo()
    {
        Disparar(Quaternion.Euler(0, 0, -180));
    }

    private void Disparar(Quaternion rotation)
    {
        if (projectile != null && shootZone != null)
        {
            Instantiate(projectile, shootZone.transform.position, rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (triggerShoot != null && !triggerShoot.IsTouching(collision)) return;

        if (collision.CompareTag("Projectile"))
        {
            Projectile p = collision.GetComponent<Projectile>();
            if (p != null)
            {
                ProcessHit(p.damage);
            }
        }
        else if (collision.CompareTag("Attack"))
        {
            CharacterController cc = collision.GetComponentInParent<CharacterController>();
            if (cc != null)
            {
                ProcessHit(cc.attackDamage);
            }
        }
    }

    private void ProcessHit(int damage)
    {
        if (isImmune) return;

        uiController?.EnabledEnemyCanvas(health, damage, maxHealth, gameObject.name, faceImage);
        GetDamage(damage);
    }

    void GetDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            if (recompensa != null)
                Instantiate(recompensa, transform.position, Quaternion.identity);

            uiController?.DisabledEnemyCanvas();
            Destroy(gameObject);
        }
        else
        {
            isImmune = true;
            immuneTimer = 0;
            blinkTimer = 0;
        }
    }

    void DamageBlink()
    {
        immuneTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkTime)
        {
            sr.enabled = !sr.enabled;
            blinkTimer = 0;
        }

        if (immuneTimer >= immuneTime)
        {
            sr.enabled = true;
            isImmune = false;
        }
    }
}