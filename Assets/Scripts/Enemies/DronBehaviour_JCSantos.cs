using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class DronBehaviour_JCSantos : MonoBehaviour
{
    [Header("Dron Attributes")]
    public int health;
    public int maxHealth;

    [Header("Movement")]
    public float speed = 2f;
    private Vector2 initialPosition;

    [Header("References")]
    public Transform playerTransform;
    private Rigidbody2D rb;
    
    private bool hasObjective;
    private bool isReturningToInitialPosition;
    private bool canPatrol = true;
    private bool canShoot;

    private Vector2 movement;

    private void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hasObjective)
        {
            FollowPlayer();
        }
        else if (isReturningToInitialPosition)
        {
            BackPosition();
        }
        else if (canPatrol)
        {
            Patrol();
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("Velocidad: " + movement);
        rb.linearVelocity = movement;
    }

    void FollowPlayer()
    {
        if (health <= 0)
        {
            movement = Vector2.zero;
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        movement = direction * speed;

        if (Vector2.Distance(transform.position, playerTransform.position) <= 1f)
        {
            canShoot = true;
            movement = Vector2.zero;
        }
    }

    void BackPosition()
    {
        Vector2 direction = (initialPosition - (Vector2)transform.position).normalized;
        movement = direction * speed;

        if (Vector2.Distance(transform.position, initialPosition) <= 0.2f)
        {
            isReturningToInitialPosition = false;
            canPatrol = true;
            movement = Vector2.zero;
        }
    }

    void Patrol()
    {
        movement = Vector2.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasObjective = true;
            isReturningToInitialPosition = false;
            canPatrol = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasObjective = false;
            isReturningToInitialPosition = true;
        }
    }
}
