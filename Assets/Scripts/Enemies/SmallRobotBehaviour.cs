using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class SmallRobotBehaviour : MonoBehaviour
{
    [Header("Robot Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float moveSpeed;
    public float patrolSpeed;
    public float patrolTime;
    private float patrolTimer;
    private float blinkTimer;
    public float blinkTime;
    private float immuneTimer;
    public float immuneTime;
    public GameObject recompensa;
    public float followDistance;
    private SpriteRenderer sr;
    public bool canAttack;
    public bool isAttacking;
    public bool canJump;
    public float attackTime;
    private float attackTimer;

    private float verticalVelocity;
    public float gravityScale = 9.8f;

    [Header("Sprites")]
    public GameObject explosion;

    [Header("IA")]
    public float playerStopDistance;
    private RobotGroundChecker childrenGroundC;
    private RobotWallChecker childrenWallC;
    private Rigidbody2D rb;
    public Transform playerTransform;
    private bool isAttemptingJump = false;
    private float stuckTimer = 0f;
    private bool isCooldown = false;
    private float cooldownTimer = 0f;
    private float lastDistance;
    private bool wallDetected;

    [Header("UI")]
    public UIController uiController;

    public bool isImmune;
    public bool hasObjective;
    public bool lookRight;
    public bool canPatrol = true;
    public bool canClimb;
    public bool noGround;
    public bool isAttack = false;

    [Header("CAMBIOS")]
    public GameObject player;

    [Header("Patrol")]
    public float detectionDistance = 1f;
    private bool isPatrolling = true;
    private float stopTimer = 0f;
    public float stopTime = 2f;

    [Header("Jump")]
    public float jumpWallForce;
    public bool isWall;

    [Header("Cronometro")]
    public GameObject cuentaAtras;
    private float timeCronometro = 5;
    private float timerCronometro;
    SpriteRenderer srCuentaAtras;

    [Header("Trigger")]
    AttackPoint scriptAttack;

    void Start()
    {
        if (transform.rotation.y == 0)
        {
            lookRight = false;
        }
        else
        {
            lookRight = true;
        }
        scriptAttack = gameObject.GetComponentInChildren<AttackPoint>();
        srCuentaAtras = cuentaAtras.GetComponent<SpriteRenderer>();
        childrenGroundC = GetComponentInChildren<RobotGroundChecker>();
        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    void Update()
    {
        DetectPlayer();
        CheckPlayerDirection();
        ApplyGravity();
        if (hasObjective && !isCooldown)
        {
            FollowPlayer();
        }
        else if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= 1f)
            {
                isCooldown = false;
                cooldownTimer = 0f;
                hasObjective = true; // Reanudar seguimiento
                canPatrol = true;
                isPatrolling = true;
            }
        }
        else if (!isAttacking)
        {
            Patrol();
        }

        if (hasObjective && canAttack)
        {
            Attack();
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (isImmune)
        {
            DamageBlink();
        }

        if (childrenWallC.isWall && canJump && hasObjective)
        {
            isWall = true;
            Jump();
        }
        else
        {
            if (!hasObjective)
                CheckGround();
        }
    }

    void CheckGround()
    {
        if (childrenGroundC.isGrounded)
        {
            canJump = true;
            noGround = false;
            isAttemptingJump = false;
        }
        else
        {
            noGround = true;
        }
    }

    void Jump()
    {
        if (!canJump)
            return; // Evitar doble salto

        canJump = false;

        if (childrenWallC.isWall)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Solo reiniciar velocidad vertical
            rb.AddForce(new Vector2(0, jumpWallForce), ForceMode2D.Impulse);
            childrenWallC.isWall = false;
            canAttack = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(new Vector2(0, 10f), ForceMode2D.Impulse);
        }
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float heightDifference = Mathf.Abs(transform.position.y - playerTransform.position.y);

        if (heightDifference > 1f)
        {
            isCooldown = true;
            hasObjective = false;
            canPatrol = true;
            isPatrolling = true;
            isAttemptingJump = false;
            stuckTimer = 0f;
            timerCronometro = 0;
            cuentaAtras.SetActive(false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isAttemptingJump)
        {
            stuckTimer += Time.deltaTime;
            if (distanceToPlayer < lastDistance)
            {
                stuckTimer = 0f;
                isAttemptingJump = false;
            }
            else if (stuckTimer >= 0.5f)
            {
                isCooldown = true;
                hasObjective = false;
                canPatrol = true;
                isAttemptingJump = false;
                stuckTimer = 0f;
            }
        }

        if (distanceToPlayer > playerStopDistance && isAttacking)
        {
            isAttacking = false;
            canAttack = false;
        }

        if (!isAttacking)
        {
            Cronometro();
            canJump = true;

            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }

        if (distanceToPlayer <= playerStopDistance)
        {
            canAttack = true;
            isAttacking = true;
        }

        lastDistance = distanceToPlayer;
    }

    void Cronometro()
    {
        cuentaAtras.SetActive(true);
        timerCronometro += Time.deltaTime;
        if (timerCronometro > timeCronometro)
        {
            canAttack = true;
            isAttacking = true;
        }
    }

    //Si no tiene vision del player patrulla.
    void Patrol()
    {
        if (isPatrolling)
        {
            stopTimer += Time.deltaTime;
            float direction = lookRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);

            if (stopTimer >= stopTime)
            {
                ChangeDirection();
                stopTimer = 0f;
            }

            if (childrenWallC.isWall || !childrenGroundC.isGrounded)
            {
                stopTimer = 0f;
                ChangeDirection();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    //Rota para mirar hacia la derecha o la izquierda.
    public void ChangeDirection()
    {
        //ESTA INVERTIDO PERO FUNCIONA PERFECTAMENTE
        if (lookRight)
        {
            srCuentaAtras.flipX = true;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            lookRight = false;
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            srCuentaAtras.flipX = false;
            lookRight = true;
        }
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float heightDifference = Mathf.Abs(transform.position.y - playerTransform.position.y);

        if (distanceToPlayer < followDistance && heightDifference < 2f && !childrenWallC.isWall)
        {
            hasObjective = true;
            isPatrolling = false;
        }
    }

    //Daña al robot al recibir disparos.
    void GetDamage(int dmg)
    {
        health -= dmg;
        //Añadir animación de daño

        if (health <= 0)
        {
            GameObject effect = Instantiate(
                explosion,
                new Vector2(transform.position.x, transform.position.y + 0.2f),
                transform.rotation
            );

            Instantiate(
                recompensa,
                new Vector2(transform.position.x, transform.position.y + .2f),
                Quaternion.identity
            );
            uiController.DisabledEnemyCanvas();
            Destroy(gameObject, 0.3f);
            Destroy(effect, 0.5f);
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

    void Attack()
    {
        if (!noGround && !isAttack)
        {
            isAttack = true;
            attackTimer += Time.deltaTime;
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkTime)
            {
                sr.enabled = !sr.enabled;
                blinkTimer = 0;

                // Reduce el tiempo de parpadeo para hacerlo más rápido
                blinkTime = Mathf.Max(0.05f, blinkTime * 0.9f); // Evita que sea menor que 0.05s
            }

            if (attackTimer >= attackTime)
            {
                if (scriptAttack.attack)
                {
                    GetDamagePlayer();
                }

                // Crea la explosión

                GameObject effect = Instantiate(
                    explosion,
                    new Vector2(transform.position.x, transform.position.y + 0.2f),
                    transform.rotation
                );

                Instantiate(
                    recompensa,
                    new Vector2(transform.position.x, transform.position.y + .2f),
                    Quaternion.identity
                );

                // Destruye el robot
                Destroy(gameObject);
                Destroy(effect, 0.5f);

                uiController.DisabledEnemyCanvas();
                isAttack = false;
            }
        }
    }

    private void ApplyGravity()
    {
        if (!childrenGroundC.isGrounded)
        {
            rb.linearVelocity += Vector2.down * gravityScale * Time.fixedDeltaTime;
        }
        else if (verticalVelocity < 0)
        {
            // Resetear la velocidad vertical cuando toca el suelo
            verticalVelocity = -0.1f;
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
            canPatrol = true;
            hasObjective = false;
            isPatrolling = true;
            isAttemptingJump = false;
            ChangeDirection();
        }
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

    public void GetDamagePlayer()
    {
        isAttack = false;

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
            Debug.Log(isImmune);
            ccPlayer.isImmune = true;
            Debug.Log(isImmune);
            ccPlayer.immuneTimer = 0;

            ccPlayer.blinkTimer = 0;
            for (int i = 0; i < srPlayer.Length; i++)
            {
                srPlayer[i].enabled = false;
            }
            Debug.Log("daño");
        }
    }
}
