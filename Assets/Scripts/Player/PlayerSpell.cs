using UnityEngine;
using UnityEngine.InputSystem;

/// Gestiona el ataque secundario del jugador: hechizos/melee (click derecho).
///
/// Sistema nuevo (spellObjects): cada hechizo es un GameObject hijo del Player con un
/// SpellBehaviour. PlayerSpell activa el GO y espera a que el propio hechizo llame a FinishSpell().
///
/// Sistema legado (attack[]): hechizos aún sin GO propio que usan el meleeSprite/meleeAnimator.
/// Se usa cuando spellObjects[numAttack] es null.
///
public class PlayerSpell : MonoBehaviour
{
    // Referencia al CharacterController del mismo GameObject, obtenida en Awake().
    private PlayerController cc;

    // Instancia del sistema de Input.
    private PlayerControls controls;

    // ──────────────────────────────────────────────────────────────────
    // SISTEMA NUEVO — GameObjects de hechizo
    // ──────────────────────────────────────────────────────────────────

    [Header("Hechizos (nuevo sistema)")]
    [Tooltip("Un SpellBehaviour por índice de hechizo. Null = usar sistema legado para ese índice.")]
    public SpellBehaviour[] spellObjects = new SpellBehaviour[5];

    // El SpellBehaviour activo en este momento (null si se usa sistema legado).
    private SpellBehaviour activeSpell;

    // ──────────────────────────────────────────────────────────────────
    // SISTEMA LEGADO — animaciones directas
    // ──────────────────────────────────────────────────────────────────

    [Header("Ataque Melee (sistema legado)")]

    /// Nombres de las animaciones de ataque melee disponibles.
    public string[] attack =
    {
        "Player_Melee_puñetazo",  // 0 — Puñetazo (ataque por defecto)
        "Player_Melee_Espada",          // 1 — Espada
        "Player_Melee_Cadena",          // 2 — Cadena
        "Plaer_Melee_Hammer",           // 3 — Martillo (esta escrito mal aposta, el animator lo usa así)
        "Player_Melee_Escudo",          // 4 — Escudo (dura mientras se mantiene click derecho)
    };

    /// Índice del ataque actualmente seleccionado.
    public int numAttack = 0;

    // Solo para el sistema legado: acumula tiempo para detectar fin de animación.
    private float attackTimer = 0;

    /// Si el jugador está ejecutando un hechizo en este momento.
    public bool isAttacking;

    // Si puede iniciar un nuevo ataque (false durante la animación, true cuando termina).
    private bool canAttack;

    /// Daño que hace el ataque melee. Lo leen los hitboxes del sprite de melee.
    public int attackDamage = 3;

    // Si SetControlEnabled(false) ha desactivado el ataque externamente.
    private bool controlEnabled = true;

    private Rigidbody2D rb;

    // ──────────────────────────────────────────────────────────────────
    // CICLO DE VIDA
    // ──────────────────────────────────────────────────────────────────

    void Awake()
    {
        cc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        controls = new PlayerControls();

        controls.Gameplay.Attack.performed += ctx => Attack();
        controls.Gameplay.Attack.canceled += ctx => CheckEndAttack();
        controls.Gameplay.ChangeAttack.performed += ctx => ChangeAttack();
    }

