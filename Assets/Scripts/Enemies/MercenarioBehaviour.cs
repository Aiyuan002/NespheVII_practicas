using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenarioBehaviour : MonoBehaviour
{
    [Header("Mercenario Attributes")]
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

    public bool isFollow;

    private SpriteRenderer sr;

    [Header("Animators")]
    public Animator mercenarioAnimator;

    [Header("IA")]
    public float followDistance;
    public float followDistanceVertical;
    public float playerStopDistance;
    public GameObject playerTransform;
    private RobotWallChecker childrenWallC;
    public bool isWaiting = false;
    private Rigidbody2D rb;

    private float distanceToPlayer;

    [Header("UI")]
    public UIController uiController;

    [Header("Shoot")]
    public Transform shootPosition;
    public Transform shootPositionDown;
    public GameObject bullet;

    public bool isImmune;
    public bool hasObjective;
    public bool canPatrol = true;
    public bool lookRight;
    public bool canShoot;

    public float checkInterval = 0.1f;
    private float nextCheckTime;

    private bool wallDetected = false;

    private int changeAttack;

    void Start()
    {
        if (transform.rotation.y == 0)
        {
            lookRight = true;
        }
        else
        {
            lookRight = false;
        }
        rb = GetComponent<Rigidbody2D>();
        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        sr = GetComponent<SpriteRenderer>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            distanceToPlayer = 40f;
        }
        else
        {
            distanceToPlayer = Vector2.Distance(
                transform.position,
                playerTransform.transform.position
            );
        }
        mercenarioAnimator.SetFloat("Distancia", distanceToPlayer);
        mercenarioAnimator.SetBool("Siguiendo", isFollow);
        mercenarioAnimator.SetBool("Muro", wallDetected);
        mercenarioAnimator.SetInteger("Cambiar", changeAttack);

        if (Time.time >= nextCheckTime)
        {
            DetectedWalls();
            nextCheckTime = Time.time + checkInterval;
        }
        DamageBlink();
    }

    void DetectedWalls()
    {
        RaycastHit2D hit = Physics2D.Linecast(
            transform.position,
            playerTransform.transform.position,
            LayerMask.GetMask("Default")
        );
        if (hit.collider != null)
        {
            if (hit.transform.tag == "Ground")
            {
                wallDetected = true;
            }
            else
            {
                wallDetected = false;
            }
        }
        else
        {
            wallDetected = false;
        }
    }

    void Shoot()
    {
        changeAttack = Random.Range(1, 10);
        Instantiate(bullet, shootPosition.position, shootPosition.rotation);
    }

    void ShootDown()
    {
        changeAttack = Random.Range(1, 10);
        Instantiate(bullet, shootPositionDown.position, shootPositionDown.rotation);
    }

    public void ChangeDirectionPatrol()
    {
        if (lookRight)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            lookRight = false;
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            lookRight = true;
        }
    }

    public void ChangeDirection(Vector3 player)
    {
        if (transform.position.x < player.x)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            lookRight = false;
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            lookRight = true;
        }
    }

    void GetDamage(int dmg)
    {
        health -= dmg;
        //Añadir animación de daño

        if (health <= 0)
        {
            mercenarioAnimator.SetBool("Muerte", true);
            Destroy(gameObject, 2.0f);
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
            mercenarioAnimator.SetBool("Shoot", true);
        }

        if (collision.transform.tag == "Projectile")
        {
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
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
            if (!isImmune)
            {
                StartCoroutine(WaitToAnim(dmg));
            }
        }
    }

    private IEnumerator WaitToAnim(int damage)
    {
        yield return new WaitForSeconds(.5f);
        GetDamage(damage);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            mercenarioAnimator.SetBool("Shoot", false);
        }
    }
}
