using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecargarAscensor : MonoBehaviour
{
    [SerializeField]
    private GameObject entrada;

    [SerializeField]
    private SpriteRenderer barraAzul;

    [SerializeField]
    private TextMeshPro textEnergy;

    public int colleted;
    private int previousColleted;

    private float blinkTimer;
    private float blinkTime = 0.3f;
    bool subiendo = false;
    private bool isAnimating = false;
    public TextMeshPro textInyectar; //implementar

    private bool playerInRange = false;

    private void OnEnable()
    {
        CharacterController.OnPlayerInteract += PlayerInteracted;
    }

    private void OnDisable()
    {
        CharacterController.OnPlayerInteract -= PlayerInteracted;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        previousColleted = colleted;
        if (other.CompareTag("Player") && !isAnimating)
        {
            playerInRange = true;
            if(colleted == 0)
            {
                textInyectar.text = "Necesitas recolectar energia";
                return;
            }
            textInyectar.text = "Presiona E para inyectar energia";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            textInyectar.text = "";
        }
    }

    private void PlayerInteracted()
    {
        if (!playerInRange)
            return;

        textInyectar.text = "";

        if(colleted > 0)
            textEnergy.text = "";

        if (previousColleted == 1)
        {
            StartCoroutine(AnimarBarra(.5f, 2f));
        }
        else if (previousColleted == 2)
        {
            StartCoroutine(AnimarBarra(1f, 3f));
        }
        else if (previousColleted == 3)
        {
            StartCoroutine(AnimarBarra(1.5f, 4f));
        }
        else if (previousColleted == 4)
        {
            StartCoroutine(AnimarBarra(2f, 5f));
        }
        else if (previousColleted == 5)
        {
            StartCoroutine(AnimarBarra(2.5f, 6f));
        }
    }

    IEnumerator AnimarBarra(float level, float duracion)
    {
        isAnimating = true;
        float tiempoTranscurrido = 0f;
        float valorInicial = barraAzul.size.x;
        float valorFinal = level;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracion;
            float nuevoAncho = Mathf.Lerp(valorInicial, valorFinal, t);
            barraAzul.size = new Vector2(nuevoAncho, barraAzul.size.y);
            yield return null;
        }

        barraAzul.size = new Vector2(valorFinal, barraAzul.size.y);
        isAnimating = false;
        if (previousColleted == 5 && !subiendo)
        {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        subiendo = true;
        float elapsed = 0f;
        textEnergy.text = " COMPLETADO";

        Animator barAnimator = textEnergy.gameObject.GetComponent<Animator>();
        barAnimator.Play("ProgressBarElevator");
        while (elapsed < 2f)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkTime)
            {
                textEnergy.enabled = !textEnergy.enabled;
                blinkTimer = 0f;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        textEnergy.enabled = true;

        Open();
    }

    private void Open()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play("Ascensor");
        entrada.SetActive(true);
    }
}
