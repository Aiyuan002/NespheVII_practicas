using TMPro;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifeTime;
    private float timer;

    public GameObject damageNumber;
    public GameObject empty;
    public GameObject explosion;
    public int[] coll;

    public bool collision_ = false;

    private Vector3 mousePos;
    private Camera mainCamera;
    private Rigidbody2D rb;
    public float force;

    public bool shootNormal = false;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        if (!shootNormal)
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - transform.position;
            Vector3 rotation = transform.position - mousePos;
            rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 180);
        }
        else
        {
            rb.linearVelocity = transform.right * speed;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Platform") ||
            collision.gameObject.CompareTag("Roof") || collision.gameObject.CompareTag("Consumable") || collision.gameObject.CompareTag("PlatformWood"))
            Destroy(gameObject);
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BossIntermedio"))
            HitEnemy(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ManaPlant plant = collision.GetComponent<ManaPlant>();

        if (plant != null)
        {
            plant.Hit();
            Destroy(gameObject);
            return;
        }

        if (!collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BossIntermedio")) return;
        HitEnemy(collision.gameObject);
    }

    private void HitEnemy(GameObject enemyGO)
    {
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.Log($"[Projectile] HitEnemy: no se encontro componente Enemy en '{enemyGO.name}'");
            return;
        }

        if (explosion != null)
        {
            GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
            Debug.Log($"[Audio] AudioManager existe: {AudioManager.Instance != null}");
            Debug.Log($"[Audio] sounds existe: {AudioManager.Instance?.sounds != null}");
            Debug.Log($"[Audio] clip existe: {AudioManager.Instance?.sounds?.explosion != null}");
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.sounds.explosion);
            Destroy(effect, 0.5f);
        }

        if (damageNumber != null)
        {
            GameObject dmgNumber = Instantiate(damageNumber, transform.position, Quaternion.identity);
            dmgNumber.GetComponent<TextMeshPro>().text = damage.ToString();
        }

        enemy.TakeDamage(damage, transform.position);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(empty.transform.position, 0.03f);
    }

    public void DestroyItslef()
    {
        Destroy(this.gameObject);
    }
}
