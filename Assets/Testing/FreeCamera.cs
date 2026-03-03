using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocidad máxima de movimiento de la cámara
    public float smoothTime = 0.1f; // Tiempo de suavizado para el movimiento

    public float zoomSpeed = 1f; // Velocidad con la que cambia el tamaño objetivo
    public float smoothFactor = 5f;
    private Vector2 velocity = Vector2.zero; // Velocidad actual de la cámara
    private Vector2 targetVelocity = Vector2.zero; // Velocidad objetivo basada en la entrada

    private Camera camera_;
    public float zoomOutStep = 1f;
    private float targetSize;

    void Start()
    {
        camera_ = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            targetSize -= zoomSpeed * Time.deltaTime;
        }

        // Si se pulsa la tecla Q, se incrementa el targetSize para hacer zoom out
        if (Input.GetKey(KeyCode.Q))
        {
            targetSize += zoomOutStep;
        }

        // Interpolamos suavemente entre el tamaño actual y el tamaño objetivo
        camera_.orthographicSize = Mathf.Lerp(
            camera_.orthographicSize,
            targetSize,
            smoothFactor * Time.deltaTime
        );
        // Obtener la entrada de las teclas WASD
        float horizontal = Input.GetAxisRaw("Horizontal"); // A (izquierda) y D (derecha)
        float vertical = Input.GetAxisRaw("Vertical"); // W (arriba) y S (abajo)

        // Calcular la velocidad objetivo basada en la entrada
        targetVelocity = new Vector2(horizontal, vertical).normalized * moveSpeed;

        // Suavizar la velocidad actual hacia la velocidad objetivo
        velocity = Vector2.Lerp(velocity, targetVelocity, Time.deltaTime / smoothTime);

        // Mover la cámara usando la velocidad suavizada
        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
    }
}
