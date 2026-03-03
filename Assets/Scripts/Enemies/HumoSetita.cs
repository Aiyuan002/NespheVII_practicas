using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumoSetita : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifeTime;
    private float timer;
    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        //timer += Time.deltaTime;
        //if (timer >= lifeTime) { Destroy(gameObject); }
 
    }
    public void DestroyItslef()
    {
        Destroy(this.gameObject);
    }
}
