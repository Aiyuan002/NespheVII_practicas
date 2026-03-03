using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class LagartoBehaviour : MonoBehaviour
{
    [Header("Lagarto Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    private float blinkTimer;
    public float blinkTime;
    private float immuneTimer;
    public float immuneTime;
    public float moveSpeed;
    public float attackTime;
    private float attackTimer;
    public float damageTime;
    public GameObject recompensa;
    private SpriteRenderer sr;
    public float followDistance;
    public float walking;
    Vector3 prePosition;
    Vector3 velocity;
    public bool stop = false;

    [Header("Animators")]
    public Animator lagartoAnimator;

    [Header("Jump")]
    public float jumpWallForce;
    public bool isWall;

    [Header("IA")]
    public float playerStopDistance;
    public Transform playerTransform;
    private Rigidbody2D rb;
    private RobotWallChecker childrenWallC;
    private RobotGroundChecker childrenGroundC;

    [Header("UI")]
    public UIController uiController;

    public bool hasObjective;
    public bool lookRight;
    public bool canAttack;
    public bool isImmune;

    public bool canJump = true;
    public bool canFollow = false;
    public bool noGround;
    public bool isAttacking;
    public bool isAttack = false;

    [Header("CAMBIOS")]
    public GameObject player;
    public bool colliderAtk = false;
    public GameObject gOAtk;
    public GameObject checkPlayer;

    PushPlayerCheck pushScript;
    AttackPoint atScript;
    bool death = false;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float detectionDistance = 1f;
    private bool isPatrolling = true;
    private float stopTimer = 0f;
    public float stopTime = 2f;

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
        prePosition = transform.position;
        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        childrenGroundC = GetComponentInChildren<RobotGroundChecker>();
        rb = GetComponent<Rigidbody2D>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
        sr = GetComponent<SpriteRenderer>();

        pushScript = checkPlayer.GetComponent<PushPlayerCheck>();
        atScript = gOAtk.GetComponent<AttackPoint>();
    }

    void Update()
    {
        if (death) return;

        HandleVelocity();
        if (health <= 0)
        {
            Death();
            return;
        }

        DetectPlayer();
        CheckPlayerDirection();

        if (hasObjective && !stop)
            FollowPlayer();
        else if (!isAttacking && !stop)
            Patrol();

        if (hasObjective && canAttack)
            Attack();

        if (isAttacking)
            rb.linearVelocity = Vector2.zero;

        if (isImmune)
            HandleDamageImmunity();

        if (childrenWallC.isWall && canJump && hasObjective)
            Jump();

        CheckGround();
    }

    void HandleVelocity()
    {
        Vector3 deltaPosition = transform.position - prePosition;
        velocity = deltaPosition / Time.deltaTime;
        prePosition = transform.position;
    }

    void Death()
    {
        death = true;
        isPatrolling = false;
        lagartoAnimator.SetTrigger("Dead");

        uiController.DisabledEnemyCanvas();
        StartCoroutine(SpawnRewardAfterDeath());
    }

    IEnumerator SpawnRewardAfterDeath()
    {
        float elapsed = 0f;
        while (elapsed < 0.9f)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkTime)
            {
                sr.enabled = !sr.enabled; // Alterna el sprite visible/invisible
                blinkTimer = 0f;
            }
            elapsed += Time.deltaTime; // Incrementa el tiempo total
            yield return null; // Espera al siguiente frame
        }

        sr.enabled = true;

        yield return new WaitForSeconds(0.6f);
        Instantiate(
            recompensa,
            new Vector2(transform.position.x, transform.position.y + 0.1f),
            Quaternion.identity
        );
        Destroy(gameObject);
    }

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

    void Jump()
    {
        canJump = false;

        if (isWall)
        {
            Debug.Log("entra???");
            lagartoAnimator.SetTrigger("Jump");

            rb.AddForce(new Vector2(0, jumpWallForce), ForceMode2D.Impulse);
            isWall = false;
            canAttack = false;
        }
    }

    void Stop()
    {
        stop = true;
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(playerTransform.position.x, transform.position.y),
            0 * Time.deltaTime
        );
        hasObjective = false;
        isAttacking = true;
    }

    void Continue()
    {
        stop = false;
        hasObjective = true;
        isAttacking = false;
    }

    void Attack()
    {
        if (noGround || death || isAttack || lagartoAnimator.GetCurrentAnimatorStateInfo(0).IsName("Lagarto_Attack"))
            return;

        isAttack = true;
        lagartoAnimator.SetTrigger("Attack");

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackTime)
        {
            GetDamagePlayer();
            attackTimer = 0;
            isAttack = false;
        }
    }

    void Patrol()
    {
        if (isPatrolling)
        {
            stopTimer += Time.deltaTime;

            if (lookRight)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(transform.position.x + detectionDistance, transform.position.y),
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(transform.position.x - detectionDistance, transform.position.y),
                    moveSpeed * Time.deltaTime
                );
            }
            lagartoAnimator.SetBool("Walk", true);
            if (stopTimer >= stopTime)
            {
                ChangeDirection();
                stopTimer = 0f;
            }
            if (childrenWallC.isWall || !childrenGroundC.isGrounded)
            {
                ChangeDirection();
                stopTimer = 0f;
            }
        }
    }

    IEnumerator jumpAttack()
    {
        yield return new WaitForSeconds(0.4f);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(0, 0.7f), ForceMode2D.Impulse);
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Debug.Log(distanceToPlayer);
        if (distanceToPlayer > playerStopDistance && isAttacking)
        {
            isAttacking = false; // Dejar de atacar
            canAttack = false; // No puede atacar mientras persigue
        }
        if (distanceToPlayer > playerStopDistance && !isAttacking)
        {
            canAttack = false;
            //if (!noGround)
            //{
            //   // isAttacking = false;
            //}
            canJump = true;
            lagartoAnimator.SetBool("Walk", true);
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(playerTransform.position.x, transform.position.y),
                moveSpeed * Time.deltaTime
            );
        }
        else if (distanceToPlayer <= playerStopDistance) // Si está en rango de ataque
        {
            lagartoAnimator.SetBool("Walk", false);
            canAttack = true;
            isAttacking = true;
        }
    }

    public void ChangeDirection()
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

    void GetDamage(int dmg)
    {
        health -= dmg;

        if (health < 0 && !death)
        {
            Death();
            return;
        }

        hasObjective = false;
        if (health > 0)
        {
            lagartoAnimator.SetTrigger("Hurt");
            isImmune = true;
            immuneTimer = 0;
            sr.enabled = false;
            blinkTimer = 0;
        }
    }

    //Maneja la inmunidad del enemigo al recibir daño.
    void HandleDamageImmunity()
    {
        immuneTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkTime)
        {
            ToggleSpriteVisibility();
            blinkTimer = 0;
        }

        if (immuneTimer >= immuneTime)
        {
            sr.enabled = true;
            isImmune = false;
        }
    }

    void ToggleSpriteVisibility()
    {
        sr.enabled = !sr.enabled;
    }

    void DetectPlayer()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < followDistance)
        {
            hasObjective = true;
            isPatrolling = false;
        }
        else // Fuera del rango
        {
            hasObjective = false;
            isPatrolling = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            //CAMBIARLO A UNA FUNCION ASI NO RENTA
            /* hasObjective = true;
             isPatrolling = false;*/
        }

        if (collision.transform.tag == "Projectile" && health > 0)
        {
            var projectile = collision.GetComponent<Projectile>();

            int dmg = projectile.damage;

            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            if (!isImmune)
            {
                GetDamage(dmg);
            }
            Destroy(collision.gameObject);
        }

        if (collision.transform.tag == "Attack" && health > 0)
        {
            int dmg = collision.gameObject.GetComponentInParent<CharacterController>().attackDamage;

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
            lagartoAnimator.SetBool("Walk", false);
            hasObjective = false;
            ChangeDirection();
            isPatrolling = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (pushScript.pushPlayer)
            {
                PushPlayer();
            }
        }
    }

    private void PushPlayer()
    {
        if (!death)
        {
            Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();

            Vector2 pushDirection = (player.transform.position - transform.position).normalized;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            float adjustedForce = 0.5f / (distance + 0.5f);

            rbPlayer.AddForce(pushDirection * adjustedForce, ForceMode2D.Impulse);
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

        Debug.Log(atScript.attack);
        CharacterController ccPlayer = player.GetComponent<CharacterController>();
        if (ccPlayer.isImmune)
            return;
        if (atScript.attack == true)
        {
            SpriteRenderer[] srPlayer;

            srPlayer = player.GetComponentsInChildren<SpriteRenderer>();
            uiController.ConsumeHealth();

            if (uiController.lifes <= 0)
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
                for (int i = 0; i < srPlayer.Length; i++)
                {
                    srPlayer[i].enabled = false;
                }
                ccPlayer.blinkTimer = 0;
                Debug.Log("daño");
            }
        }
    }

    public void PositionUp()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + walking);
    }

    public void PositionDown()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - walking);
    }

    //_______________________________________________________________
}