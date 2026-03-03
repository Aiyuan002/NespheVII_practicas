using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifeTime;
    private float timer;

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

        if (!shootNormal)
        {
            rb = GetComponent<Rigidbody2D>();
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - transform.position;
            Vector3 rotation = transform.position - mousePos;
            rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 180);
        }
    }

    private void Update()
    {
        if (shootNormal)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
        Overlap();
    }

    public void Overlap()
    {
        Collider2D coll;

        coll = Physics2D.OverlapCircle(empty.transform.position, 0.03f);
        if (coll != null && coll.CompareTag("Projectile"))
        {
            return;
        }


        if (coll == null)
        {
            //Debug.Log("no hay nada");
        }
        else if (coll.CompareTag("Ground") || coll.CompareTag("Walls"))
        {
            Destroy(this.gameObject);
        }
        else
        {
            //else if (coll.CompareTag("Enemy"))
            //{
            //    Destroy(this.gameObject);
            //}
            if (coll.CompareTag("Player"))
            {
                Debug.Log("aquientrasiono");
                collision_ = true;
                Destroy(this.gameObject);
                collision_ = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name + " bro aqui");
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
        Debug.Log("quecolisiona" + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            //Hace que salga una explosion en el player
            if (explosion != null)
            {
                GameObject effect = Instantiate(explosion, transform.position, transform.rotation);
                Destroy(effect, 0.5f);
            }
        }
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
