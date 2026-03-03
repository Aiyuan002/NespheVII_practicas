using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumRobotBehaviour : MonoBehaviour
{
    [Header("Robot Attributes")]
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

    private SpriteRenderer sr;

    [Header("Sprites")]
    public GameObject explosion;

    [Header("Jump")]
    public float jumpWallForce;
    public float jumpCliffForce;
    public bool isWall;
    public bool isCliff;

    [Header("IA")]
    public float followDistance;
    public float followDistanceVertical;
    public float playerStopDistance;
    public GameObject player;
    public Transform playerTransform;
    private Rigidbody2D rb;
    private Vector3 initialPosition;
    private RobotWallChecker childrenWallC;
    private RobotGroundChecker childrenGroundC;

    [Header("UI")]
    public UIController uiController;

    [Header("Shoot")]
    public Transform shootPosition;
    public GameObject bullet;

    public bool isImmune;
    public bool hasObjective;
    public bool lookRight;
    public bool canShoot;
    public bool canPatrol = true;
    public bool canJump = true;
    public bool noGround;

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

        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        childrenGroundC = GetComponentInChildren<RobotGroundChecker>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        uiController = GameObject.FindFirstObjectByType<UIController>();

        initialPosition = transform.position;
    }

    void Update()
    {
        DetectPlayer();

        if (!hasObjective && canPatrol)
        {
            Patrol();
        }
        else if (!hasObjective && !canPatrol)
        {
            GoInitialPosition();
        }

        if (hasObjective)
        {
            FollowPlayer();
        }

        if (hasObjective && !canShoot)
        {
            Refresh();
        }
        else if (canShoot)
        {
            Aim();
        }

        if (isImmune)
        {
            DamageBlink();
        }

        if (childrenWallC.isWall && canJump)
        {
            isWall = true;
            if (childrenGroundC.isGrounded) // Solo salta si está tocando el suelo
            {
                Jump();
            }
        }

        if (noGround && canJump)
        {
            isCliff = true;
            if (childrenGroundC.isGrounded) // Solo permite saltar si hay suelo
            {
                Jump();
            }
        }
        else
        {
            CheckGround();
        }
    }

    //Comprobar si toca el suelo
    void CheckGround()
    {
        if (childrenGroundC.isGrounded)
        {
            canJump = true;
            noGround = false;
        }
        else
        {
            noGround = true;
        }
    }

    void DetectPlayer()
    {
        float distX = Mathf.Abs(transform.position.x - playerTransform.position.x);
        float distY = Mathf.Abs(transform.position.y - playerTransform.position.y);

        // Se inicializa una sola vez para evitar cambios aleatorios en cada frame
        if (followDistanceVertical <= 0)
        {
            followDistanceVertical = 1.5f; // Un valor predeterminado
        }

        if (distX < followDistance && distY <= followDistanceVertical)
        {
            CheckPlayerDirection();
            hasObjective = true;
            canPatrol = false;
        }
        else
        {
            hasObjective = false;
            canPatrol = true;
        }
    }

    private void CheckPlayerDirection()
    {
        Debug.Log("en la player + " + playerTransform.position.x);
        Debug.Log("en la enemigo + " + transform.position.x);
        if (playerTransform.position.x > transform.position.x)
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
    }

    void ChangeDirection()
    {
        if (lookRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            lookRight = false;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            lookRight = true;
        }
    }

    //Disparo
    void Shoot()
    {
        Instantiate(bullet, shootPosition.position, shootPosition.rotation);
    }

    void Jump()
    {
        if (!childrenGroundC.isGrounded)
            return;

        canJump = false;

        if (isWall)
        {
            rb.AddForce(new Vector2(0, jumpWallForce), ForceMode2D.Impulse);
            isWall = false;
        }

        if (isCliff)
        {
            rb.AddForce(new Vector2(0, jumpCliffForce), ForceMode2D.Impulse);
            isCliff = false;
        }
    }

    //Si pierde la visión del player vuelve a su posicion inicial.
    void GoInitialPosition()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            initialPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, initialPosition) < 0.1)
        {
            canPatrol = true;
            //childrenRoofC.isRoof = false;
        }
    }

    //Si tiene vision del player lo persigue.
    void FollowPlayer()
    {
        canPatrol = false;
        if (Vector2.Distance(transform.position, playerTransform.position) > playerStopDistance)
        {
            if (!childrenGroundC.isGrounded)
            {
                ChangeDirection();

                return; // Salir sin moverse
            }
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(playerTransform.position.x, transform.position.y),
                moveSpeed * Time.deltaTime
            );
        }
    }

    //Si no tiene vision del player patrulla.
    void Patrol()
    {
        if (lookRight)
        {
            transform.Translate(Vector2.right * patrolSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector2.left * patrolSpeed * Time.deltaTime, Space.World);
        }

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolTime)
        {
            patrolTimer = 0;
            ChangeDirection();
        }
        if (childrenWallC.isWall || !childrenGroundC.isGrounded)
        {
            patrolTimer = 0f;
            ChangeDirection();
        }
    }

    //Si tiene vision del player apunta durante un tiempo y despues dispara.
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

    //Daña al robot al recibir disparos.
    void GetDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
            Instantiate(recompensa, transform.position, Quaternion.identity);
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
                Debug.Log("aqui entra verdad");
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
            hasObjective = false;
            if (
                (lookRight && playerTransform.position.x < transform.position.x)
                || (!lookRight && playerTransform.position.x > transform.position.x)
            )
            {
                ChangeDirection();
            }
        }
    }

    private void PushPlayer()
    {
        Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();

        Vector2 pushDirection = (player.transform.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        float adjustedForce = 0.1f / (distance + 0.5f);

        rbPlayer.AddForce(pushDirection * adjustedForce, ForceMode2D.Impulse);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PushPlayer();
        }
    }
}
