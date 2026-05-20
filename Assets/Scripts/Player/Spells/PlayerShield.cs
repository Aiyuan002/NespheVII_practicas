using System.Collections;
using UnityEngine;

public class PlayerShield : SpellBehaviour
{
    public static event System.Action OnParry;
    [Tooltip("Animator del GameObject ShieldPlayer (el propio GO en el que está este script)")]
    public Animator playerAnimator;

    [Tooltip("Animator del hijo Shield (solo las animaciones del escudo)")]
    public Animator shieldAnimator;

    [Tooltip("Efecto de parar el tiempo del parry")]
    public float parryDuration = 0.1f;

    // True solo después de que StartShield() haya sido llamado por el animation event.
    // Permite detectar si el botón se soltó antes de que el escudo estuviera desplegado.
    private bool shieldActive = false;

    [Tooltip("Margen de tiempo (segundos) desde que se activa el escudo para considerar el parry perfecto")]
    public float parryWindow = 0.5f;

    private float shieldActivationTime;
    private PlayerController pc;
    private PlayerMainAttack pma;
    private SpriteRenderer sr;

    private void OnEnable()
    {
        shieldActive = false;
        pc = GetComponentInParent<PlayerController>();
        pma = GetComponentInParent<PlayerMainAttack>();
        sr = GetComponent<SpriteRenderer>();
    }

    /// Llamado por Animation Event al terminar la animación de sacar el escudo en ShieldPlayer.
    /// Activa el GO del Shield e inicia su animación de Idle.
    public void StartShield()
    {
        shieldActivationTime = Time.time;
        shieldActive = true;
        shieldAnimator.gameObject.SetActive(true);
        pc.isProtected = true;
        shieldAnimator.SetTrigger("Start");
    }

    /// Llamado por PlayerSpell cuando el jugador suelta el botón de hechizo.
    public override void OnButtonReleased()
    {
        if (!shieldActive)
        {
            // El botón se soltó antes de que el escudo terminara de desplegarse:
            // no hay animación Destroy que esperar, terminar directamente.
            FinishSpell();
            return;
        }

        shieldAnimator.SetTrigger("Destroy");
        StartCoroutine(WaitForDestroyAndFinish());
    }

    private IEnumerator WaitForDestroyAndFinish()
    {
        yield return null;

        // Espera mientras:
        //   - el animator está en transición (Idle → Destroy), o
        //   - el estado actual es en bucle (estado Idle), o
        //   - la animación Destroy aún no ha terminado (normalizedTime < 1)
        while (shieldAnimator.IsInTransition(0)
               || shieldAnimator.GetCurrentAnimatorStateInfo(0).loop
               || shieldAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        shieldAnimator.gameObject.SetActive(false);
        pc.isProtected = false;
        FinishSpell();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("EnemyProjectile")) return;

        Vector2 toProjectile = (Vector2)(other.transform.position - transform.position);
        Vector2 facingDir = pc.lookingRight ? Vector2.right : Vector2.left;
        bool hitFromFront = Vector2.Dot(toProjectile, facingDir) > 0;

        Debug.Log("HitFromFront: " + hitFromFront);

        if (hitFromFront)
        {
            if (shieldActive)
            {
                shieldAnimator.SetTrigger("Hit");
                if (Time.time - shieldActivationTime <= parryWindow)
                    StartCoroutine(OnParrySuccess());
            }
            Destroy(other.gameObject);
        }
        else
        {
            pc.GetDamage();
            Destroy(other.gameObject);
        }
    }

    private IEnumerator OnParrySuccess()
    {
        OnParry?.Invoke();
        pc.uiController.RecoverEnergy(pc.uiController.energyCosumed * 2);
        pma?.ActivatePoweredShot();

        sr.color = new Color(1.0f, 0.85f, 0.2f);

        yield return new WaitForSeconds(parryDuration);

        sr.color = Color.white;
    }
}
