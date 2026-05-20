using UnityEngine;

/// Clase base para todos los hechizos del jugador.
/// Cada hechizo es un GameObject hijo del Player con un script que hereda de esta clase.
///
/// Ciclo de vida:
///   1. PlayerSpell activa el GameObject del hechizo (SetActive(true)).
///   2. El hechizo hace lo suyo (animaciones, colliders, lógica, etc.).
///   3. Cuando el hechizo termina, llama a FinishSpell() para devolver el control.
///
/// Cuándo termina cada hechizo (ejemplos):
///   - Escudo   → al terminar la animación Destroy (el jugador soltó el botón)
///   - Puñetazo → al cabo de 1 segundo (independientemente del botón)
///   - Otros    → cuando su propia lógica lo decida
public abstract class SpellBehaviour : MonoBehaviour
{
    private PlayerSpell playerSpell;

    protected virtual void Awake()
    {
        playerSpell = GetComponentInParent<PlayerSpell>();
    }

    /// Llamado por PlayerSpell cuando el jugador suelta el botón de hechizo.
    /// Sobrescribir solo en hechizos que reaccionen a soltar el botón (p.ej. escudo).
    /// Por defecto no hace nada (p.ej. puñetazo, que termina solo por su propio timer).
    public virtual void OnButtonReleased() { }

    /// Llama a este método cuando el hechizo ha terminado para devolver el control al jugador.
    protected void FinishSpell()
    {
        playerSpell.FinishSpell();
    }
}
