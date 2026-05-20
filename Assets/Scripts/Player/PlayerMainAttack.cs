using UnityEngine;
using UnityEngine.InputSystem;

/// Gestiona el ataque primario del jugador: disparar con la pistola (click izquierdo).
public class PlayerMainAttack : MonoBehaviour
{

    private PlayerController cc;
    private PlayerSpell playerSpell;

    private PlayerControls controls;

    // ──────────────────────────────────────────────────────────────────
    // VARIABLES DE DISPARO
    // ──────────────────────────────────────────────────────────────────

    [Header("Disparo")]

    /// Tiempo mínimo (en segundos) entre disparos consecutivos.
    public float shootCooldown = 0.5f;

    /// Prefab de la bala que se instancia al disparar.
    public GameObject bullet;

    /// Tiempo de retardo (segundos) antes de restaurar canShoot tras disparar.
    public float shootDelay;

    // Timer que acumula el tiempo desde el último disparo para el retardo shootDelay.
    private float shootDelayTimer;

    // Si el jugador puede disparar en este momento.
    // Se pone a false justo después de disparar y se restaura cuando shootDelayTimer >= shootDelay.
    private bool canShoot = true;

    // Acumula tiempo continuamente para comparar contra shootCooldown
    private float shootCooldownTimer = 0;

    // Si SetControlEnabled(false) ha desactivado el disparo externamente.
    private bool controlEnabled = true;

    // ──────────────────────────────────────────────────────────────────
    // DISPARO POTENCIADO (parry de escudo)
    // ──────────────────────────────────────────────────────────────────

    [Header("Disparo potenciado (parry)")]
    [Tooltip("Multiplicador de daño del disparo potenciado")]
    public float powerShotDamageMultiplier = 2f;

    [Tooltip("Multiplicador de velocidad del disparo potenciado")]
    public float powerShotSpeedMultiplier = 2f;

    private bool pendingPoweredShot = false;

    // ──────────────────────────────────────────────────────────────────
    // VARIABLES DE POINT SHOOTING (apuntado libre con ratón)
    // ──────────────────────────────────────────────────────────────────

    [Header("Point Shooting")]

    /// Siempre esta point shooting, pero yo lo dejo por si acaso. Antes servía para algo y 
    /// me da miedo tocarlo ahora mismo.
    public bool isPointShooting;

    /// Componente que calcula qué zona de apuntado corresponde a la posición del ratón
    /// y devuelve el clip de animación correcto + el Transform de spawn de la bala.
    public PointShooting pointShooting;

    /// Referencia al componente Projectile de la bala para indicarle si debe moverse
    /// hacia el cursor (shootNormal=false) o en su dirección prefijada (shootNormal=true).
    /// Esto también es una movida PERO creo que los enemigos si lo usan en !isNormal, asi que de nuevo,
    /// yo lo modifico aqui por si acaso
    public Projectile projectile;

    // ──────────────────────────────────────────────────────────────────
    // VARIABLES INTERNAS DE ANIMACIÓN DE SPLIT-SHOOT
    //
    // El sistema de split-shoot vive en CharacterController porque mezcla estado de
    // movimiento y disparo. Pero los VALORES que ese sistema necesita los calcula aquí.
    // Se sincronizan a CharacterController en cada FixedUpdate. Esto es más
    // acoplamiento del que me gustaría, pero cambiarlo antes de la demo es simplemente inabarcable.

    // Si al terminar el split-shoot hay que girar el sprite (el jugador apuntó
    // en dirección contraria a la que miraba).
    private bool shouldChangeLookingDirection = false;

    // Si el disparo apunta hacia la derecha.
    private bool shootingRight = true;

    // Nombre del clip de animación de las piernas durante el point-shoot.
    private string pointShootLegsClip;

    // ──────────────────────────────────────────────────────────────────
    // CICLO DE VIDA
    // ──────────────────────────────────────────────────────────────────

    void Awake()
    {
        cc = GetComponent<PlayerController>();
        playerSpell = GetComponent<PlayerSpell>();

        controls = new PlayerControls();

        controls.Gameplay.Shoot.performed += ctx => CheckShootDirection();
    }

