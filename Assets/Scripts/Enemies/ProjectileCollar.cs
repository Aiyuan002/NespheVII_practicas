using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollar : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifeTime;
    public float timer;

    float timeFalse;
    float timerFalse;

    private Transform bossIntermedio;
    private Transform shootPositionBoss;
    private Transform shootPositionBossBack;
    private BossIntermedioBehaviour bossScript;

    public bool isReturning = false;

    public GameObject empty;

    public GameObject explosion;
    public int[] coll;

    public bool collision_ = false;

    public void Start()
    {
        GameObject bossObj = GameObject.FindGameObjectWithTag("BossIntermedio");

        if (bossObj != null)
        {
            bossScript = bossObj.gameObject.GetComponent<BossIntermedioBehaviour>();
            bossIntermedio = bossObj.gameObject.transform;
            bossScript.NotifyCollarNoDispo();
            shootPositionBossBack = bossIntermedio.gameObject.transform.Find(
                "ShootPositionBossBack"
            );
            //Creo que es
            shootPositionBoss = bossIntermedio.gameObject.transform.Find("ShootPositionBoss");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (shootPositionBoss == null)
        {
            return;
        }
        if (!isReturning)
        {
            // Movimiento inicial hacia adelante
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            // Comprobar si debe regresar
            if (timer >= lifeTime)
            {
                isReturning = true;
                // Resetear timer si quieres medir el tiempo de retorno
            }
        }
        else
        {
            Debug.Log("aqui no hay quien viva");
            // Movimiento hacia la posición de retorno
            transform.position = Vector2.MoveTowards(
                transform.position,
                shootPositionBossBack.position,
                speed * Time.deltaTime
            );

            // Opcional: Destruir al llegar cerca
            if (Vector2.Distance(transform.position, shootPositionBossBack.position) < 0.1f)
            {
                isReturning = false;
                DestroyProjectile();
            }
        }

        Overlap();
    }

    public void Overlap()
    {
        Collider2D coll;
        coll = Physics2D.OverlapCircle(empty.transform.position, 0.03f);

        if (coll == null)
        {
            //Debug.Log("no hay nada");
        }
        else
        {
            if (coll.CompareTag("Ground"))
            {
                DestroyProjectile();
            }
            //else if (coll.CompareTag("Enemy"))
            //{
            //   Destroy(this.gameObject);
            //}
            else if (coll.CompareTag("Player"))
            {
                Debug.Log("aquientrasiono");
                collision_ = true;
                DestroyProjectile();
                collision_ = false;
            }
        }
    }

    void DestroyProjectile()
    {
        if (bossScript != null)
            bossScript.NotifyCollarDispo();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Overlap no funciona

        if (collision.gameObject.name == "Tilemap_Col")
        {
            DestroyProjectile();
        }
        Debug.Log("quecolisiona" + collision.gameObject.tag);
        if (collision.CompareTag("Player"))
        {
            //Hace que salga una explosion en el player
            if (explosion != null)
            {
                GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
                Destroy(effect, 0.5f);
            }
            DestroyProjectile();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(empty.transform.position, 0.03f);
    }
}
