using UnityEngine;
using UnityEngine.Localization;

public class DronBehaviour_JCSantos : MonoBehaviour
{
    public enum DronState
    {
        Patrol,
        Chase,
        Return,
        Attack,
        Dead
    }

    [Header("Dron Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float aimTime = 0.7f;
    public float shootCooldown = 1.5f;
    private float shootTimer;
    private float aimingTimer;
    public float refreshTime;
    private float refreshTimer;

    public GameObject recompensa;
    private SpriteRenderer spriteRenderer;

    private float immuneTimer;
    public float immuneTime;
    private float blinkTimer;
    public float blinkTime;

    [Header("Sprites")]
    public GameObject eyesSprite;
    public Color eyesColor2;
    public Color eyesColor1;
    public Color eyesColor0;
    public GameObject explosion;

    [Header("Animators")]
    public Animator dronAnimator;
    public Animator eyesAnimator;

    [Header("AI")]
    private Vector3 initialPosition;
    private DronState state;

    [Header("UI")]
    public UIController uiController;
    public bool isImmune;

    [Header("Shoot")]
    public Transform shootPosition;
    public GameObject bullet;

    [Header("Localization")]
    public LocalizedString enemyName;

    [Header("UI Distance")]
    public float hideUIDistance = 4f;
    private bool enemyUIShown;

    [Header("Movement")]
    public float patrolTime = 3f;
    private float patrolTimer;
    private bool lookRight = true;
    public float speed = 2f;
    public Transform wallChecker;

    [Header("Detection & Combat")]
    public float loseTargetDelay = 1.2f;
    public float attackDistanceX = 1f;
    public float exitAttackDistanceX = 1.5f;
    public float chaseDeadZoneX = 0.15f;

    private bool playerDetected;
    private float lostTargetTimer;

    private Collider2D col2D;
    private Vector2 movement;

    [Header("References")]
    public Transform playerTransform;
    private Rigidbody2D rb;
    public RobotWallChecker childrenWallC;
    private SpriteRenderer eyesSR;

    private int lastChangeFrame;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        eyesSR = eyesSprite.GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
        shootTimer = 0f;

        initialPosition = transform.position;
        uiController = GameObject.FindFirstObjectByType<UIController>();
        childrenWallC = GetComponentInChildren<RobotWallChecker>();

        //Estado inicial del Drone
        state = DronState.Patrol;
        playerDetected = false;
        lostTargetTimer = 0f;
        isImmune = false;
        immuneTimer = 0f;
        blinkTimer = 0f;
        spriteRenderer.enabled = true;
        if (eyesSR != null) eyesSR.enabled = true;
    }

    private void Update()
    {
        HideUIIfFar();
        CheckStateTransitions();

        switch (state)
        {
            case DronState.Patrol:
                Patrol();
                break;
            case DronState.Chase:
                FollowPlayer();
                break;
            case DronState.Return:
                BackPosition();
                break;
            case DronState.Attack:
                AttackBehaviour();
                break;
        }

        if (isImmune)
            DamageBlink();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement;
    }

    // ---------------- ESTADOS ----------------
    void Patrol()
    {
        Vector2 patrolDir = lookRight ? Vector2.right : Vector2.left;

        if (IsWallAhead())
        {
            ChangeDirection();
            patrolTimer = 0f;
            movement = Vector2.zero;
            return;
        }

        movement = patrolDir * speed;

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolTime)
        {
            patrolTimer = 0f;
            ChangeDirection();
        }
    }

    void FollowPlayer()
    {
        if (playerTransform == null)
        {
            movement = Vector2.zero;
            return;
        }

        float dirX = playerTransform.position.x - transform.position.x;

        // Evita oscilación cuando está casi alineado en X.
        if (Mathf.Abs(dirX) <= chaseDeadZoneX)
        {
            movement = Vector2.zero;
            return;
        }

        if (IsWallAhead())
        {
            movement = Vector2.zero;
            return;
        }

        float sign = Mathf.Sign(dirX);
        LookToDirection(dirX);
        movement = new Vector2(sign * speed, 0f);
    }

    void BackPosition()
    {
        Vector2 dir = (initialPosition - transform.position).normalized;
        LookToDirection(dir.x);

        if (IsWallAhead())
        {
            movement = Vector2.zero;
            return;
        }

        movement = dir * speed;

        if (Vector3.Distance(transform.position, initialPosition) < 0.2f)
        {
            state = DronState.Patrol;
            movement = Vector2.zero;
        }
    }

    void AttackBehaviour()
    {
        movement = Vector2.zero;
        if (playerTransform == null)
            return;
        //Miramos siempre al jugador
        float dirX = playerTransform.position.x - transform.position.x;
        LookToDirection(dirX);

        shootTimer += Time.deltaTime;

        if(shootTimer >= shootCooldown){
            shootTimer = 0f;
            Shoot();
        }
    }

