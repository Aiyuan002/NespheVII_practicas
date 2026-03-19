using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 2f;
    public float lifeTime = 2.5f;

    private float timer;

    public GameObject empty;
    public GameObject explosion;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
        CheckCollision();
    }

    void CheckCollision()
    {
        if (empty == null) return;

        Collider2D coll = Physics2D.OverlapCircle(empty.transform.position, 0.03f);

        if (coll == null) return;
        if (coll.CompareTag("Ground") || coll.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }

        if (coll.CompareTag("Player"))
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}