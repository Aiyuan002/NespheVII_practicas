using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRobotBehaviour : MonoBehaviour
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

    [Header("IA")]
    public float followDistance;
    public float playerStopDistance;
    public Transform playerTransform;
    private RobotRoofChecker childrenRoofC;
    private RobotGroundChecker childrenGroundC;
    private RobotWallChecker childrenWallC;
    private Vector3 initialPosition;
    public GameObject player;

    [Header("UI")]
    public UIController uiController;

    [Header("Shoot")]
    public Transform shootPosition;
    public GameObject bullet;
    Projectile collision_;

    public bool isImmune;
    public bool hasObjective;
    public bool lookRight;
    public bool canShoot;
    public bool canPatrol = true;
    public bool noGround;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.eulerAngles.y == 0)
        {
            lookRight = true;
        }
        else
        {
            lookRight = false;
        }

        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        childrenGroundC = GetComponentInChildren<RobotGroundChecker>();
        childrenRoofC = GetComponentInChildren<RobotRoofChecker>();
        sr = GetComponent<SpriteRenderer>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
        collision_ = bullet.GetComponent<Projectile>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
        CheckPlayerDirection();
        if (!hasObjective && canPatrol)
        {
            Patrol();
        }
        else if (!hasObjective && !canPatrol)
        {
            GoInitialPosition();
        }

        if (hasObjective && !childrenRoofC.isRoof)
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
        CheckGround();
    }

    void Death()
    {
        GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
        Instantiate(recompensa, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(effect, 0.5f);
        uiController.DisabledEnemyCanvas();
    }

    void CheckGround()
    {
        if (childrenGroundC.isGrounded)
        {
            noGround = false;
        }
        else
        {
            noGround = true;
        }
    }

    //Disparo
    void Shoot()
    {
        Instantiate(bullet, shootPosition.position, shootPosition.rotation);
        Debug.Log(collision_.collision_);
        if (collision_.collision_)
        {
            Debug.Log("deberia");
            GameObject effect = Instantiate(
                explosion,
                new Vector2(transform.position.x, transform.position.y + 0.2f),
                transform.rotation
            );
        }
    }

    //Si pierde la visión del player, vuelve a su posicion inicial.
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
            childrenRoofC.isRoof = false;
        }
    }

    //Si tiene vision del player lo persigue.
    void FollowPlayer()
    {
        if (childrenWallC.isWall)
        {
            canPatrol = true;
            hasObjective = false;
            return;
        }
        canPatrol = false;
        if (Vector2.Distance(transform.position, playerTransform.position) > playerStopDistance)
        {
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
            ChangeDirection();
            patrolTimer = 0f;
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

    void GetDamage(int dmg)
    {
        health -= dmg;
        //Añadir animación de daño

        if (health <= 0)
        {
            Death();
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

    void DetectPlayer()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < followDistance)
        {
            hasObjective = true;
            canPatrol = false;
        }
        else // Fuera del rango
        {
            hasObjective = false;
            canPatrol = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            hasObjective = false;
            ChangeDirection();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //PONER PUSH
        if (collision.transform.tag == "Player")
        {
            //   PushPlayer();
        }
    }

    private void PushPlayer()
    {
        Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();

        Vector2 pushDirection = (player.transform.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        float adjustedForce = 0.5f / (distance + 0.5f);

        rbPlayer.AddForce(pushDirection * adjustedForce, ForceMode2D.Impulse);
    }

    private void CheckPlayerDirection()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= followDistance)
        {
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
    }
}
