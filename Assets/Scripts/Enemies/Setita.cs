using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setita : MonoBehaviour
{
    [Header("Seta Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float moveSpeed;
    public float patrolSpeed;
    public float patrolTime;
    private float patrolTimer;
    public float aimTime;
    private float aimingTimer;
    public float refreshTime;
    private float refreshTimer;
    private float blinkTimer;
    public float blinkTime;
    private float immuneTimer;
    public float immuneTime;
    public GameObject recompensa;
    private Animator anim;

    private Transform playerTransform;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    [Header("Sprites")]
    //public GameObject explosion;

    [Header("UI")]
    public UIController uiController;

    [Header("Shoot")]
    public Transform shootPosition;
    public GameObject overlapPos;
    public GameObject bullet;
    public LayerMask overlapLayer;

    public bool isImmune;
    public bool attacked;
    public bool hasObjective;
    public bool lookRight;
    public bool canShoot;
    public bool canPatrol = true;

    private EntraZona entraZona;
    public GameObject zona;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        entraZona = GetComponentInChildren<EntraZona>();

        uiController = GameObject.Find("UI").GetComponent<UIController>();

        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(Disparar());
    }

    void Update()
    {
        Overlap();
        if (entraZona.enZona)
        {
            anim.SetBool("Walking", true);
            transform.position = Vector2.MoveTowards(transform.position,
            new Vector2(entraZona.player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
        }

        else if (!entraZona.enZona)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Walking", false);
        }

        if (isImmune) { DamageBlink(); }
    }

    void Overlap()
    {
        Collider2D overlap = Physics2D.OverlapCircle(overlapPos.transform.position, 0.2f, overlapLayer);
        if (overlap != null)
        {
            if (overlap.CompareTag("Player") && !attacked)
            {
                Debug.Log("disparo");
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("Humo");
                attacked = true;
                //Instantiate(bullet, shootPosition.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Projectile")
        {
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            //DamageBlink();
            if (!isImmune) { GetDamage(dmg); }
            //Destroy(collision.gameObject);
        }

        if (collision.transform.tag == "Attack")
        {
            var dmg = collision.gameObject.GetComponentInParent<CharacterController>().attackDamage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            //DamageBlink();
            if (!isImmune) { GetDamage(dmg); }
        }
    }

    public void Shoot()
    {
        Instantiate(bullet, shootPosition.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(overlapPos.transform.position, 0.2f);
    }

    void GetDamage(int dmg)
    {
        health -= dmg;
        //Añadir animación de daño

        if (health <= 0)
        {
            Instantiate(recompensa, transform.position, Quaternion.identity);
            Destroy(gameObject);
            uiController.DisabledEnemyCanvas();
        }
        else
        {
            isImmune = true;
            immuneTimer = 0;
            //sr.enabled = false;
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

    public IEnumerator Disparar()
    {
        while (true)
        {
            if (attacked)
            {
                zona.SetActive(false);
                yield return new WaitForSeconds(3f);
                attacked = false;
            }
            else
            {
                yield return new WaitForEndOfFrame();
                zona.SetActive(true);
            }
        }
    }
}
