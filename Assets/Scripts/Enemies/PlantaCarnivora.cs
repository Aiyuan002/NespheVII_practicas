using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantaCarnivora : MonoBehaviour
{
    [Header("Attack Attributes")]
    public GameObject emptyR;
    public GameObject emptyL;

    public Animator animator;
    public GameObject player;

    public UIController uiController;

    public CollL collL;
    public CollR collR;

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

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        collL = emptyL.GetComponent<CollL>();
        collR = emptyR.GetComponent<CollR>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isImmune)
        {
            DamageBlink();
        }

        GetDamagePlayer();
    }

    public void GetDamagePlayer()
    {
        if (collR.canAttackR || collL.canAttackL)
        {
            player.SetActive(true);
            Debug.Log("Daño al player");
            SpriteRenderer[] srPlayer;
            CharacterController ccPlayer;
            ccPlayer = player.GetComponent<CharacterController>();
            srPlayer = player.GetComponentsInChildren<SpriteRenderer>();
            uiController.ConsumeHealth();
            if (uiController.lifes <= 0 && uiController.currentHealth <= 0)
            {
                Debug.Log("Me mata");
                collR.canAttackR = false;
                collL.canAttackL = false;
                Destroy(collR);
                Destroy(collL);
                Destroy(player);
                //player.SetActive(false);
                //StartCoroutine(DesaparecePlayer());
            }
        }
    }

    IEnumerator DesaparecePlayer()
    {
        yield return new WaitForSeconds(3);
        // player.SetActive(true);
    }

    void GetDamage(int dmg)
    {
        health -= dmg;
        //Añadir animación de daño

        if (health < 0)
        {
            Instantiate(recompensa, transform.position, Quaternion.identity);
            Destroy(gameObject);
            uiController.DisabledEnemyCanvas();
        }
        else if (health > 0)
        {
            isImmune = true;
            immuneTimer = 0;
            sr.enabled = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Projectile")
        {
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            //DamageBlink();
            if (!isImmune)
            {
                GetDamage(dmg);
            }
            Destroy(collision.gameObject);
        }

        if (collision.transform.tag == "Attack")
        {
            var dmg = collision.gameObject.GetComponentInParent<CharacterController>().attackDamage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            //DamageBlink();
            if (!isImmune)
            {
                GetDamage(dmg);
            }
        }
    }
}
