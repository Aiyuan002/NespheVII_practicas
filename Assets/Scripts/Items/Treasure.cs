using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D treasureCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        treasureCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity == Vector2.zero)
        {
            treasureCollider.isTrigger = true;
        }
    }
}
