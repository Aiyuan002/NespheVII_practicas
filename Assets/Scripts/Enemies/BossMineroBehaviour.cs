using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMineroBehaviour : MonoBehaviour
{
    [Header("Boss Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float aimTime;
    private float aimingTimer;
    public float refreshTime;
    private float refreshTimer;
    private float blinkTimer;
    public float blinkTime;
    private float immuneTimer;
    public float immuneTime;

    private SpriteRenderer sr;

    [Header("Animators")]
    public Animator mineroAnimator;

    [Header("Sprites")]
    public GameObject explosion;

    [Header("IA")]
    public Transform playerTransform;

    [Header("UI")]
    public UIController uiController;

    [Header("Shoot")]
    public Transform shootPositionDown;
    public Transform shootPositionMiddle;
    public Transform shootPositionUp;
    public GameObject bullet;
    public bool isShootingUp;
    public bool isShootingMiddle;

    public bool isImmune;
    public bool hasObjective;
    public bool lookRight;
    public bool canShoot;

    void Start()
    {
        if (transform.rotation.y == 0) { lookRight = true; }
        else { lookRight = false; }

        sr = GetComponent<SpriteRenderer>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    void Update()
    {
        if (hasObjective && !canShoot) { Refresh(); }
        else if (canShoot) { Aim(); }

        if (isImmune) { DamageBlink(); }
    }

    void Shoot()
    {
        if(isShootingUp)
        {
            Instantiate(bullet, shootPositionUp.position, shootPositionUp.rotation);
        }
        else if (isShootingMiddle)
        {
            Instantiate(bullet, shootPositionMiddle.position, shootPositionMiddle.rotation);
        }
        else
        {
            Instantiate(bullet, shootPositionDown.position, shootPositionDown.rotation);
        }
    }

    void Aim()
    {
        aimingTimer += Time.deltaTime;
        if (aimingTimer >= aimTime)
        {
            aimingTimer = 0;
            Shoot();
            canShoot = false;
        }

    }

    //Rota para mirar hacia la otra dirección.
    void ChangeDirection()
    {
        if (lookRight)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            lookRight = false;
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            lookRight = true;
        }
    }

    void Refresh()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshTime)
        {
            refreshTimer = 0;
            canShoot = true;
        }
    }

    //Daña al robot al recibir disparos.
    void GetDamage(int dmg)
    {
        health -= dmg;
        //Añadir animación de daño

        if (health <= 0)
        {
            GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
            Destroy(effect, 0.5f);
            uiController.DisabledEnemyCanvas();
        }
        else
        {
            isImmune = true;
            immuneTimer = 0;
            sr.enabled = false;
            blinkTimer = 0;
        }
    }

    //Si recibe daño, parpadea
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
        if (collision.transform.tag == "Player")
        {
            hasObjective = true;
            mineroAnimator.SetBool("Attack", true);
        }

        if (collision.transform.tag == "Projectile")
        {
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            if (!isImmune) { GetDamage(dmg); }
            Destroy(collision.gameObject);   
        }

        if (collision.transform.tag == "Attack")
        {
            var dmg = collision.gameObject.GetComponentInParent<CharacterController>().attackDamage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            if (!isImmune) { GetDamage(dmg); }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            hasObjective = false;
            mineroAnimator.SetBool("Attack", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.transform.position.x > transform.position.x)
            {
                if (!lookRight)
                {
                    ChangeDirection();
                }
            }
            else
            {
                if (lookRight)
                {
                    ChangeDirection();
                }
            }

            if (collision.transform.position.y > transform.position.y+0.5)
            {
                mineroAnimator.SetBool("AimUp", true);
                mineroAnimator.SetBool("AimMiddle", true);
                isShootingUp = true;
                isShootingMiddle = false;
            }
            else if (collision.transform.position.y < transform.position.y-0.5)
            {
                mineroAnimator.SetBool("AimUp", false);
                mineroAnimator.SetBool("AimMiddle", false);
                isShootingMiddle = false;
            }
            else
            {
                mineroAnimator.SetBool("AimUp", false);
                mineroAnimator.SetBool("AimMiddle", true);
                isShootingUp = false;
                isShootingMiddle = true;
            }
        }
    }
}
