using System.Collections;
using UnityEngine;

public class PlayerPunch : SpellBehaviour
{
    [Tooltip("Animator del GameObject PunchPlayer")]
    public Animator playerAnimator;

    [Tooltip("Duración del puñetazo en segundos. Ajustar según la animación.")]
    public float duration = 1f;

    [Tooltip("Transform desde donde se hará el daño")]
    public Transform hitPoint;

    [Tooltip("Daño que hará el puñetazo")]
    public float damage = 25f;

    [Tooltip("Radio del área de daño alrededor de hitPoint")]
    public float hitRadius = 1f;

    [Tooltip("Fuerza de knockback aplicada a los enemigos (impulso)")]
    public float knockbackForce = 8f;

    private PlayerController pc;

    private void OnEnable()
    {
        pc = GetComponentInParent<PlayerController>();

        //Reproducir animación de puñetazo.
        // playerAnimator?.Play("Player_Melee_puñetazo", -1, 0f);

    }

    public void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius);
        foreach (Collider2D hit in hits)
        {
            // Enemigos
            if (hit.CompareTag("Enemy") || hit.CompareTag("BossIntermedio"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage((int)damage, hitPoint.position);

                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockbackDir = (hit.transform.position - pc.transform.position).normalized;
                    rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }

                Debug.Log("HITTEADO EL ENEMY: " + enemy.name);
            }

            // Puertas secretas
            PuertaSecreta puerta = hit.GetComponent<PuertaSecreta>();
            if (puerta != null)
            {
                puerta.ReceiveHit();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }

    public void FinishPunch()
    {
        FinishSpell();
    }
}