    private void OnEnable()  => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    void FixedUpdate()
    {
        // Acumular cooldown entre disparos.
        shootCooldownTimer += Time.deltaTime;

        // Retardo post-disparo: restaurar canShoot después de shootDelay segundos.
        if (!canShoot)
        {
            shootDelayTimer += Time.deltaTime;
            if (shootDelayTimer >= shootDelay)
            {
                shootDelayTimer = 0;
                canShoot = true;
            }
        }

        // Sincronizar valores de animación con CharacterController.
        // UpdateLegsWhileSplitShooting() y EndSplitShoot() siguen en CC porque operan
        // sobre sprites y animadores del movimiento, pero necesitan estos valores.
        cc.shootingRight               = shootingRight;
        cc.shouldChangeLookingDirection = shouldChangeLookingDirection;
        cc.pointShootLegsClip          = pointShootLegsClip;
    }

    // ──────────────────────────────────────────────────────────────────
    // LÓGICA DE DISPARO
    // ──────────────────────────────────────────────────────────────────

    /// Punto de entrada del disparo primario. Llamado al pulsar click izquierdo.
    void CheckShootDirection()
    {
        if (!controlEnabled)
            return;

        // Gate de cooldown: ignorar si no ha pasado suficiente tiempo desde el último disparo.
        if (shootCooldownTimer >= shootCooldown)
            shootCooldownTimer = 0;
        else
            return;

        // Tras un parry exitoso, cancelar el escudo al pulsar disparar para que se sienta responsive.
        if (pendingPoweredShot && cc.isSpellActive)
        {
            cc.isProtected = false;
            playerSpell?.FinishSpell();
        }

        // Condiciones para poder disparar:
        // !canShoot           → todavía en el retardo post-disparo anterior
        // cc.isCrouching      → agachado
        // cc.isClimbing       → escalando
        // !cc.CanUseSplitShoot() → torso ocupado (agachado, escalando, o haciendo melee)
        // cc.currentSpeed == cc.runSpeed → corriendo
        // cc.isChangingDirection → cambiando dirección al correr
        if (!canShoot
            || cc.isCrouching
            || cc.isClimbing
            || cc.isDashing
            || !cc.CanUseSplitShoot()
            || cc.currentSpeed == cc.runSpeed
            || cc.isChangingDirection)
            return;

        // Obtener info de apuntado: clip del torso, clip de piernas, dirección y spawn.
        pointShooting.GetShootInfo(
            out string bodyClipName,
            out string legsClipName,
            out bool aimingRight,
            out int zone,
            out Transform spawnPos
        );

        shouldChangeLookingDirection = (aimingRight != cc.lookingRight);
        shootingRight    = aimingRight;
        pointShootLegsClip = legsClipName;

        // Sincronizar antes de BeginSplitShoot para que UpdateLegsWhileSplitShooting
        // tenga los valores correctos desde el primer frame.
        cc.shootingRight               = shootingRight;
        cc.shouldChangeLookingDirection = shouldChangeLookingDirection;
        cc.pointShootLegsClip          = pointShootLegsClip;

        cc.BeginSplitShoot(bodyClipName);

        GameObject bulletGO = Instantiate(bullet, spawnPos.position, spawnPos.rotation);

        if (pendingPoweredShot)
        {
            Projectile proj = bulletGO.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.damage = Mathf.RoundToInt(proj.damage * powerShotDamageMultiplier);
                proj.force *= powerShotSpeedMultiplier;
                proj.speed *= powerShotSpeedMultiplier;
                proj.transform.localScale = Vector3.one * 2;
                SpriteRenderer sr = bulletGO.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = Color.red;
            }
            pendingPoweredShot = false;
        }

        cc.playerAudio.PlayPlayerShot();

        canShoot = false;
    }

    /// Llamado por CharacterController.SetControlEnabled() para activar/desactivar el disparo.
    public void SetEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }

    /// Activa el próximo disparo como potenciado (llamado desde PlayerShield en parry perfecto).
    public void ActivatePoweredShot()
    {
        pendingPoweredShot = true;
    }
}
