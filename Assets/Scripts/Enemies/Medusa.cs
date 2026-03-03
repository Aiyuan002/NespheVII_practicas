using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medusa : MonoBehaviour
{
    [Header("Medusa Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float patrolTime;
    private float patrolTimer;
    public float aimTime;
    private float aimingTimer;
    public float meleeTime;
    private float meleeTimer;

    public Collider2D bodyCollider;
    public GameObject recompensa;
    public SpriteRenderer sr;
    private float immuneTimer;
    public float immuneTime;
    private float blinkTimer;
    public float blinkTime;
    public float followDistance;

    private SpriteRenderer srPelota;
    private CircleCollider2D circleCollider;

    [Header("Animators")]
    public Animator medusaAnimator;

    [Header("AI")]
    private bool previousHasObjective = false;
    public GameObject player;
    public Transform playerTransform;
    private Vector3 initialPosition;
    private bool isReturningToInitialPosition = false;
    private RobotWallChecker childrenWallC;
    private RobotGroundChecker childrenGroundC;
    Rigidbody2D rb;
    bool death = false;
    float stuckTimer = 0f;
    float stuckThreshold = 1f;

    [Header("Movement (replaces NavMeshAgent)")]
    public float moveSpeed = 0.6f;
    public float patrolSpeed = 0.8f;
    private bool isStopped = false;
    private Vector3 lastPosition;
    private float stuckCheckTimer = 0f;

    [Header("UI")]
    public UIController uiController;
    public bool isImmune;

    [Header("Melee")]
    public GameObject gOAtk;
    AttackPoint atScript;
    private float actionCooldown = 1.5f;
    private float actionTimer = 0f;

    public bool hasObjective;
    public bool lookRight;
    public bool canShoot;
    public bool canPatrol;
    public bool canMelee;

    private bool yaImpulsado = false;
    private bool isCheckingDistance = false;
    private bool isExecutingAttack = false;
    private bool finishMelee = false;

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

        initialPosition = transform.position;
        lastPosition = transform.position;
        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        childrenGroundC = GetComponentInChildren<RobotGroundChecker>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        atScript = gOAtk.GetComponent<AttackPoint>();

        // Descongelar X para que el movimiento por velocidad funcione.
        // Mantener Y congelada (la medusa flota) y rotación congelada.
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        actionTimer += Time.deltaTime;

        if (health == 0 && !death)
        {
            Death();
            return;
        }
        if (!death)
        {
            if (childrenGroundC.isGrounded)
            {
                if (!yaImpulsado && transform.position.y < 10)
                {
                    rbForce();
                }

                if (transform.position.y < 0.6f)
                {
                    HeightMin();
                }
            }
            else
            {
                yaImpulsado = false;
            }

            if (hasObjective)
            {
                FollowPlayer();
            }

            if (actionTimer >= actionCooldown)
            {
                if (
                    !hasObjective
                    && canPatrol
                    && !isReturningToInitialPosition
                    && !isExecutingAttack
                )
                {
                    Patrol();
                }
                else if (
                    canMelee
                    && !medusaAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack")
                    && !finishMelee
                )
                {
                    Melee();
                }
                else if (
                    canShoot
                    && !medusaAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot")
                    && !isExecutingAttack
                    && !finishMelee
                )
                {
                    Aim();
                }
            }

            if (isImmune)
            {
                DamageBlink();
            }
            CheckPlayerDirection();
            DetectPlayer();
        }
    }

    void Death()
    {
        death = true;
        circleCollider.radius = 0.22f;

        gameObject.layer = LayerMask.NameToLayer("PatrolRange");
        // Quitar freeze de Y para que caiga al morir, mantener rotación
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isStopped = true;
        canPatrol = false;
        medusaAnimator.SetTrigger("death");

        uiController.DisabledEnemyCanvas();

        StartCoroutine(SpawnRewardAfterDeath());
    }

    IEnumerator SpawnRewardAfterDeath()
    {
        Destroy(gameObject, 2.4f);
        float elapsed = 0f;
        while (elapsed < 2.4f)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkTime)
            {
                sr.enabled = !sr.enabled;
                blinkTimer = 0f;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void StartBlink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        float elapsedTime = 0f;
        float blinkDuration = 4f;
        float blinkSpeed = 0.2f;

        while (elapsedTime < blinkDuration)
        {
            srPelota.enabled = !srPelota.enabled;
            blinkSpeed = Mathf.Lerp(0.01f, 0.2f, elapsedTime / blinkDuration);

            yield return new WaitForSeconds(blinkSpeed);

            elapsedTime += blinkSpeed;
        }

        srPelota.enabled = true;
    }

    void FollowPlayer()
    {
        if (health <= 0 || death)
            return;

        if (!isCheckingDistance)
            StartCoroutine(CheckDistanceWithDelay());

        // Movimiento directo hacia el jugador (reemplaza NavMeshAgent.destination)
        if (!isStopped && playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// Mueve la medusa horizontalmente hacia el jugador a la velocidad configurada.
    /// Solo mueve en X para mantener la flotacion natural del Rigidbody2D en Y.
    /// </summary>
    void MoveTowardsPlayer()
    {
        float dirX = playerTransform.position.x - transform.position.x;
        float moveDir = Mathf.Sign(dirX);

        if (Mathf.Abs(dirX) > 0.3f)
        {
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    IEnumerator CheckDistanceWithDelay()
    {
        isCheckingDistance = true;

        if (playerTransform == null || death || health <= 0)
        {
            isCheckingDistance = false;
            yield break;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= 1.3f)
        {
            isStopped = true;
            canMelee = true;
            canShoot = false;
        }
        else if (distance <= 1.9f)
        {
            isStopped = false;
            canShoot = true;
            canMelee = false;
        }
        else
        {
            isStopped = false;
            canShoot = false;
            canMelee = false;
        }

        yield return new WaitForSeconds(0.4f);
        isCheckingDistance = false;
    }

    void Aim()
    {
        if (isExecutingAttack)
            return;

        medusaAnimator.Play("Aim");
        aimingTimer += Time.deltaTime;

        if (aimingTimer >= aimTime)
        {
            medusaAnimator.SetTrigger("Shoot");
            aimingTimer = 0;
            canShoot = false;
            actionTimer = 0f;
        }
    }

    void Melee()
    {
        if (!isExecutingAttack)
        {
            StartCoroutine(ExecuteMeleeAttack());
        }
    }

    IEnumerator ExecuteMeleeAttack()
    {
        isExecutingAttack = true;
        canMelee = false;
        rb.linearVelocity = Vector2.zero;
        isStopped = true;

        medusaAnimator.Play("melee");
        yield return new WaitForSeconds(0.3f);
        GetDamagePlayer();

        medusaAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.7f);

        isStopped = false;
        ChangeDirection();

        isExecutingAttack = false;
        finishMelee = true;
        actionTimer = 0f;

        StartCoroutine(FinishedAttack());
    }

    void DetectPlayer()
    {
        if (playerTransform == null)
            return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        hasObjective = distance < 1.7f && !finishMelee;
        canPatrol = !hasObjective;

        if (previousHasObjective && !hasObjective)
        {
            // Dejar de perseguir: parar movimiento
            isStopped = true;
        }

        previousHasObjective = hasObjective;
    }

    IEnumerator FinishedAttack()
    {
        yield return new WaitForSeconds(3f);
        if (finishMelee)
        {
            finishMelee = false;
        }
    }

    void GetDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            // La muerte se gestiona en Update() cuando health == 0
        }
        if (health > 0)
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
            hasObjective = true;
            canPatrol = false;
            isStopped = false;
        }

        if (collision.transform.tag == "Projectile")
        {
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            GetDamage(dmg);
            Destroy(collision.gameObject);
        }

        if (collision.transform.tag == "Attack")
        {
            var dmg = collision.gameObject.GetComponentInParent<CharacterController>().attackDamage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            StartCoroutine(WaitToAnim(dmg));
        }
    }

    private IEnumerator WaitToAnim(int damage)
    {
        yield return new WaitForSeconds(.5f);
        GetDamage(damage);
    }

    private void CheckPlayerDirection()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= followDistance && !finishMelee)
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

    void Patrol()
    {
        if (!canPatrol)
            return;

        // Deteccion de atascamiento: compara posicion actual con la anterior
        stuckCheckTimer += Time.deltaTime;
        if (stuckCheckTimer >= 0.5f)
        {
            float distMoved = Vector2.Distance(transform.position, lastPosition);
            if (distMoved < 0.05f)
            {
                stuckTimer += stuckCheckTimer;
                if (stuckTimer > stuckThreshold)
                {
                    ChangeDirection();
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
            lastPosition = transform.position;
            stuckCheckTimer = 0f;
        }

        if (childrenWallC.isWall)
        {
            ChangeDirection();
        }
        if (lookRight)
        {
            rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-patrolSpeed, rb.linearVelocity.y);
        }

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolTime)
        {
            patrolTimer = 0;
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        lookRight = !lookRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (lookRight ? 1 : -1);
        transform.localScale = scale;
    }

    public void rbForce()
    {
        rb.AddForce(Vector2.up * 500);
        yaImpulsado = true;
        Debug.Log("Aplicando fuerza hacia arriba.");
    }

    void HeightMin()
    {
        Vector2 posicion = transform.position;
        posicion.y = .5f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        transform.position += new Vector3(0, .02f, 0);
    }

    //AJUSTAR ESTO PARA QUE SEA REAL Y AJUSTAR EL DE FUEGO.
    public void GetDamagePlayer()
    {
        if (!atScript.attack || player == null)
            return;

        var ccPlayer = player.GetComponent<CharacterController>();
        if (ccPlayer == null || ccPlayer.isImmune)
            return;

        var srPlayer = player.GetComponentsInChildren<SpriteRenderer>();
        uiController.ConsumeHealth();

        if (uiController.lifes <= 0 && uiController.currentHealth <= 0)
        {
            Destroy(player);
        }
        else
        {
            uiController.ChangePlayerFace();
            ccPlayer.isImmune = true;
            ccPlayer.immuneTimer = 0;
            ccPlayer.blinkTimer = 0;
            foreach (var sr in srPlayer)
            {
                sr.enabled = false;
            }
        }
    }
}
