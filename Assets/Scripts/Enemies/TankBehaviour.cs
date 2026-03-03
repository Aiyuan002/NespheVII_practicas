using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehaviour : MonoBehaviour
{
    [Header("Tank Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float aimTime;
    private float aimingTimer;
    public float aimDelay;
    public float refreshTime;
    private float refreshTimer;
    private float blinkTimer;
    public float blinkTime;
    private float immuneTimer;
    public float immuneTime;

    private SpriteRenderer sr;

    [Header("Sprites")]
    public GameObject explosion;

    [Header("UI")]
    public UIController uiController;

    [Header("Shoot")]
    //private Transform shootPosition;
    public GameObject tankCanyon;
    public GameObject turretCanyon;
    public float canyonSpeed;
    public float turretCanyonSpeed;
    public Transform target;
    public Transform canyonPostion;
    public Transform turretCanyonPosition;
    public GameObject bullet;

    public bool isImmune;
    public bool hasObjective;
    public bool canShoot = true;
    public bool isFar = true;
    public bool turretDamage;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasObjective && !canShoot) { Refresh(); }
        else if (canShoot) { Aim(); }

        if (isImmune) { DamageBlink(); }

        if (isFar) { MoveCanyon(tankCanyon, canyonSpeed); }
        if (!isFar && !turretDamage) {  MoveCanyon(turretCanyon, turretCanyonSpeed); }

        CheckObjective();
    }

    void CheckObjective()
    {
        if (Vector2.Distance(transform.position, target.position) < 5)
        {
            hasObjective = true;
            if (Vector2.Distance(transform.position, target.position) < 3)
            {
                isFar = false;
            }
            else
            {
                isFar = true;
            }
        }
        else
        {
            hasObjective = false;
        }
    }

    private void MoveCanyon(GameObject canyon, float speed)
    {
        Vector3 direction = canyon.transform.position - target.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float an = Mathf.Clamp(angle, -10, 10);

        Quaternion rotation = Quaternion.AngleAxis(an, Vector3.forward);
        canyon.transform.rotation = Quaternion.Slerp(canyon.transform.rotation, rotation, speed * Time.deltaTime);
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

    //Delay para reiniciar el funcionamiento del robot.
    void Refresh()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshTime)
        {
            refreshTimer = 0;
            canShoot = true;
        }
    }

    void Shoot()
    {
        if (isFar)
        {
            Instantiate(bullet, canyonPostion.position, canyonPostion.rotation);
        }
        else if (!turretDamage)
        {
            Instantiate(bullet, turretCanyonPosition.position, turretCanyonPosition.rotation);
        } 
    }

    //Daña al tanque al recibir disparos.
    void GetDamage(int dmg)
    {
        health -= dmg;
        aimTime += aimDelay;
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
            if (health <= maxHealth/2)
            {
                turretDamage = true;
            }

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
    {/*
        if (collision.transform.tag == "Player")
        {
            hasObjective = true;
        }*/

        if (collision.transform.tag == "Projectile")
        {
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            if (!isImmune) { GetDamage(dmg); }
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {/*
        if (collision.transform.tag == "Player")
        {
            hasObjective = false;
        }*/
    }
}
