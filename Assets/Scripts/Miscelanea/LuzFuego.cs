using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LuzFuego : MonoBehaviour
{
    UnityEngine.Rendering.Universal.Light2D luz;
    private float targetIntensity; // Intensidad objetivo
    public float intensityChangeSpeed = 2.0f; // Velocidad de cambio
    public float minIntensity = 0.8f; // Mínima intensidad
    public float maxIntensity = 3f; // Máxima intensidad
    public float changeInterval = 0.1f; // Intervalo entre nuevos valores de intensidad
    private float timer = 0f;

    void Start()
    {
        if (luz == null)
        {
            luz = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        }
        targetIntensity = luz.intensity;
    }

    void Update()
    {
        // Temporizador para cambiar el objetivo de la intensidad
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity); // Generar un nuevo objetivo de intensidad
            timer = 0f; // Reiniciar temporizador
        }

        // Suavizar el cambio hacia la intensidad objetivo
        luz.intensity = Mathf.Lerp(
            luz.intensity,
            targetIntensity,
            intensityChangeSpeed * Time.deltaTime
        );
    }
}
