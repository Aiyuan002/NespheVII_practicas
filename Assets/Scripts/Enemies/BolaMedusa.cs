using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaMedusa : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    CircleCollider2D circleCol;

    public float speed = 8f;
    private Vector2 moveDirection;
    private bool directionSet = false;

    /// <summary>
    /// Llamado por ShootMedusa justo después de instanciar la bala.
    /// </summary>
    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        directionSet = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCol = GetComponent<CircleCollider2D>();
        

        // Hacer cinemático para que la física no interfiera con el movimiento
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Convertir el collider en trigger para que no rebote con otros colliders
        if (circleCol != null)
            circleCol.isTrigger = true;

        // Auto-destruir después de 5 segundos como seguridad
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!directionSet)
            return;

        // Mover la bala cada frame en la dirección del jugador
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