    // ---------------- TRANSICIONES ----------------
    void CheckStateTransitions()
    {
        if (state == DronState.Dead)
            return;
        if (health <= 0){
            Die();
            return;
        }
        //Cuando perdemos al jugador
        if(playerDetected)
            lostTargetTimer = 0f;
        else
            lostTargetTimer += Time.deltaTime;
        //No detecto al jugador cada x tiempo
        if(!playerDetected && lostTargetTimer >= loseTargetDelay){
            state = DronState.Return;
            return;
        }
        //Detecto al jugador
        if(playerDetected && playerTransform != null){
            float dx = Mathf.Abs(playerTransform.position.x - transform.position.x);
            //Si ya está atacando
            if (state == DronState.Attack)
            {
                //Solo salgo si se aleja bastante
                if (dx > exitAttackDistanceX)
                {
                    state = DronState.Chase;
                }
                return;
            }
            //Entramos en ataque cuando nos detecta
            if (dx <= attackDistanceX){
                shootTimer = shootCooldown;
                state = DronState.Attack;
            }
            else
                state = DronState.Chase;
        }
    }

    // ---------------- COLISIONES ----------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = true;
            lostTargetTimer = 0f;
        }

        if (collision.CompareTag("Projectile"))
        {
            var projectile = collision.GetComponent<Projectile>();
            if (projectile != null)
                //GetDamage(projectile.damage); Dño Antiguo
                ProcessHit(projectile.damage);

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Mantiene lock mientras siga dentro.
            playerDetected = true;
            lostTargetTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // No cambiamos de estado aquí directamente.
            playerDetected = false;
        }
    }

    // ---------------- DAÑO ----------------
    void GetDamage(int dmg)
    {
        if (state == DronState.Dead) return;
        if (isImmune) return; // evita daño múltiple durante invulnerabilidad
        health -= dmg;
        if (health <= 0)
            return;
        isImmune = true;
        immuneTimer = immuneTime;  // cuenta atrás total de invulnerabilidad
        blinkTimer = 0f;           // reinicia parpadeo
        // Arranca visible; el parpadeo lo controla DamageBlink
        spriteRenderer.enabled = true;
        if (eyesSR != null) eyesSR.enabled = true;
    }

    void Die()
    {
        state = DronState.Dead;

        if (explosion != null)
        {
            GameObject fx = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(fx, 0.5f);
        }

        if (recompensa != null)
            Instantiate(recompensa, transform.position, Quaternion.identity);

        col2D.enabled = false;
        Destroy(gameObject);
        uiController?.DisabledEnemyCanvas();
    }
    // ---------------- UTILES ----------------
    void ChangeDirection()
    {
        if (Time.frameCount == lastChangeFrame) return;
        lastChangeFrame = Time.frameCount;

        lookRight = !lookRight;
        spriteRenderer.flipX = !lookRight;
        eyesSR.flipX = !lookRight;

        Vector3 checkerPos = wallChecker.localPosition;
        checkerPos.x *= -1f;
        wallChecker.localPosition = checkerPos;
    }

    void Shoot()
    {
        Debug.Log("================== Shoot ================");
        if (playerTransform == null || shootPosition == null || bullet == null) return;

        Vector2 direction = (playerTransform.position - shootPosition.position).normalized;

        GameObject spawnedBullet =  Instantiate(bullet, shootPosition.position, Quaternion.identity);

        DronDisparo dronBullet =  spawnedBullet.GetComponent<DronDisparo>();

        if (dronBullet != null){
            dronBullet.Initialize(direction);
        }
    }

    void LookToDirection(float directionX)
    {
        if (directionX > 0f && !lookRight)
            ChangeDirection();
        else if (directionX < 0f && lookRight)
            ChangeDirection();
    }

    bool IsWallAhead()
    {
        return childrenWallC != null && childrenWallC.isWall;
    }

    void HideUIIfFar() { }
    private void ProcessHit(int damage)
    {
        if (isImmune) return;

        string localizedName = enemyName.IsEmpty ? gameObject.name : enemyName.GetLocalizedString();

        uiController?.EnabledEnemyCanvas(health, damage, maxHealth, localizedName, faceImage);
        enemyUIShown = true;
        GetDamage(damage);
    }

    void DamageBlink()
    {
        if (!isImmune) return;
        // Cuenta atrás de invulnerabilidad
        immuneTimer -= Time.deltaTime;
        // Frecuencia de parpadeo
        float step = (blinkTime > 0f) ? blinkTime : 0.08f;
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= step)
        {
            blinkTimer = 0f;
            bool newState = !spriteRenderer.enabled;
            spriteRenderer.enabled = newState;
            // PArpadeo de los ojos
            if (eyesSR != null)
                eyesSR.enabled = newState;
        }
        // Fin de invulnerabilidad, forzar visible SIEMPRE
        if (immuneTimer <= 0f)
        {
            isImmune = false;
            immuneTimer = 0f;
            blinkTimer = 0f;
            spriteRenderer.enabled = true;
            if (eyesSR != null) eyesSR.enabled = true;
        }
    }
}
