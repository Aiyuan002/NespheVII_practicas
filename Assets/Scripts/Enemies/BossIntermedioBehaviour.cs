using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BossIntermedioBehaviour : MonoBehaviour
{
    [Header("Boss Intermedio Attributes")]
    public Sprite faceImage;
    public int EnemyHealth = 200;
    public int maxHealth = 200;

    public float blinkTime = 0.1f;
    public float immuneTime = 1f;
    public float moveSpeed = 2f;
    public float followDistance = 10f;
    public float playerStopDistance = 2f;

    public GameObject recompensa;

    [Header("Block")]
    public bool blockProjectile = false;
    public float blockCooldown = 10f;
    private float blockTimer = 0f;

    [Header("Teleport / Phase / Timers")]
    public float timerFalse = 5f;
    private float timeFalse = 0f;

    public float timerTP = 6f;
    private float timeTP = 0f;

    public float phaseChangeThreshold = 100f;

    [Header("Animator")]
    public Animator BossIntermedioAnimator;

    [Header("IA")]
    public Transform playerTransform;
    public GameObject player;

    [Header("UI")]
    public UIController uiController;

    [Header("Melee")]
    public GameObject gOAtk;
    private AttackPoint atScript;
    private int contador = 0;

    [Header("Shoot")]
    public Transform shootPosition;
    public Transform shootPositionNube;
    public GameObject bullet;
    public GameObject nube;

    [Header("Cinematic")]
    public GameObject cinematic;
    public GameObject transition;
    public Transform puntoDeSpawn;

    [Header("State")]
    public bool death;
    public bool isFollow;
    public bool hasObjective;
    public bool lookRight;
    public bool canAttack = true;
    public bool isImmune;
    public bool canJump = true;
    public bool canFollow = false;
    public bool noGround;
    public bool isAttacking;
    public bool isAttack = false;

    private bool inCinematic = false;
    private bool collarAvailable = true;

    private float blinkTimer = 0f;
    private float immuneTimer = 0f;
    private float distanceToPlayer = 0f;

    private GameObject bulletDestroy;
    private GameObject bulletDestroyNube;

    private int activeCollars = 0;

    private SpriteRenderer sr;

    private float attackCooldown = 3f;
    public float recoveryAfterAttack = 0.75f;
    private bool isRecovering = false;

    private void Start()
    {
        if (uiController == null)
            uiController = GameObject.FindFirstObjectByType<UIController>();

        if (BossIntermedioAnimator == null)
            BossIntermedioAnimator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        if (gOAtk != null)
            atScript = gOAtk.GetComponent<AttackPoint>();

        ValidateReferences();
        UpdateCollarEstado();
    }

    private void Update()
    {
        if (inCinematic)
            return;

        if (death)
        {
            HandleDeath();
            return;
        }

        UpdateTimers();
        UpdateDistanceInfo();
        UpdateAnimatorParameters();

        if (isImmune)
            HandleImmunity();

        DetectPlayer();

        if (hasObjective)
        {
            ChangeDirection(playerTransform.position);
            FollowPlayer();
        }
        else
        {
            isFollow = false;
            isAttacking = false;
        }
    }

    private void ValidateReferences()
    {
        if (uiController == null)
            Debug.LogError("[BossIntermedio] UIController no encontrado.");

        if (BossIntermedioAnimator == null)
            Debug.LogError("[BossIntermedio] Animator no encontrado.");

        if (sr == null)
            Debug.LogError("[BossIntermedio] SpriteRenderer no encontrado.");

        if (playerTransform == null)
            Debug.LogError("[BossIntermedio] playerTransform no asignado.");

        if (player == null)
            Debug.LogError("[BossIntermedio] player no asignado.");

        if (atScript == null)
            Debug.LogWarning("[BossIntermedio] AttackPoint no encontrado en gOAtk.");

        if (shootPosition == null)
            Debug.LogWarning("[BossIntermedio] shootPosition no asignado.");

        if (shootPositionNube == null)
            Debug.LogWarning("[BossIntermedio] shootPositionNube no asignado.");

        if (bullet == null)
            Debug.LogWarning("[BossIntermedio] bullet no asignado.");

        if (nube == null)
            Debug.LogWarning("[BossIntermedio] nube no asignado.");
    }

    private void UpdateTimers()
    {
        timeTP += Time.deltaTime;
        if (timeTP >= timerTP)
            timeTP = 0f;

        if (!collarAvailable)
        {
            timeFalse += Time.deltaTime;
            if (timeFalse >= timerFalse)
            {
                collarAvailable = true;
                timeFalse = 0f;
                BossIntermedioAnimator.SetBool("CollarDispo", true);
            }
        }

        blockTimer += Time.deltaTime;

        if (blockProjectile && blockTimer >= blockCooldown)
        {
            blockProjectile = false;
            blockTimer = 0f;
        }
    }

    private void UpdateDistanceInfo()
    {
        if (playerTransform == null)
            return;

        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
    }

    private void UpdateAnimatorParameters()
    {
        if (BossIntermedioAnimator == null)
            return;

        BossIntermedioAnimator.SetFloat("Distancia", distanceToPlayer);
        BossIntermedioAnimator.SetInteger("Vida", EnemyHealth);
        BossIntermedioAnimator.SetInteger("Contador", contador);
        BossIntermedioAnimator.SetBool("Siguiendo", isFollow);
        BossIntermedioAnimator.SetFloat("TiempoParaTeletransporte", timeTP);
        BossIntermedioAnimator.SetFloat("Tiempo", blockTimer);

        if (atScript != null)
            BossIntermedioAnimator.SetBool("Delante", atScript.attack);
    }

    private void DetectPlayer()
    {
        if (playerTransform == null)
            return;

        hasObjective = Vector2.Distance(transform.position, playerTransform.position) < followDistance;
    }

    private void FollowPlayer()
    {
        if (playerTransform == null || isRecovering)
            return;

        if (distanceToPlayer > playerStopDistance)
        {
            isFollow = true;
            isAttacking = false;
            canJump = true;
            canAttack = false;

            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(playerTransform.position.x, transform.position.y),
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            isFollow = false;
            isAttacking = true;
            canAttack = true;
        }
    }

    public void ChangeDirection(Vector3 playerPos)
    {
        if (transform.position.x < playerPos.x)
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

    private void HandleDeath()
    {
        death = false;
        inCinematic = true;

        if (bulletDestroy != null)
            Destroy(bulletDestroy);

        if (bulletDestroyNube != null)
            Destroy(bulletDestroyNube);

        bulletDestroy = null;
        bulletDestroyNube = null;

        if (uiController != null)
            uiController.DisabledEnemyCanvas();

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        if (transition != null)
        {
            Animator trans = transition.GetComponent<Animator>();
            if (trans != null)
            {
                trans.Play("TransicionEntreCamaras");
                trans.speed = 2f;
            }
        }

        yield return new WaitForSeconds(0.25f);

        if (cinematic != null)
            cinematic.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null && puntoDeSpawn != null)
            foundPlayer.transform.position = puntoDeSpawn.position;
    }

    public void NotifyCollarDispo()
    {
        activeCollars = Mathf.Max(0, activeCollars - 1);
        UpdateCollarEstado();
    }

    public void NotifyCollarNoDispo()
    {
        activeCollars++;
        UpdateCollarEstado();
    }

    private void UpdateCollarEstado()
    {
        collarAvailable = activeCollars == 0;

        if (BossIntermedioAnimator != null)
            BossIntermedioAnimator.SetBool("CollarDispo", collarAvailable);
    }

    // =========================
    // Animation Events - Attacks
    // =========================

    public void MeleeAttack1()
    {
        TriggerAttackCooldown();
        StartCoroutine(AttackRecovery());
    }

    public void MeleeAttack2()
    {
        TriggerAttackCooldown();
        StartCoroutine(AttackRecovery());
    }

    public void RangedAttack()
    {
        TriggerAttackCooldown();
        StartCoroutine(AttackRecovery());
    }

    private IEnumerator AttackRecovery()
    {
        isRecovering = true;
        canAttack = false;
        isFollow = false;
        yield return new WaitForSeconds(recoveryAfterAttack);
        isRecovering = false;
        canAttack = true;
    }

    public void MeleeAttack1Phase2()
    {
        TriggerAttackCooldown();
    }

    public void MeleeAttack2Phase2()
    {
        TriggerAttackCooldown();
    }

    public void RangedCombo()
    {
        TriggerAttackCooldown();
    }

    private void TriggerAttackCooldown()
    {
        canAttack = false;
        CancelInvoke(nameof(EnableAttack));
        Invoke(nameof(EnableAttack), attackCooldown);
    }

    private void EnableAttack()
    {
        canAttack = true;
    }

    // =========================
    // Damage to Boss
    // =========================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (EnemyHealth <= 0 || inCinematic)
            return;

        if (collision.CompareTag("Projectile"))
        {
            HandleProjectileCollision(collision);
            return;
        }

        if (collision.CompareTag("Attack"))
        {
            HandleMeleeCollision(collision);
        }
    }

    private void HandleProjectileCollision(Collider2D collision)
    {
        if (blockTimer >= blockCooldown)
        {
            if (BossIntermedioAnimator != null)
                BossIntermedioAnimator.SetTrigger("Bloquear");

            blockProjectile = true;
            blockTimer = 0f;

            Destroy(collision.gameObject);
            return;
        }

        if (blockProjectile)
        {
            Destroy(collision.gameObject);
            return;
        }

        Projectile projectile = collision.GetComponent<Projectile>();
        if (projectile == null)
            return;

        int dmg = projectile.damage;

        if (uiController != null)
        {
            uiController.EnabledEnemyCanvas(
                EnemyHealth,
                dmg,
                maxHealth,
                gameObject.name,
                faceImage
            );
        }

        if (!isImmune)
            GetDamage(dmg);

        Destroy(collision.gameObject);
    }

    private void HandleMeleeCollision(Collider2D collision)
    {
        CharacterController cc = collision.GetComponentInParent<CharacterController>();
        if (cc == null)
            return;

        int dmg = cc.attackDamage;

        if (uiController != null)
        {
            uiController.EnabledEnemyCanvas(
                EnemyHealth,
                dmg,
                maxHealth,
                gameObject.name,
                faceImage
            );
        }

        if (!isImmune)
            StartCoroutine(WaitToAnim(dmg));
    }

    private IEnumerator WaitToAnim(int damage)
    {
        yield return new WaitForSeconds(0.5f);
        GetDamage(damage);
    }

    private void GetDamage(int dmg)
    {
        EnemyHealth -= dmg;

        if (EnemyHealth <= 0)
        {
            EnemyHealth = 0;
            death = true;
            return;
        }

        isImmune = true;
        immuneTimer = 0f;
        blinkTimer = 0f;

        if (sr != null)
            sr.enabled = false;

        if (uiController != null)
            uiController.ChangePlayerFace();
    }

    private void HandleImmunity()
    {
        immuneTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkTime)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;

            blinkTimer = 0f;
        }

        if (immuneTimer >= immuneTime)
        {
            if (sr != null)
                sr.enabled = true;

            isImmune = false;
        }
    }

    // =========================
    // Shoot
    // =========================

    public void ShootCollar()
    {
        if (shootPosition == null || bullet == null || playerTransform == null)
            return;

        Vector2 direction = playerTransform.position - shootPosition.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletDestroy = Instantiate(
            bullet,
            shootPosition.position,
            Quaternion.Euler(0, 0, angle)
        );
    }

    public void ShootNube()
    {
        if (shootPositionNube == null || nube == null)
            return;

        Vector3 nuevaRotacion = shootPositionNube.rotation.eulerAngles;
        nuevaRotacion.z = -180;

        bulletDestroyNube = Instantiate(
            nube,
            shootPositionNube.position,
            Quaternion.Euler(nuevaRotacion)
        );
    }

    // =========================
    // Damage to Player
    // =========================

    public void GetDamagePlayer()
    {
        if (player == null || atScript == null)
            return;

        CharacterController ccPlayer = player.GetComponent<CharacterController>();
        if (ccPlayer == null || ccPlayer.isImmune)
            return;

        if (!atScript.attack)
            return;

        contador++;
        if (contador > 2)
            contador = 1;

        SpriteRenderer[] srPlayer = player.GetComponentsInChildren<SpriteRenderer>();

        if (uiController != null)
            uiController.ConsumeHealth();

        if (uiController != null && uiController.lifes <= 0 && uiController.currentHealth <= 0)
        {
            Destroy(player);
            return;
        }

        if (uiController != null)
            uiController.ChangePlayerFace();

        ccPlayer.isImmune = true;
        ccPlayer.immuneTimer = 0;
        ccPlayer.blinkTimer = 0;

        StartCoroutine(BlinkEffect(srPlayer, 1.5f, 0.2f));
    }

    private IEnumerator BlinkEffect(SpriteRenderer[] renderers, float duration, float blinkInterval)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].enabled = !renderers[i].enabled;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = true;

        if (player != null)
        {
            CharacterController ccPlayer = player.GetComponent<CharacterController>();
            if (ccPlayer != null)
                ccPlayer.isImmune = false;
        }
    }
}
