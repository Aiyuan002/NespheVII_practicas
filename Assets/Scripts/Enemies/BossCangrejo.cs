using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class BossCangrejo : MonoBehaviour
{
    [Header("Cangrejo Attributes")]
    public Sprite faceImage;
    public int health;
    public int maxHealth;

    private float immuneTimer;
    public float immuneTime;
    private float blinkTimer;
    public float blinkTime;

    private SpriteRenderer sr;

    public bool isFollow;

    public bool death;
    public bool blockProjectile = false;
    public float blockTimer;

    public GameObject recompensa;

    [Header("Animators")]
    public Animator cangrejoAnimator;
    public Animator puertas; //Se encuentra en Grid/Tilemap_Jefe

    [Header("UI")]
    public UIController uiController;
    public bool isImmune;

    [Header("IA")]
    public GameObject player;
    public Transform playerTransform;
    public float distanceToPlayer;
    public bool lookRight;

    [Header("Shoot")]
    public Transform shootPosition;
    public GameObject bullet;

    [Header("Variables")]
    public ActivaMurosBoss scriptPuertas;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cangrejoAnimator = GetComponent<Animator>();
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (death)
        {
            cangrejoAnimator.SetBool("Muerto", death);
            death = false;

            uiController.DisabledEnemyCanvas();
            StartCoroutine(Death());
        }
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        cangrejoAnimator.SetFloat("Distancia", distanceToPlayer);
        cangrejoAnimator.SetInteger("Vida", health);
        cangrejoAnimator.SetBool("Siguiendo", isFollow);
        cangrejoAnimator.SetBool("EsperaLasPuertas", scriptPuertas.activar);
        if (isImmune)
        {
            DamageBlink();
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
        cangrejoAnimator.SetFloat("Tiempo", blockTimer);
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.3f);
        Instantiate(
            recompensa,
            new Vector2(transform.position.x, transform.position.y + 0.1f),
            Quaternion.identity
        );
        Destroy(gameObject);
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
        if (collision.CompareTag("Projectile") && blockTimer >= 10f && scriptPuertas.activar)
        {
            cangrejoAnimator.SetTrigger("Bloquear");
            blockProjectile = true;
        }
        if (collision.transform.tag == "Projectile" && !blockProjectile && scriptPuertas.activar)
        {
            isFollow = true;
            var dmg = collision.gameObject.GetComponent<Projectile>().damage;
            uiController.EnabledEnemyCanvas(health, dmg, maxHealth, gameObject.name, faceImage);
            GetDamage(dmg);
            Destroy(collision.gameObject);
        }

        if (collision.transform.tag == "Attack" && scriptPuertas.activar)
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

    public void GetDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            puertas.Play("AbrirPuertas");
            death = true;
            return;
        }

        if (health > 0)
        {
            isImmune = true;
            immuneTimer = 0;
            sr.enabled = false;
            blinkTimer = 0;
        }
    }

    public void Shoot()
    {
        float angle =
            Mathf.Atan2(
                playerTransform.position.y - shootPosition.position.y,
                playerTransform.position.x - shootPosition.position.x
            ) * Mathf.Rad2Deg;

        // Instancia la bala con la rotación calculada

        Instantiate(bullet, shootPosition.position, Quaternion.Euler(0, 0, angle - 180));
    }

    public void GetDamagePlayer()
    {
        CharacterController ccPlayer = player.GetComponent<CharacterController>();
        if (ccPlayer.isImmune)
            return;
        SpriteRenderer[] srPlayer;

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
            for (int i = 0; i < srPlayer.Length; i++)
            {
                srPlayer[i].enabled = false;
            }
            ccPlayer.blinkTimer = 0;
            Debug.Log("daño");
        }
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        //transform.position = new Vector2(transform.position.x, transform.position.y + 0.09f);
    }

    public void Rotation()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.09f);
        transform.rotation = Quaternion.Euler(0, 0, -5);
    }
}
