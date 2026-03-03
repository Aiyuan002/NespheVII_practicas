using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DronBehaviour : MonoBehaviour
{
    [Header("Dron Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;
    public float patrolTime;
    private float patrolTimer;
    public float aimTime;
    private float aimingTimer;
    public float refreshTime;
    private float refreshTimer;
    public Collider2D bodyCollider;
    public GameObject recompensa;
    private SpriteRenderer sr;
    private float immuneTimer;
    public float immuneTime;
    private float blinkTimer;
    public float blinkTime;

    [Header("Sprites")]
    public GameObject eyesSprite;

    //public Color eyesColor3;
    public Color eyesColor2;
    public Color eyesColor1;
    public Color eyesColor0;
    public GameObject explosion;

    [Header("Animators")]
    public Animator dronAnimator;
    public Animator eyesAnimator;

    [Header("AI")]
    //public Transform[] searchPoints;
    private NavMeshAgent agent;
    public Transform playerTransform;
    private Vector3 initialPosition;
    private RobotWallChecker childrenWallC;
    private bool isReturningToInitialPosition = false;

    [Header("UI")]
    public UIController uiController;
    public bool isImmune;

    [Header("Shoot")]
    public Transform shootPosition;
    public GameObject bullet;

    public bool hasObjective;
    public bool lookRight;
    public bool canShoot;
    public bool canPatrol;

    private void Start()
    {
        if (transform.rotation.y == 0)
        {
            lookRight = true;
        }
        else
        {
            lookRight = false;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = false;

        initialPosition = transform.position;
        sr = GetComponent<SpriteRenderer>();
        childrenWallC = GetComponentInChildren<RobotWallChecker>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    private void Update()
    {
        if (hasObjective)
        {
            FollowPlayer();
        }
        if (isReturningToInitialPosition && !canPatrol && !hasObjective)
        {
            BackPosition();
        }
        // if (!canShoot) { Refresh(); }
        if (!hasObjective && canPatrol && !isReturningToInitialPosition)
        {
            Patrol();
        }
        else if (canShoot && !dronAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
        {
            Aim();
        }
        if (isImmune)
        {
            DamageBlink();
        }

        //CheckPosition();
        //CheckDirection();
    }

    //Si pierde vision del player lo busca.
    /*void SearchPlayer()
    {
        int nextPoint = Random.Range(0, searchPoints.Length);

        if (lookRight && searchPoints[nextPoint].position.x > transform.position.x)
        {
            agent.destination = searchPoints[nextPoint].position;
        }
        else if(!lookRight && searchPoints[nextPoint].position.x < transform.position.x)
        {
            agent.destination = searchPoints[nextPoint].position;
        }
        else
        {
            nextPoint = Random.Range(0, searchPoints.Length);
        }
    }*/

    //Comprobar si se encuentra en la posición inicial.


    //void CheckPosition()
    //{
    //    if (agent.remainingDistance < 0.1f)
    //    {
    //        agent.isStopped = true;
    //        canPatrol = true;
    //    }
    //}


    //Si tiene vision del player lo persigue.
    void FollowPlayer()
    {
        agent.isStopped = false;

        if (health > 0)
        {
            agent.destination = playerTransform.position;

            agent.stoppingDistance = 1f;

            if (Vector3.Distance(agent.transform.position, playerTransform.position) <= 1f)
            {
                canShoot = true;
            }
        }
    }

    //Si no tiene vision del player patrulla.
    void Patrol()
    {
        if (!canPatrol)
            return;

        agent.stoppingDistance = 1f;
        if (lookRight)
        {
            transform.Translate(Vector2.right * 0.3f * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector2.left * 0.3f * Time.deltaTime, Space.World);
        }

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolTime)
        {
            patrolTimer = 0;
            ChangeDirection();
        }
        if (childrenWallC.isWall)
        {
            ChangeDirection();
            patrolTimer = 0f;
        }
    }

    //Si tiene vision del player apunta durante un tiempo y despues dispara.
    void Aim()
    {
        dronAnimator.Play("Aim");
        eyesAnimator.Play("Aim");

        aimingTimer += Time.deltaTime;
        if (aimingTimer >= aimTime)
        {
            aimingTimer = 0;
            dronAnimator.SetTrigger("Shoot");
            eyesAnimator.SetTrigger("Shoot");
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
            eyesSprite.transform.rotation = new Quaternion(0, 180, 0, 0);
            lookRight = false;
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            eyesSprite.transform.rotation = new Quaternion(0, 0, 0, 0);
            lookRight = true;
        }

        dronAnimator.Play("ChangeDirection");
        eyesAnimator.Play("ChangeDirection");
    }

    //Ejecuta el disparo.
    void Shoot()
    {
        float angle =
            Mathf.Atan2(
                playerTransform.position.y - shootPosition.position.y,
                playerTransform.position.x - shootPosition.position.x
            ) * Mathf.Rad2Deg;

        // Instancia la bala con la rotación calculada

        Instantiate(bullet, shootPosition.position, Quaternion.Euler(0, 0, angle - 180));
    }

    //Vuelve a la posicion inicial

    void BackPosition()
    {
        agent.destination = initialPosition;
        agent.stoppingDistance = 0.11f;

        if (Vector3.Distance(agent.transform.position, initialPosition) <= 0.19f)
        {
            isReturningToInitialPosition = false;
            canPatrol = true; // Reactiva el patrullaje solo después de llegar a la posición inicial
        }
    }

    //Delay para reiniciar el funcionamiento del dron.
    void Refresh()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshTime)
        {
            refreshTimer = 0;
            canShoot = true;
        }
    }

    //Daña al dron al recibir disparos.
    void GetDamage(int dmg)
    {
        health -= dmg;
        ChangeEyesColor();

        if (health <= 0)
        {
            //Implementar muerte del dron
            GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
            Instantiate(recompensa, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(effect, 0.5f);
            uiController.DisabledEnemyCanvas();
        }
        if (health > 0)
        {
            isImmune = true;
            immuneTimer = 0;
            sr.enabled = false;
            blinkTimer = 0;
        }
    }

    //Cambia el color de los ojos en funcion de la salud restante.
    void ChangeEyesColor()
    {
        if (health == 2)
        {
            eyesSprite.GetComponent<SpriteRenderer>().color = eyesColor2;
        }
        if (health == 1)
        {
            eyesSprite.GetComponent<SpriteRenderer>().color = eyesColor1;
        }
        if (health == 0)
        {
            eyesSprite.GetComponent<SpriteRenderer>().color = eyesColor0;
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
            isReturningToInitialPosition = false;
            hasObjective = true;
            canPatrol = false;
            agent.isStopped = false;
            agent.destination = playerTransform.position;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Debug.Log("problema");
            hasObjective = false;
            ChangeDirection();

            canShoot = false;
            if (health > 0)
            {
                isReturningToInitialPosition = true;
            }

            if (canShoot)
            {
                aimingTimer = 0;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            hasObjective = true;
            if (collision.transform.position.x > transform.position.x)
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
