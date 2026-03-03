using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProjectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifeTime;
    private float timer;

    public GameObject empty;
    public int[] coll;

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifeTime) { Destroy(gameObject); }
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
                //Debug.Log("a");
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(empty.transform.position, 0.03f);
    }
}
