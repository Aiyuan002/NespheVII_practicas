using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BossIntermedioBehaviour : MonoBehaviour
{
    [Header("Boss Intermedio Attributes")]
    public Sprite faceImage;
    public int EnemyHealth;
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
    public bool death;
    public bool isFollow;

    public bool blockProjectile = false;
    public float blockTimer;
    float timeFalse;
    public float timerFalse;

    float timeTP;
    public float timerTP;

    [Header("Animators")]
    public Animator BossIntermedioAnimator;

    public float phaseChangeThreshold = 100f; // Umbral para cambiar a la fase 2 (50% de vida)

    private float attackCooldown = 3f; // Tiempo de espera después de cada ataque

    //private float phaseCooldown = 4f; // Tiempo antes de cambiar de fase NO SE ESTÁ USANDO

    [Header("IA")]
    public float playerStopDistance;
    public Transform playerTransform;
    private Rigidbody2D rb;
    private RobotWallChecker childrenWallC;
    private RobotGroundChecker childrenGroundC;
    private float distanceToPlayer;

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

    private int activeCollars = 0;

    [Header("Melee")]
    public GameObject gOAtk;
    AttackPoint atScript;
    public GameObject player;
    int contador;

    [Header("Shoot")]
    public Transform shootPosition;
    public Transform shootPositionNube;
    public GameObject bullet;
    public GameObject nube;

    bool collarAvailable;

    [Header("Cinematic")]
    bool inCinematic = false;
    public GameObject cinematic;
    public GameObject transition;
    GameObject bulletDestroy;
    private GameObject bulletDestroyNube;

    void Start()
    {
        uiController = GameObject.FindFirstObjectByType<UIController>();
        BossIntermedioAnimator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        atScript = gOAtk.GetComponent<AttackPoint>();
    }

    private void Update()
    {
        if (death)
        {
            // BossIntermedioAnimator.SetBool("Muerto", death);

            Destroy(bulletDestroy);
            death = false;
            inCinematic = true;
            //cinematica
            StartCoroutine(Death());
        }
        if (inCinematic)
        {
            Destroy(bulletDestroyNube);
            Destroy(bulletDestroy);
        }

        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        BossIntermedioAnimator.SetFloat("Distancia", distanceToPlayer);
        BossIntermedioAnimator.SetInteger("Vida", EnemyHealth);
        BossIntermedioAnimator.SetInteger("Contador", contador);
        BossIntermedioAnimator.SetBool("Siguiendo", isFollow);
        BossIntermedioAnimator.SetBool("Delante", atScript.attack);
        timeTP += Time.deltaTime;
        if (timeTP >= timerTP)
        {
            timeTP = 0f;
        }
        BossIntermedioAnimator.SetFloat("TiempoParaTeletransporte", timeTP);
        if (!collarAvailable)
        {
            timeFalse += Time.deltaTime;
            if (timeFalse >= timerFalse)
            {
                BossIntermedioAnimator.SetBool("CollarDispo", true);
                collarAvailable = true;
                timeFalse = 0;
            }
        }

        if (isImmune)
        {
            Damage();
        }
        if (blockTimer >= 10f)
        {
            if (!blockProjectile)
            {
                return;
            }
            else
            {
                blockTimer = 0f;
                blockProjectile = false;
            }
        }
        else
        {
            blockTimer += Time.deltaTime;
        }

        BossIntermedioAnimator.SetFloat("Tiempo", blockTimer);
    }

    IEnumerator Death()
    {
        Animator trans = transition.GetComponent<Animator>();
        trans.Play("TransicionEntreCamaras");
        trans.speed = 2f;

        yield return new WaitForSeconds(0.25f);

        cinematic.SetActive(true);
    }

    public void NotifyCollarDispo()
    {
        activeCollars--;
        Debug.Log($"Collares activos (Dispo): {activeCollars}");
        UpdateCollarEstado();
    }

    public void NotifyCollarNoDispo()
    {
        activeCollars++;
        Debug.Log($"Collares activos (Dispo): {activeCollars}");
        UpdateCollarEstado();
    }

    private void UpdateCollarEstado()
    {
        collarAvailable = (activeCollars == 0);
        BossIntermedioAnimator.SetBool("CollarDispo", collarAvailable);
    }

    private void MeleeAttack1()
    {
        // Lógica de la animación y el ataque Melee1 para la fase 1
        // Colocar la lógica de daño al jugador aquí
        //Debug.Log("Ataque Melee1 fase 1");
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void MeleeAttack2()
    {
        // Lógica de la animación y el ataque Melee2 para la fase 1
        // Colocar la lógica de daño al jugador aquí
        //Debug.Log("Ataque Melee2 fase 1");
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void RangedAttack()
    {
        // Lógica de la animación y el ataque a distancia para la fase 1
        // Colocar la lógica de daño al jugador aquí
        //Debug.Log("Ataque a distancia fase 1");
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void MeleeAttack1Phase2()
    {
        // Lógica de la animación y el ataque Melee1 para la fase 2
        // Colocar la lógica de daño al jugador aquí
        //Debug.Log("Ataque Melee1 fase 2");
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void MeleeAttack2Phase2()
    {
        // Lógica de la animación y el ataque Melee2 para la fase 2
        // Colocar la lógica de daño al jugador aquí
        //Debug.Log("Ataque Melee2 fase 2");
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void RangedCombo()
    {
        // Lógica de la animación y el ataque a distancia en combo para la fase 2
        // Colocar la lógica de daño al jugador aquí
        //Debug.Log("Ataque a distancia en combo fase 2");
        canAttack = false;
        Invoke("EnableAttack", attackCooldown);
    }

    private void EnableAttack()
    {
        // Permitir que el boss ataque nuevamente después del tiempo de espera
        canAttack = true;
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Debug.Log("cuanta" + distanceToPlayer);
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

            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(playerTransform.position.x, transform.position.y),
                moveSpeed * Time.deltaTime
            );
        }
        else if (distanceToPlayer <= playerStopDistance) // Si está en rango de ataque
        {
            //   canAttack = true;
            isAttacking = true;
        }
    }

    public void ChangeDirection(Vector3 player)
    {
        Debug.Log("delante " + (transform.position.x > player.x));
        if (transform.position.x < player.x)
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

    void DetectPlayer()
    {
        Debug.Log(Vector2.Distance(transform.position, playerTransform.position));
        if (Vector2.Distance(transform.position, playerTransform.position) < followDistance)
        {
            hasObjective = true;
        }
        else // Fuera del rango
        {
            hasObjective = false;
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
        if (collision.CompareTag("Projectile") && blockTimer >= 10f)
        {
            BossIntermedioAnimator.SetTrigger("Bloquear");
            blockProjectile = true;
        }
        if (collision.transform.tag == "Projectile" && EnemyHealth > 0 && !blockProjectile)
        {
            var projectile = collision.GetComponent<Projectile>();

            int dmg = projectile.damage;

            uiController.EnabledEnemyCanvas(
                EnemyHealth,
                dmg,
                maxHealth,
                gameObject.name,
                faceImage
            );
            if (!isImmune)
            {
                GetDamage(dmg);
            }
            Destroy(collision.gameObject);
        }

        if (collision.transform.tag == "Attack" && EnemyHealth > 0)
        {
            var dmg = collision.gameObject.GetComponentInParent<CharacterController>().attackDamage;
            uiController.EnabledEnemyCanvas(
                EnemyHealth,
                dmg,
                maxHealth,
                gameObject.name,
                faceImage
            );
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

    void GetDamage(int dmg)
    {
        EnemyHealth -= dmg;

        if (EnemyHealth == 0)
        {
            death = true;
            uiController.DisabledEnemyCanvas();

            return;
        }

        if (EnemyHealth > 0)
        {
            isImmune = true;
            immuneTimer = 0;
            sr.enabled = false;
            blinkTimer = 0;
            uiController.ChangePlayerFace();
        }
    }

    public void ShootCollar()
    {
        Vector2 direction = playerTransform.position - shootPosition.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletDestroy = Instantiate(bullet, shootPosition.position, Quaternion.Euler(0, 0, angle));
    }

    public void ShootNube()
    {
        Vector3 nuevaRotacion = shootPosition.rotation.eulerAngles;

        nuevaRotacion.z = -180;
        bulletDestroyNube = Instantiate(
            nube,
            shootPositionNube.position,
            Quaternion.Euler(nuevaRotacion)
        );
    }

    public void GetDamagePlayer()
    {
        CharacterController ccPlayer = player.GetComponent<CharacterController>();
        if (ccPlayer.isImmune)
            return;
        if (contador == 2)
        {
            contador = 0;
        }
        contador += 1;

        if (atScript.attack)
        {
            SpriteRenderer[] srPlayer;

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
                StartCoroutine(BlinkEffect(srPlayer, 1.5f, 0.2f));
                Debug.Log("daño");
            }
        }
    }

    private IEnumerator BlinkEffect(SpriteRenderer[] renderers, float duration, float blinkInterval)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Alternar visibilidad de los sprites
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = !renderers[i].enabled;
            }
            // Esperar el intervalo de parpadeo
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Asegurarse de que los sprites queden visibles al final
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = true;
        }

        // Desactivar inmunidad después del parpadeo
        CharacterController ccPlayer = player.GetComponent<CharacterController>();
        ccPlayer.isImmune = false;
    }

    void Damage()
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
}