    void Start()
    {
        // Inicializar con capacidad de atacar.
        canAttack = true;
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    void FixedUpdate()
    {
        // Comprobar si la animación de ataque actual ha terminado.
        if (isAttacking)
            CheckEndLoadAttack();
    }

    // ──────────────────────────────────────────────────────────────────
    // MÉTODOS DE ATAQUE
    // ──────────────────────────────────────────────────────────────────

    /// Punto de entrada del hechizo. Llamado al pulsar click derecho.
    public void Attack()
    {
        if (!controlEnabled)
            return;

        if (!canAttack)
            return;

        if (cc.isSplitShooting)
            return;

        if (cc.isDashing)
            return;

        if (cc.isFalling)
            return;

        if (cc.isClimbing)
            return;

        if (!cc.uiController.ConsumeEnergy())
            return;

        rb.linearVelocity = Vector2.zero;

        canAttack = false;
        isAttacking = true;
        cc.isSpellActive = true;
        cc.canMove = false;

        // Ocultar todos los sprites del player.
        cc.normalSprite.SetActive(false);
        cc.crouchSprite.SetActive(false);
        cc.dashSprite.SetActive(false);
        cc.climbSprite.SetActive(false);
        cc.trepaSprite.SetActive(false);
        cc.colgadoSprite.SetActive(false);

        bool hasSpellObject = spellObjects != null
            && numAttack < spellObjects.Length
            && spellObjects[numAttack] != null;

        if (hasSpellObject)
        {
            // Sistema nuevo: activar el GO del hechizo y esperar a que llame a FinishSpell().
            activeSpell = spellObjects[numAttack];
            activeSpell.gameObject.SetActive(true);
        }
        else
        {
            // Sistema legado: mostrar meleeSprite y reproducir animación.
            cc.meleeSprite.SetActive(true);
            cc.meleeAnimator.Play(attack[numAttack], -1, 0);
        }
    }

    /// Comprueba si la animación del sistema LEGADO ha terminado.
    /// El sistema nuevo no pasa por aquí: los SpellBehaviour llaman a FinishSpell() solos.
    void CheckEndLoadAttack()
    {
        // Sistema nuevo: nada que hacer, el hechizo notifica solo.
        if (activeSpell != null)
            return;

        // Sistema legado: el escudo termina al soltar el botón, no por duración.
        if (cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo")
            || cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_INICIO")
            || cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_ACTIVO"))
            return;

        if (attackTimer < cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).length)
            attackTimer += Time.deltaTime;
        else
            FinishSpell();
    }

    /// Llamado al SOLTAR el botón (Attack.canceled).
    void CheckEndAttack()
    {
        if (!isAttacking) return;

        if (activeSpell != null)
        {
            // Sistema nuevo: notificar al hechizo activo.
            activeSpell.OnButtonReleased();
        }
        else
        {
            // Sistema legado: solo el escudo reacciona a soltar el botón.
            if (cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo")
                || cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_INICIO")
                || cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_ACTIVO"))
            {
                cc.meleeAnimator.SetBool("Shield_Off", true);

                if (attackTimer < cc.meleeAnimator.GetCurrentAnimatorStateInfo(0).length)
                    attackTimer += Time.deltaTime;
                else
                    FinishSpell();
            }
        }
    }

    /// Termina el hechizo activo y restaura el control al jugador.
    /// Llamado por los SpellBehaviour (sistema nuevo) o internamente (sistema legado).
    public void FinishSpell()
    {
        activeSpell?.gameObject.SetActive(false);
        activeSpell = null;

        isAttacking = false;
        canAttack = true;
        cc.isSpellActive = false;
        cc.canMove = true;
        attackTimer = 0;

        cc.normalSprite.SetActive(true);
        cc.crouchSprite.SetActive(false);
        cc.dashSprite.SetActive(false);
        cc.climbSprite.SetActive(false);
        cc.trepaSprite.SetActive(false);
        cc.colgadoSprite.SetActive(false);
        cc.meleeSprite.SetActive(false);

        cc.normalAnimator.SetBool("Land", true);
    }

    /// Cancela el ataque de forma forzada desde el exterior.
    /// Llamado por CharacterController.Jump() cuando el jugador salta mientras ataca.

    /// DIFERENCIA con EndAttack(): canAttack queda en false (no se reactiva inmediatamente).
    /// Se reactiva cuando el jugador aterriza vía OnGrounded(). 
    public void ForceEndAttack()
    {
        if (!isAttacking) return;

        activeSpell?.gameObject.SetActive(false);
        activeSpell = null;

        isAttacking = false;
        canAttack = false;  // Se restaura en OnGrounded(), no aquí
        cc.isSpellActive = false;
        cc.canMove = true;
        attackTimer = 0;

        cc.meleeSprite.SetActive(false);
        cc.normalSprite.SetActive(true);
    }

    /// Restaura la capacidad de atacar al tocar el suelo.
    /// Llamado por CharacterController.Grounding() y SwitchToNormalSprite().
    public void OnGrounded()
    {
        canAttack = true;
    }

    /// Cambia al siguiente tipo de ataque melee ciclando por el array.
    void ChangeAttack()
    {
        int next = numAttack;
        for (int i = 1; i <= spellObjects.Length; i++)
        {
            int candidate = (numAttack + i) % spellObjects.Length;
            if (spellObjects[candidate] != null)
            {
                next = candidate;
                break;
            }
        }
        numAttack = next;
        cc.uiController.NextMeleeAttack();
    }

    /// Selecciona un ataque específico por índice y lo ejecuta inmediatamente.
    public void ExecuteSelectedAttack(int index)
    {
        numAttack = index;

        activeSpell?.gameObject.SetActive(false);
        activeSpell = null;
        isAttacking = false;
        canAttack = true;
        cc.isSpellActive = false;
        cc.canMove = true;
        attackTimer = 0f;

        Attack();
    }

    /// Llamado por CharacterController.SetControlEnabled() para activar/desactivar los ataques.
    public void SetEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }
}
