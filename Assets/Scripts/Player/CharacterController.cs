using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public static event Action OnPlayerInteract;
    [Header("Testing")]
    public float currentSpeed;

    [Header("UI")]
    private UIController uiController;

    [Header("Character Attributes")]
    public float blinkTimer;
    public float blinkTime;
    public float immuneTimer;
    public float immuneTime;
    private SpriteRenderer[] sr;
    public bool isImmune;

    [Header("Animators")]
    public Animator normalAnimator;
    public Animator topAnimator;
    public Animator botAnimator;
    public Animator crouchAnimator;
    public Animator climbAnimator;
    public Animator fullAnimator;
    public Animator meleeAnimator;
    public Animator trepaAnimator;
    public Animator colgadoAnimator;
    public Animator muerteAnimator;

    //Movimiento
    [Header("Movement")]
    public float moveSpeed = 1;
    public float runSpeed = 2.5f;

    //private float currentSpeed;
    private Vector2 move;
    private bool lookingRight = true;
    private bool canMove;
    private bool isMoving;
    public bool canStandUp = true;
    public bool canDecelerate = true;
    public bool canAccelerate;
    private float changeDirSpeed;
    private bool isChangingDirection = false;
    public bool stopWalls = false;

    //Salto
    [Header("Jump")]
    public float firstJumpForce;
    public float secondJumpForce;
    public bool canJump = false;
    public bool allowSecondJump = false;
    private int numOfJumps = 2;
    private float maxJumpSpeed = 7;
    private Rigidbody2D rb;
    public bool atGround;
    public bool isFalling;
    public float jumpTime;
    private float jumpTimer;
    private bool doJump;
    private bool heightDamage = false;
    public bool canDoubleJump = false;
    public bool isJumping;

    //Posiciones de disparo
    [Header("Shoot Positions")]
    public Transform hShootPosition;
    public Transform vShootPosition;
    public Transform dShootPosition;
    public Transform hShootPositionCrouch;
    public Transform hShootPositionClimb;
    public Transform dUpShootPositionClimb;
    public Transform dDownShootPositionClimb;
    public Transform vDownShootPositionRoof;
    public Transform dDownShootPositionRoof;

    //Disparo
    [Header("Shoot")]
    public GameObject bullet;
    public float shootDelay;
    private float shootDelayTimer;
    private bool canShoot = true;
    public float delayToMove;
    public float delayToMoveTimer;
    private Vector2 shootDirection;
    private float delayToIdleTimer;
    private bool isShooting;

    // Split shoot (torso/piernas)
    [Header("Split Shoot (Torso/Legs)")]
    [SerializeField] private float splitShootSafetyTime = 0.2f; // fallback si el clip no se puede leer este frame
    private bool isSplitShooting;
    private float splitShootTimer;
    private string splitLegsStateRequested;

    // Necesario para salir de Fall al aterrizar mientras sigues disparando
    private bool legsNeedsLanding;

    //Crouch
    [Header("Crouch")]
    public float crouchMoveSpeed;
    public bool isCrouching;
    private float crouchValue;

    //Desplazamiento
    [Header("Dash")]
    public float dashDistance;
    public float dashSpeed;
    private Vector2 dashPosition;
    private bool doDash;
    public bool canDash;
    private bool isDashing;
    private int stuckFrames = 0;
    private const float stuckThreshold = 0.001f; // Umbral de movimiento mínimo
    [SerializeField] private /*const*/ int maxStuckFrames = 10;

    public Transform dashChecker;
    public float dashCheckerRadius;

    //Climb
    [Header("Climb")]
    public float climbMoveSpeed;
    public bool isClimbing = false;
    public bool isWallTouching = false;
    public bool isRoofTouching = false;
    public bool isVineTouching = false;
    public bool isWallClimbing = false;
    public bool isRoofClimbing = false;
    public bool isRoofCrouch = false;
    public bool canClimb = true;
    private bool canGroundThisFrame = true;

    //Ataque melee
    [Header("Melee")]
    public string[] attack =
    {
        "Player_Melee_puñetazo",
        "Player_Melee_Espada",
        "Player_Melee_Cadena",
        "Plaer_Melee_Hammer",
        "Player_Melee_Escudo",
        "Player_Melee_Batcuerda_INICIO",
        "", //Animación de la otra cuerda
    };
    public int numAttack = 0;
    private float attackTimer = 0;
    public bool isAttacking;
    public bool canAttack;
    public int attackDamage = 3;

    [Space(5)]
    [Header("Hook")]
    public LineRenderer line;
    private RaycastHit2D hit;

    public DistanceJoint2D joint;
    private Transform hookOrigin;
    public bool isAiming;
    public bool isHooking;
    public bool stopHook = false;

    [Header("Rope")]
    public GameObject shootRope;
    public GameObject anchor;
    public GameObject rope;
    private Vector2 ropeDirection;
    public bool isAimingRope;
    private float loadRopeTimer;
    public float loadRopeTime;
    private int ropeLength = 1;
    public int maxRopeLength;

    [Header("Items")]
    public int armorItems;
    public int weaponItems;
    public int ammunitionItems;
    public int meleeItems;
    public int consumableItems;
    public int abilityItems;
    public int missionItmes;

    [Header("Sprites")]
    //Cambios sprites
    public GameObject fullSprite;
    //public GameObject normalSprite;
    public GameObject crouchSprite;
    public GameObject dashSprite;
    public GameObject climbSprite;
    public GameObject meleeSprite;
    public GameObject trepaSprite;
    public GameObject colgadoSprite;
    public GameObject muerteSprite;
    public GameObject normalSprite;
    public GameObject shootSprite;

    private PlayerControls controls;
    public bool enablePlatform;

    [Header("Dialogs")]
    private bool isNearNPC = false;
    public GameObject attacks;
    public GameObject minimap;
    public GameObject textPulsaF;
    public GameObject PanelDialogo;
    public Text textDialogo;
    public GameObject canExit;

    //private bool isDialogeInProgress = false;
    private bool isDialogueInProgress;
    private bool canPressF = true;
    int contador = 0;

    public Piolet voidPiolet;

    [Header("Respawn/Checkpoint")]

    [Space(5)]
    [Header("Inventory")]
    public Inventory inventory;

    [Space(5)]
    [Header("Post Processing")]
    public Volume volume;

    [Space(5)]
    [Header("PointShooting")]
    public bool isPointShooting;
    public PointShooting pointShooting;
    public Projectile projectile;

    void Awake()
    {
        controls = new PlayerControls();

        //controls.Gameplay.Jump.started += ctx => Jump();
        controls.Gameplay.Jump.performed += ctx => CheckJump();
        controls.Gameplay.Jump.canceled += ctx => CheckSecondJump();
        controls.Gameplay.Jump.canceled += ctx => doJump = false;
        controls.Gameplay.Shoot.performed += ctx => CheckShootDirection();
        controls.Gameplay.Crouch.performed += ctx => crouchValue = ctx.ReadValue<float>();
        controls.Gameplay.Crouch.performed += ctx => Crouch();
        controls.Gameplay.Crouch.canceled += ctx =>
        {
            crouchValue = 0;
            if (atGround && !isClimbing)
            {
                CheckTop();
            }
            else
            {
                isCrouching = false;
            }
        };
        controls.Gameplay.Dash.performed += ctx => Dash();
        controls.Gameplay.Run.performed += ctx => Run();
        controls.Gameplay.Run.canceled += ctx => currentSpeed = moveSpeed;
        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
        controls.Gameplay.ShootDirection.performed += ctx =>
            shootDirection = ctx.ReadValue<Vector2>();
        controls.Gameplay.ShootDirection.canceled += ctx => shootDirection = Vector2.zero;
        controls.Gameplay.Climb.performed += ctx => Climb();
        controls.Gameplay.Attack.performed += ctx => Attack();
        controls.Gameplay.Attack.canceled += ctx => CheckEndAttack();
        controls.Gameplay.ChangeAttack.performed += ctx => ChangeAttack();
        controls.Gameplay.RoofCrouch.performed += ctx => RoofCrouch();
        controls.Gameplay.RoofCrouch.canceled += ctx => StopRoofCrouch();
        controls.Gameplay.AttackDirection.performed += ctx =>
            ropeDirection = ctx.ReadValue<Vector2>();
        controls.Gameplay.Talk.performed += ctx => Talk();
        controls.Gameplay.Interact.performed += ctx =>
        {
            OnPlayerInteract?.Invoke();
        };
    }

    private void Start()
    {
        uiController = GameObject.Find("UI").GetComponentInChildren<UIController>();

        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed;
        changeDirSpeed = runSpeed;
        muerteSprite.SetActive(false);
        canMove = true;
        canAttack = true;
        line.enabled = false;
        joint.enabled = false;

        if (shootSprite != null)
            shootSprite.SetActive(false);
    }

    private void FixedUpdate()
    {
        isJumping = !atGround && rb.linearVelocity.y > 0f;

        if (isPointShooting)
        {
            projectile.shootNormal = false;
            pointShooting.enabled = true;
        }
        else
        {
            projectile.shootNormal = true;
            pointShooting.enabled = false;
        }

        // Control fin de disparo split (torso/piernas)
        if (isSplitShooting)
        {
            splitShootTimer += Time.deltaTime;

            // Mientras el torso dispara, las piernas siguen el movimiento/salto real.
            UpdateLegsWhileSplitShooting();

            if (splitShootTimer >= GetCurrentTopClipLengthOrFallback())
            {
                EndSplitShoot();
            }
        }

        if (!isWallTouching && !isRoofTouching && !isVineTouching)
        {
            if (isClimbing)
            {
                // Cuando terminamos de escalar, obtenemos un pequeño impulso hacia arriba para llegar mas facilmente a la plataforma o suelo elevado
                if (move.y > 0)
                {
                    if (isWallClimbing)
                        JumpWithPlatform(firstJumpForce * 1.5f, true);
                    else
                        JumpWithPlatform(firstJumpForce, true);
                    isWallTouching = false;
                    isVineTouching = false;
                    canGroundThisFrame = false;
                    canClimb = false;
                    StartCoroutine(ResetCanGroundThisFrame());
                    StartCoroutine(ResetCanClimb());
                    //Debug.Log("canClimb: " + canClimb + " tiempo: " + Time.time);

                    Debug.Log("IMPULSAO");
                }

                isWallClimbing = false;
                isRoofClimbing = false;

                climbSprite.SetActive(false);
                trepaSprite.SetActive(false);
                colgadoSprite.SetActive(false);
                normalSprite.SetActive(true);
                currentSpeed = moveSpeed;
            }

            rb.gravityScale = 1;
            isClimbing = false;
        }
        if (isImmune)
        {
            immuneTimer += Time.deltaTime;
            // Lógica de parpadeo aquí
            if (immuneTimer >= immuneTime)
            {
                isImmune = false;
            }
        }

        if (isVineTouching || isWallTouching)
        {
            doJump = false;
            Climb();
        }
        //Apuntar y disparar cuerda
        if (isAiming)
        {
            CheckShootDirection();
        }
        else if (isHooking)
        {
            if (hit.collider != null)
            {
                joint.enabled = true;
                line.SetPosition(0, hookOrigin.position);
                line.SetPosition(1, hit.point);
                joint.connectedAnchor = hit.point;

                if (stopHook)
                {
                    line.enabled = false;
                    joint.enabled = false;
                    isHooking = false;
                    stopHook = false;
                }
            }
            else
            {
                isHooking = false;
            }
        }

        if (isAimingRope)
        {
            AimRope();
        }

        if (isImmune)
        {
            DamageBlink();
        }

        ChangeWallRoofClimb();

        if (isAttacking)
        {
            CheckEndLoadAttack();
        }

        //Salto
        if (canJump)
        {
            Jump();
        }

        //Retardo de disparo.
        if (!canShoot)
        {
            shootDelayTimer += Time.deltaTime;
            if (shootDelayTimer >= shootDelay)
            {
                shootDelayTimer = 0;
                canShoot = true;
            }
        }

        if (canExit.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("al menos lo intentara??");
            canExit.SetActive(false);
            PanelDialogo.SetActive(false);
            attacks.SetActive(true);
            minimap.SetActive(true);
            textDialogo.text = "";
            canPressF = true;
            ResetDialog();
        }

        if (isCrouching || isClimbing && !canMove)
        {
            delayToMoveTimer += Time.deltaTime;
            if (delayToMoveTimer >= delayToMove)
            {
                delayToMoveTimer = 0;
                canMove = true;
            }
        }

        //Sensor de caida.
        if (!atGround && rb.linearVelocity.y < -0.1f)
        {
            if (!heightDamage && rb.linearVelocity.y < -maxJumpSpeed)
            {
                heightDamage = true;
                uiController.ConsumeHealth();
            }

            Fall();
        }

        //Dash
        if (doDash)
        {
            canDash = false;
            canShoot = false;
            canJump = false;
            canAttack = false;

            // Guardar la posición antes de mover
            float previousX = transform.position.x;
            transform.position = Vector3.MoveTowards(
                transform.position,
                dashPosition,
                dashSpeed * Time.deltaTime
            );

            // Calcular la distancia movida en X en este frame
            float distanceMovedX = Mathf.Abs(transform.position.x - previousX);

            // Comprobar si el personaje está atascado en X
            if (distanceMovedX < stuckThreshold)
            {
                stuckFrames++;
                if (stuckFrames >= maxStuckFrames)
                {
                    ForceStopDash(); // Detener el dash si está atascado
                    return; // Salir del método para evitar más procesamiento
                }
            }
            else
            {
                stuckFrames = 0; // Reiniciar si se mueve lo suficiente en X
            }
        }

        if (canMove)
        {
            if (move.x < 0)
            {
                if (
                    lookingRight
                    && (currentSpeed == runSpeed || currentSpeed == changeDirSpeed)
                    && !isChangingDirection
                )
                {
                    ChangeRunDirection();
                }
                else if (!isChangingDirection)
                {
                    move = new Vector2(-1, 0);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    lookingRight = false;
                }
            }
            else if (move.x > 0)
            {
                if (
                    !lookingRight
                    && (currentSpeed == runSpeed || currentSpeed == changeDirSpeed)
                    && !isChangingDirection
                )
                {
                    ChangeRunDirection();
                }
                else if (!isChangingDirection)
                {
                    move = new Vector2(1, 0);
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    lookingRight = true;
                }
            }
            else if (move.y > 0 && isClimbing && !isRoofTouching)
            {
                move = new Vector2(0, 1);
            }
            else if (move.y < 0 && isClimbing && !isRoofTouching)
            {
                move = new Vector2(0, -1);
            }
            else
            {
                isMoving = false;
                if (isCrouching)
                {
                    crouchAnimator.SetBool("Walk", false);
                }
                else if (isClimbing)
                {
                    if (isVineTouching)
                    {
                        trepaAnimator.SetBool("Walk", false);
                    }
                    else if (isRoofTouching && isRoofCrouch)
                    {
                        colgadoAnimator.SetBool("CrouchWalk", false);
                    }
                    else if (isRoofTouching && !isRoofCrouch)
                    {
                        colgadoAnimator.SetBool("Walk", false);
                    }
                    else
                    {
                        climbAnimator.SetBool("WalkUp", false);
                        climbAnimator.SetBool("WalkDown", false);
                    }
                }
                else
                {
                    normalAnimator.SetBool("Walk", false);
                    normalAnimator.SetBool("Run", false);
                }
                return;
            }

            if (!stopWalls)
            {
                Vector2 c = new Vector2(0, move.y) * currentSpeed * Time.deltaTime;
                transform.Translate(c, Space.World);

                Vector2 m = new Vector2(move.x, 0) * currentSpeed * Time.deltaTime;

                transform.Translate(m, Space.World);
                isMoving = true;
            }

            //Animaciones de movimiento.
            if (isCrouching)
            {
                crouchAnimator.SetBool("Walk", true);
            }
            else if (isClimbing && move.y > 0)
            {
                if (isVineTouching)
                {
                    trepaAnimator.SetBool("Walk", true);
                }
                else
                {
                    climbAnimator.SetBool("WalkUp", true);
                    climbAnimator.SetBool("WalkDown", false);
                }
            }
            else if (isClimbing && move.y < 0)
            {
                if (isVineTouching)
                {
                    trepaAnimator.SetBool("Walk", true);
                }
                else
                {
                    climbAnimator.SetBool("WalkUp", false);
                    climbAnimator.SetBool("WalkDown", true);
                }
            }
            else if (isClimbing && isRoofTouching && isRoofCrouch)
            {
                colgadoAnimator.SetBool("CrouchWalk", true);
            }
            else if (isClimbing && isRoofTouching && !isRoofCrouch)
            {
                colgadoAnimator.SetBool("Walk", true);
            }
            else if (currentSpeed == moveSpeed)
            {
                if(normalAnimator.enabled == false)
                {
                    Debug.Log("WALK SHOOT");
                    botAnimator.SetBool("Land", false);
                    botAnimator.SetBool("Walk", true);
                }
                else
                {
                    normalAnimator.SetBool("Walk", true);
                    normalAnimator.SetBool("Run", false);
                }
                    
            }
            else if (currentSpeed == runSpeed || canDecelerate || canAccelerate)
            {
                if (normalAnimator.enabled == false)
                {
                    botAnimator.SetBool("Land", false);
                    botAnimator.SetBool("Walk", false);
                    botAnimator.SetBool("Run", true);
                }
                else
                {
                    normalAnimator.SetBool("Walk", false);
                    normalAnimator.SetBool("Run", true);
                }
            }
        }

    }

    private float GetCurrentTopClipLengthOrFallback()
    {
        if (topAnimator == null)
            return splitShootSafetyTime;

        var stateInfo = topAnimator.GetCurrentAnimatorStateInfo(0);
        // Evita 0 si todavía no ha actualizado el state este frame.
        var len = stateInfo.length;
        return (len > 0.01f) ? len : splitShootSafetyTime;
    }

    private bool CanUseSplitShoot()
    {
        return !isCrouching && !isClimbing && !isRoofCrouch;
    }

    private void UpdateLegsWhileSplitShooting()
    {
        if (botAnimator == null)
            return;

        if (!atGround)
        {
            legsNeedsLanding = true;

            botAnimator.SetBool("Walk", false);
            botAnimator.SetBool("Run", false);

            botAnimator.SetBool("Land", false);

            splitLegsStateRequested = null;
            return;
        }

        if (isMoving)
        {
            if (currentSpeed == runSpeed)
            {
                botAnimator.SetBool("Walk", false);
                botAnimator.SetBool("Run", true);
            }
            else
            {
                botAnimator.SetBool("Run", false);
                botAnimator.SetBool("Walk", true);
            }
        }
        else
        {
            botAnimator.SetBool("Walk", false);
            botAnimator.SetBool("Run", false);
        }

        splitLegsStateRequested = null;
    }

    private void TryPlayLegsState(string stateName)
    {
        if (botAnimator == null)
            return;

        if (string.Equals(splitLegsStateRequested, stateName, StringComparison.Ordinal))
            return; // evita spamear

        splitLegsStateRequested = stateName;
        botAnimator.Play(stateName, -1, 0f);
    }

    private void BeginSplitShoot(string topStateName)
    {
        if (!CanUseSplitShoot())
            return;

        if (normalSprite != null) normalSprite.SetActive(false);
        if (shootSprite != null) shootSprite.SetActive(true);

        if (topAnimator != null)
        {
            topAnimator.Play(topStateName, -1, 0f);
            topAnimator.SetBool("Land", false);
        }

        if (botAnimator != null)
        {
            botAnimator.SetBool("Land", false);
        }

        canMove = true;

        isSplitShooting = true;
        splitShootTimer = 0f;
        splitLegsStateRequested = null;

        UpdateLegsWhileSplitShooting();
    }

    private void EndSplitShoot()
    {
        isSplitShooting = false;
        splitShootTimer = 0f;
        splitLegsStateRequested = null;

        // Volver a sprite normal
        if (shootSprite != null) shootSprite.SetActive(false);
        if (normalSprite != null) normalSprite.SetActive(true);

        // Volver a idle vía Land=true
        if (topAnimator != null) topAnimator.SetBool("Land", true);
        if (botAnimator != null) botAnimator.SetBool("Land", true);
        normalAnimator.SetBool("Land", true);
    }

    //Salto
    void Jump()
    {
        if (doJump)
        {
            if (isClimbing)
            {
                rb.gravityScale = 1;
                climbSprite.SetActive(false);
                trepaSprite.SetActive(false);
                colgadoSprite.SetActive(false);
                normalSprite.SetActive(true);
                isClimbing = false;
                currentSpeed = moveSpeed;
            }
            else if (isAttacking)
            {
                meleeSprite.SetActive(false);
                normalSprite.SetActive(true);
                isAttacking = false;
                canMove = true;
            }
            enablePlatform = false;
            atGround = false;
            canDash = false;
            canAttack = false;
            numOfJumps = 1;

            //rb.velocity = Vector2.zero;
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpTime)
            {
                doJump = false;
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(new Vector2(0, firstJumpForce), ForceMode2D.Impulse);
            }

            if(isSplitShooting)
            {
                botAnimator.SetBool("Land", false);
                botAnimator.Play("Jump");
            }
            else
            {
                normalAnimator.Play("Jump");
                normalAnimator.SetBool("Land", false);
            }

            if (isRoofTouching)
            {
                doJump = false;
                rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
            }
        }
    }

    //Doble salto
    //Cuando se tengan las botas, antes no
    void SecondJump()
    {
        if (canDoubleJump == true)
        {
            if (canJump && numOfJumps == 1 && allowSecondJump)
            {
                enablePlatform = false;
                allowSecondJump = false;
                canJump = false;
                numOfJumps = 0;

                rb.linearVelocity = Vector2.zero;
                rb.AddForce(new Vector2(0, secondJumpForce), ForceMode2D.Impulse);
                isFalling = false;

                botAnimator.Play("DoubleJump");
            }
        }
    }

    //Caida
    void Fall()
    {
        if (!isFalling)
        {
            if (isClimbing)
            {
                dashSprite.SetActive(false);
                climbSprite.SetActive(false);
                trepaSprite.SetActive(false);
                colgadoSprite.SetActive(false);
                crouchSprite.SetActive(false);
                normalSprite.SetActive(true);
                isClimbing = false;
                currentSpeed = moveSpeed;
            }

            isFalling = true;
            atGround = false;
            canDash = false;
            canAttack = false;
            enablePlatform = true;

            if(isSplitShooting)
            {
                botAnimator.SetBool("Falling", true);
            }
            normalAnimator.SetBool("Land", false);
            normalAnimator.SetBool("Falling", true);
        }
    }

    //Restablece los saltos
    void RestartJump()
    {
        rb.linearVelocity = Vector2.zero;
        numOfJumps = 2;
        allowSecondJump = false;
        jumpTimer = 0;
        if (!isCrouching)
        {
            canJump = true;
        }
    }

    //Acciones al tocar el suelo
    public void Grounding()
    {
        if (canGroundThisFrame)
        {
            rb.linearVelocity = Vector2.zero;
            RestartJump();
            atGround = true;
            canStandUp = true;
            canDash = true;
            canAttack = true;
            isFalling = false;
            heightDamage = false;

            if (isSplitShooting)
            {
                botAnimator.SetBool("Falling", false);
                botAnimator.SetBool("Land", true);
            }
            else
            {
                normalAnimator.SetBool("Land", true);
                normalAnimator.SetBool("Falling", false);
            }
        }
    }

    private IEnumerator ResetCanGroundThisFrame()
    {
        yield return new WaitForSeconds(0.1f);
        canGroundThisFrame = true;
    }

    private IEnumerator ResetCanClimb()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("RESETEANDO CAN CLIMB");
        canClimb = true;
    }

    void CheckJump()
    {
        if (atGround || isClimbing)
        {
            doJump = true;
        }
        else
        {
            SecondJump();
        }
    }

    //Comprueba si puede ejecutar el segundo salto
    void CheckSecondJump()
    {
        if (rb.linearVelocity.y != 0)
        {
            allowSecondJump = true;
        }
    }

    public void JumpWithPlatform(float jumpForce, bool platform)
    {
        if (platform)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            atGround = false;

            normalAnimator.Play("Jump");
            normalAnimator.SetBool("Land", false);
        }
        numOfJumps = 1;
        allowSecondJump = true;
    }

    //Disparo en horizontal
    void HorizontalShoot()
    {
        if (isAiming)
        {
            ShootHook(hShootPosition);
        }
        else if (canShoot)
        {
            if (isCrouching)
            {
                crouchAnimator.Play("HorizontalShoot", -1, 0);
                Instantiate(bullet, hShootPositionCrouch.position, hShootPositionCrouch.rotation);
                canMove = false;
                delayToMoveTimer = 0;
            }
            else if (isClimbing)
            {
                if (isRoofTouching && isMoving)
                {
                    colgadoAnimator.Play("Player_Colgado_Move_Shoot_Horizontal", -1, 0);
                    Instantiate(bullet, hShootPosition.position, hShootPosition.rotation);
                }
                else if (isRoofTouching && !isMoving)
                {
                    colgadoAnimator.Play("Player_Colgado_Shoot_Horizontal", -1, 0);
                    Instantiate(bullet, hShootPosition.position, hShootPosition.rotation);
                }
                else
                {
                    climbAnimator.Play("Player_Full_Climb_Shoot", -1, 0);
                    Instantiate(bullet, hShootPositionClimb.position, hShootPositionClimb.rotation);
                    canMove = false;
                    delayToMoveTimer = 0;
                }
            }
            else
            {
                if (CanUseSplitShoot())
                {
                    BeginSplitShoot("HorizontalShoot");
                }
                else
                {
                    normalAnimator.Play("HorizontalShoot", -1, 0);
                    normalAnimator.SetBool("Land", false);
                    canMove = true;
                }

                Instantiate(bullet, hShootPosition.position, hShootPosition.rotation);
            }
            canShoot = false;
        }
    }

    //Disparo en vertical
    void VerticalShoot()
    {
        if (isAiming)
        {
            ShootHook(vShootPosition);
        }
        else if (canShoot && !isCrouching && !isClimbing)
        {
            if (CanUseSplitShoot())
            {
                BeginSplitShoot("VerticalShoot");
            }
            else
            {
                normalAnimator.Play("VerticalShoot", -1, 0f);
                normalAnimator.SetBool("Land", false);
            }

            Instantiate(bullet, vShootPosition.position, vShootPosition.rotation);
            canShoot = false;
        }
    }

    //Disparo en diagonal hacia arriba
    void UpDiagonalShoot()
    {
        if (isAiming)
        {
            ShootHook(dShootPosition);
        }
        else if (canShoot && !isCrouching && !isRoofCrouch)
        {
            if (isClimbing)
            {
                climbAnimator.Play("player_Full_Climb_Shoot_Diagonal_UP", -1, 0);
                Instantiate(bullet, dUpShootPositionClimb.position, dUpShootPositionClimb.rotation);
                canMove = false;
                delayToMoveTimer = 0;
            }
            else
            {
                if (CanUseSplitShoot())
                {
                    BeginSplitShoot("DiagonalShoot");
                }
                else
                {
                    normalAnimator.Play("DiagonalShoot", -1, 0);
                    normalAnimator.SetBool("Land", false);
                }

                Instantiate(bullet, dShootPosition.position, dShootPosition.rotation);
                canShoot = false;
            }
        }
    }

    //Disparo en diagonal hacia abajo
    void DownDiagonalShoot()
    {
        if (canShoot && isClimbing)
        {
            if (isRoofTouching && isMoving)
            {
                colgadoAnimator.Play("Player_Colgado_Move_Shoot_Diagonal", -1, 0);
                Instantiate(
                    bullet,
                    dDownShootPositionRoof.position,
                    dDownShootPositionRoof.rotation
                );
            }
            else if (isRoofTouching && !isMoving)
            {
                colgadoAnimator.Play("Player_Colgado_Shoot_Down", -1, 0);
                Instantiate(
                    bullet,
                    vDownShootPositionRoof.position,
                    vDownShootPositionRoof.rotation
                );
            }
            else
            {
                climbAnimator.Play("player_Full_Climb_Shoot_Diagonal_Down", -1, 0);
                Instantiate(
                    bullet,
                    dDownShootPositionClimb.position,
                    dDownShootPositionClimb.rotation
                );
                canMove = false;
                delayToMoveTimer = 0;
            }
        }
    }

    //Comprueba la direccion de disparo
    void CheckShootDirection()
    {
        if (!isPointShooting)
        {
            if (isAttacking && !isAiming)
            {
                meleeSprite.SetActive(false);
                normalSprite.SetActive(true);
                isAttacking = false;
                normalAnimator.SetBool("Land", true);
            }

            if (shootDirection.y > 0)
            {
                if (shootDirection.x >= -0.5f && shootDirection.x <= 0.5f)
                {
                    VerticalShoot();
                }
                else
                {
                    UpDiagonalShoot();
                }
            }
            else if (shootDirection.y < 0 && !isCrouching)
            {
                DownDiagonalShoot();
            }
            else
            {
                HorizontalShoot();
            }
        }
    }

    //Disparar cuerda
    void ShootHook(Transform origin)
    {
        hit = Physics2D.Raycast(origin.position, origin.right.normalized, 2f);
        Debug.DrawLine(origin.position, origin.position + origin.right.normalized * 2f, Color.blue);

        if (hit.collider != null)
        {
            if (
                hit.collider.gameObject.CompareTag("Ground")
                || hit.collider.gameObject.CompareTag("Climb")
            )
            {
                hookOrigin = origin;
                line.enabled = true;
                line.SetPosition(0, origin.position);
                line.SetPosition(1, hit.point);
            }
        }
    }

    void Run()
    {
        if (!isCrouching && !stopWalls && !isJumping && !isFalling)
        {
            currentSpeed = runSpeed;
        }
    }

    //Cambiar de direccion al correr
    void ChangeRunDirection()
    {
        if (canDecelerate)
        {
            isChangingDirection = true;
            canJump = false;
            canShoot = false;
            changeDirSpeed = changeDirSpeed - 1f;
            currentSpeed = changeDirSpeed;
            canDecelerate = false;
        }

        if (!canDecelerate && !canAccelerate)
        {
            if (lookingRight)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            canAccelerate = true;
            isChangingDirection = false;
        }

        if (canAccelerate)
        {
            changeDirSpeed = changeDirSpeed + 1f;
            currentSpeed = changeDirSpeed;
            canAccelerate = false;
            canDecelerate = true;
            canJump = true;
            canShoot = true;
            lookingRight = !lookingRight;
        }
    }

    //Se agacha
    void Crouch()
    {
        if (!isDashing && !isClimbing && !isAttacking && atGround)
        {
            if (crouchValue > 0.35f || !canStandUp)
            {
                normalSprite.SetActive(false);
                dashSprite.SetActive(false);
                climbSprite.SetActive(false);
                trepaSprite.SetActive(false);
                colgadoSprite.SetActive(false);
                crouchSprite.SetActive(true);
                isCrouching = true;
                canJump = false;
                currentSpeed = crouchMoveSpeed;
            }
            else
            {
                CheckTop();
                Debug.Log("CheckTop en Crouch");
            }
        }
    }

    //Agacharse en el techo
    void RoofCrouch()
    {
        if (isClimbing && isRoofTouching)
        {
            isRoofCrouch = true;
            colgadoSprite.GetComponent<SpriteRenderer>().flipX = true;
            colgadoAnimator.SetBool("Crouch", true);
        }
    }

    //Dejar de agacharse en el techo
    void StopRoofCrouch()
    {
        if (isRoofCrouch)
        {
            isRoofCrouch = false;
            colgadoSprite.GetComponent<SpriteRenderer>().flipX = false;
            colgadoAnimator.SetBool("Crouch", false);
        }
    }

    //Calcula la posicion final para el desplazamiento
    void Dash()
    {
        if (canDash)
        {
            // Comprobar si hay una pared cerca en la dirección del dash, para evitar que el jugador se quede atascado en ella al iniciar el dash
            if (CheckPlayerTooCloseToWall())
            {
                return;
            }

            normalSprite.SetActive(false);
            crouchSprite.SetActive(false);
            climbSprite.SetActive(false);
            trepaSprite.SetActive(false);
            colgadoSprite.SetActive(false);
            meleeSprite.SetActive(false);
            dashSprite.SetActive(true);

            isAttacking = false;
            isDashing = true;
            doDash = true;
            canMove = false;
            canAttack = false;
            dashPosition = transform.position;
            if (lookingRight)
            {
                dashPosition.x += dashDistance;
            }
            else
            {
                dashPosition.x -= dashDistance;
            }
        }
    }

    private bool CheckPlayerTooCloseToWall()
    {
        RaycastHit2D hit;
        if (lookingRight)
        {
            hit = Physics2D.Raycast(dashChecker.position, Vector2.right, dashCheckerRadius, LayerMask.GetMask("Walls"));
        }
        else
        {
            hit = Physics2D.Raycast(dashChecker.position, Vector2.left, dashCheckerRadius, LayerMask.GetMask("Walls"));
        }
        return hit.collider != null;
    }

    //Escala
    public void Climb()
    {
        if (canClimb == true)
        {
            //Debug.Log("canClimb: " + canClimb + " Time: " + Time.time);
            if ((isWallTouching || isVineTouching) && !atGround || isRoofTouching)
            {
                normalSprite.SetActive(false);
                crouchSprite.SetActive(false);
                dashSprite.SetActive(false);
                if (isVineTouching)
                {
                    climbSprite.SetActive(false);
                    trepaSprite.SetActive(true);
                }
                else if (isRoofTouching)
                {
                    isRoofClimbing = true;
                    colgadoSprite.SetActive(true);
                }
                else
                {
                    trepaSprite.SetActive(false);
                    isWallClimbing = true;
                    climbSprite.SetActive(true);
                }
                isClimbing = true;
                isFalling = false;
                RestartJump();
                canDash = false;
                canAttack = false;

                rb.linearVelocity = Vector3.zero;

                rb.gravityScale = 0.01f;
                currentSpeed = climbMoveSpeed;
            }
        }
    }

    //Cambiar animación, colgarse o escalar
    void ChangeWallRoofClimb()
    {
        if (isWallClimbing && isRoofTouching)
        {
            isWallClimbing = false;
            isRoofClimbing = true;
            colgadoSprite.SetActive(true);
            climbSprite.SetActive(false);
            isWallTouching = false;
        }
        else if (isRoofClimbing && isWallTouching)
        {
            isWallClimbing = true;
            isRoofClimbing = false;
            colgadoSprite.SetActive(false);
            climbSprite.SetActive(true);
            isRoofTouching = false;
            Debug.Log("Escalo");
        }
    }

    void AimRope()
    {
        if (ropeDirection.y > 0)
        {
            if (ropeDirection.x >= -0.5f && ropeDirection.x <= 0.5f)
            {
                meleeAnimator.SetBool("LoadRopeD", false);
                meleeAnimator.SetBool("LoadRopeV", true);
            }
            else if (meleeAnimator.GetBool("LoadRopeV"))
            {
                meleeAnimator.SetBool("LoadRopeV", false);
                meleeAnimator.SetBool("LoadRopeD", true);
            }
            else if (meleeAnimator.GetBool("LoadRopeH"))
            {
                meleeAnimator.SetBool("LoadRopeH", false);
                meleeAnimator.SetBool("LoadRopeD", true);
            }
        }
        else
        {
            meleeAnimator.SetBool("LoadRopeD", false);
            meleeAnimator.SetBool("LoadRopeH", true);
        }

        LoadRope();
    }

    //Cargar longitud cuerda
    void LoadRope()
    {
        if (uiController.currentEnergy > 0)
        {
            loadRopeTimer += Time.deltaTime;
            if (loadRopeTimer >= loadRopeTime)
            {
                if (ropeLength < maxRopeLength)
                {
                    ropeLength++;
                }
                loadRopeTimer = 0;
                uiController.ConsumeEnergy();
            }
        }
    }

    //Ataque melee
    public void Attack()
    {
        if (canAttack && uiController.currentEnergy > 0)
        {
            canAttack = false;
            isAttacking = true;
            canMove = false;
            normalSprite.SetActive(false);
            crouchSprite.SetActive(false);
            dashSprite.SetActive(false);
            climbSprite.SetActive(false);
            trepaSprite.SetActive(false);
            colgadoSprite.SetActive(false);
            meleeSprite.SetActive(true);
            if (numAttack < attack.Length - 1)
            {
                Debug.Log(attack[numAttack]);
                meleeAnimator.Play(attack[numAttack], -1, 0);
                uiController.ConsumeEnergy();
            }
            else if (numAttack == attack.Length - 1)
            {
                meleeAnimator.SetBool("ShootRope", false);
                isAimingRope = true;

                //Comprobar direccion batcuerda
                if (ropeDirection.y > 0)
                {
                    if (ropeDirection.x >= -0.5f && ropeDirection.x <= 0.5f)
                    {
                        meleeAnimator.SetBool("LoadRopeV", true);
                    }
                    else
                    {
                        meleeAnimator.SetBool("LoadRopeD", true);
                    }
                }
                else
                {
                    meleeAnimator.SetBool("LoadRopeH", true);
                }
            }
            else
            {
                isAiming = true;
                uiController.ConsumeEnergy();
            }
        }
    }

    //Comprobar si la animacion de ataque ha terminado
    void CheckEndLoadAttack()
    {
        if (
            !meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo")
            && !meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_INICIO")
            && !meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_ACTIVO")
            && !meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Batcuerda_INICIO")
            && !meleeAnimator
                .GetCurrentAnimatorStateInfo(0)
                .IsName("Player_Melee_Batcuerda_Carga_Horizontal")
            && !meleeAnimator
                .GetCurrentAnimatorStateInfo(0)
                .IsName("Melee_Batcuerda_Carga_Diagonal")
            && !meleeAnimator
                .GetCurrentAnimatorStateInfo(0)
                .IsName("Player_Melee_Batcuerda_Carga_Arriba")
        )
        {
            if (attackTimer < meleeAnimator.GetCurrentAnimatorStateInfo(0).length)
            {
                attackTimer += Time.deltaTime;
            }
            else
            {
                EndAttack();
            }
        }
    }

    //Comprobar si la animacion de escudo o de cuerda ha terminado
    void CheckEndAttack()
    {
        if (isAiming)
        {
            isAiming = false;
            isHooking = true;
            EndAttack();
        }
        else
        {
            if (
                meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo")
                || meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_INICIO")
                || meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Escudo_ACTIVO")
            )
            {
                meleeAnimator.SetBool("Shield_Off", true);
                if (attackTimer < meleeAnimator.GetCurrentAnimatorStateInfo(0).length)
                {
                    attackTimer += Time.deltaTime;
                }
                else
                {
                    EndAttack();
                }
            }
            else if (
                meleeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Melee_Batcuerda_INICIO")
                || meleeAnimator
                    .GetCurrentAnimatorStateInfo(0)
                    .IsName("Player_Melee_Batcuerda_Carga_Horizontal")
                || meleeAnimator
                    .GetCurrentAnimatorStateInfo(0)
                    .IsName("Melee_Batcuerda_Carga_Diagonal")
                || meleeAnimator
                    .GetCurrentAnimatorStateInfo(0)
                    .IsName("Player_Melee_Batcuerda_Carga_Arriba")
            )
            {
                isAimingRope = false;
                if (meleeAnimator.GetBool("LoadRopeH"))
                {
                    meleeAnimator.SetBool("LoadRopeH", false);
                    Instantiate(shootRope, hShootPosition.position, hShootPosition.rotation);
                }
                else if (meleeAnimator.GetBool("LoadRopeD"))
                {
                    meleeAnimator.SetBool("LoadRopeD", false);
                    Instantiate(shootRope, dShootPosition.position, dShootPosition.rotation);
                }
                else if (meleeAnimator.GetBool("LoadRopeV"))
                {
                    meleeAnimator.SetBool("LoadRopeV", false);
                    Instantiate(shootRope, vShootPosition.position, vShootPosition.rotation);
                }
                meleeAnimator.SetBool("ShootRope", true);
            }
        }
    }

    //Finalizar ataque
    void EndAttack()
    {
        isAttacking = false;
        canAttack = true;
        canMove = true;
        attackTimer = 0;
        normalSprite.SetActive(true);
        crouchSprite.SetActive(false);
        dashSprite.SetActive(false);
        climbSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        meleeSprite.SetActive(false);
        //topAnimator.SetBool("Land", true);
        //botAnimator.SetBool("Land", true);
        normalAnimator.SetBool("Land", true);
    }

    //Cambiar ataque melee
    void ChangeAttack()
    {
        if (numAttack < attack.Length - 1)
        {
            numAttack++;
        }
        else
        {
            numAttack = 0;
        }

        uiController.NextMeleeAttack();
    }

    //Generar batcuerda
    public void CreateRope(Transform transform, string tag)
    {
        GameObject anchorObject;

        if (tag == "Roof")
        {
            anchorObject = Instantiate(anchor, transform.position, anchor.transform.rotation);
        }
        else
        {
            Quaternion anchorRotation = Quaternion.AngleAxis(270, Vector3.forward);
            anchorObject = Instantiate(anchor, transform.position, anchorRotation);
        }

        GameObject ropeElement = Instantiate(
            rope,
            anchorObject.transform.position,
            rope.transform.rotation
        );
        for (int i = 1; i < ropeLength; i++)
        {
            Vector3 position = new Vector3(
                ropeElement.transform.position.x,
                ropeElement.transform.position.y
                    - (ropeElement.GetComponent<SpriteRenderer>().sprite.bounds.size.y),
                ropeElement.transform.position.z
            );
            ropeElement = Instantiate(rope, position, rope.transform.rotation);
        }
        ropeLength = 1;
    }

    void SwitchToNormalSprite()
    {
        if (!isDashing)
        {
            crouchValue = 0;
            canMove = true;
            canJump = true;
            canAttack = true;
            crouchSprite.SetActive(false);
            dashSprite.SetActive(false);
            climbSprite.SetActive(false);
            trepaSprite.SetActive(false);
            colgadoSprite.SetActive(false);
            normalSprite.SetActive(true);
            isCrouching = false;
            currentSpeed = moveSpeed;
            //topAnimator.SetBool("Land", true);
            //botAnimator.SetBool("Land", true);
            normalAnimator.SetBool("Land", true);
        }
    }

    void CheckTop()
    {
        if (canStandUp && controls.Gameplay.Crouch.phase != InputActionPhase.Started && !isClimbing)
        {
            SwitchToNormalSprite();
        }
        else
        {
            Crouch();
        }
    }

    void ForceStopDash()
    {
        var posY = transform.position.y;
        isDashing = false;
        doDash = false;
        canMove = true;
        canDash = true;
        canShoot = true;
        canJump = true;
        canStandUp = true;
        //topAnimator.transform.localPosition = Vector3.zero;
        //topAnimator.SetBool("Land", true);
        normalAnimator.transform.localPosition = Vector3.zero;
        normalAnimator.SetBool("Land", true);
        CheckTop();
        Debug.Log("CheckTop en ForceStopDash");
        SwitchToNormalSprite();
        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
    }

    public void GetDamage()
    {
        sr = GetComponentsInChildren<SpriteRenderer>();

        uiController.ConsumeHealth();

        if (uiController.currentHealth > 0)
        {
            isImmune = true;
            immuneTimer = 0f;
            blinkTimer = 0f;
            foreach (var r in sr)
                r.enabled = false;
            return;
        }

        if (uiController.lifes > 1)
        {
            uiController.lifes--;
            uiController.lifesText.text = uiController.lifes.ToString();

            uiController.currentHealth = uiController.maxHealth;
            uiController.healthSlider.value = uiController.maxHealth;
            uiController.healthText.text = $"{uiController.maxHealth}/{uiController.maxHealth}";
            return;
        }

        StartCoroutine(DeathAndRespawn());
    }

    private IEnumerator DeathAndRespawn()
    {
        canMove = false;
        canAttack = false;

        normalSprite.SetActive(false);
        fullSprite.SetActive(false);
        crouchSprite.SetActive(false);
        dashSprite.SetActive(false);
        climbSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        meleeSprite.SetActive(false);

        muerteSprite.SetActive(true);
        muerteAnimator.Play("Player_Muerte");

        Vignette vig;
        if (volume.profile.TryGet<Vignette>(out vig))
            vig.intensity.value = 1f;

        float duration = muerteAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        transform.position = CheckpointManager.GetCheckpoint();

        muerteSprite.SetActive(false);
        normalSprite.SetActive(true);
        fullSprite.SetActive(true);

        uiController.currentHealth = uiController.maxHealth;
        uiController.healthSlider.value = uiController.maxHealth;
        uiController.healthText.text = $"{uiController.maxHealth}/{uiController.maxHealth}";

        canMove = true;
        canAttack = true;
    }

    void DamageBlink()
    {
        sr = GetComponentsInChildren<SpriteRenderer>();
        immuneTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkTime)
        {
            for (int i = 0; i < sr.Length; i++)
            {
                sr[i].enabled = !sr[i].enabled;
            }
            blinkTimer = 0;
        }

        if (immuneTimer >= immuneTime)
        {
            for (int i = 0; i < sr.Length; i++)
            {
                sr[i].enabled = true;
            }
            isImmune = false;
            immuneTimer = 0; // Reiniciar
            blinkTimer = 0;
        }
    }

    private void Talk()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && isNearNPC && canPressF)
        {
            PanelDialogo.SetActive(true);
            canPressF = false;
            if (PanelDialogo == true)
            {
                StartCoroutine("DialogSequence");
                canAccelerate = false;

                canAttack = false;
                canDash = false;
                canMove = false;
                canJump = false;
                canMove = false;
                canShoot = false;
            }
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            canStandUp = false;
            if (isHooking)
            {
                stopHook = true;
            }
        }

        if (collision.transform.tag == "EnemyProjectile")
        {
            if (!isImmune)
            {
                GetDamage();
            }
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "EnemyAttack")
        {
            //if (!isImmune) { GetDamage(); }
        }

        if (
            collision.transform.tag == "Ammunition1"
            || collision.transform.tag == "Ammunition2"
            || collision.transform.tag == "Ammunition3"
        )
        {
            Destroy(collision.gameObject);
            uiController.GetAmmunition(collision.transform.tag);
        }

        if (collision.gameObject.CompareTag("Treasure"))
        {
            collision.gameObject.GetComponent<Chest>().OpenChest();
        }

        if (collision.transform.tag == "Armor")
        {
            armorItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "Weapon")
        {
            weaponItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "Ammunition")
        {
            ammunitionItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "Melee")
        {
            meleeItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "Consumable")
        { //nada
        }
        else if (collision.transform.tag == "Ability")
        {
            canClimb = true;
            abilityItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "Botas")
        {
            canDoubleJump = true;
            abilityItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "Mission")
        {
            missionItmes++;
            Destroy(collision.gameObject);
        }
        else if (collision.transform.tag == "NPC")
        {
            isNearNPC = true;
            textPulsaF.SetActive(true);
        }
        else if (collision.transform.tag == "Traductor")
        {
            Traductor traductor = FindFirstObjectByType<Traductor>();
            traductor.isActiveTranslate = true;
            uiController.ActiveIconTranslate();

            Destroy(collision.transform.parent.gameObject);
        }
        else if (collision.transform.tag == "HealthPotion")
        {
            //uiController.RecoverHealth();
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            if (isCrouching || isDashing)
            {
                canStandUp = true;
                CheckTop();
                Debug.Log("CheckTop en OnTriggerExit2D");
            }
        }

        if (collision.transform.tag == "NPC")
        {
            isNearNPC = false;
            canPressF = true;
            textPulsaF.SetActive(false);
            PanelDialogo.SetActive(false);
            attacks.SetActive(true);
            minimap.SetActive(true);
            canExit.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Grounding();
        }
        if (
            (collision.transform.tag == "Ground" || collision.transform.tag == "Consumable")
            && isDashing
        )
        {
            Debug.Log("cortando");
            Debug.Log("Position Y: " + transform.position.y);

            ForceStopDash();
        }
    }

    private void OnTalkPerformed(InputAction.CallbackContext context)
    {
        if (isNearNPC == true)
        {
            Talk();
            isFalling = false;
            normalAnimator.SetBool("Falling", false);
        }
    }

    public Vector3 GetTransform() => transform.position;

    IEnumerator DialogSequence()
    {
        normalAnimator.SetBool("Run", false);
        normalAnimator.SetBool("Walk", false);
        textDialogo.text = "";
        attacks.SetActive(false);
        minimap.SetActive(false);
        string texto = string.Empty;

        if (!uiController.translateIcon.activeSelf)
        {
            texto = "⏚⟟⟒⋏⎐⟒⋏⟟⎅⍜ ⏃ ⟒⌇⏁⟒ ⌿⌰⏃⋏⟒⏁⏃ ⌇⎍ ⋏⏃⎐⟒ ⌇⟒ ⟒⌇⏁⍀⟒⌰⌰⍜";
        }
        else
        {
            texto = "Bienvenido a este planeta, su nave se estrelló. ";
        }
        contador = 0;
        while (contador < texto.Length)
        {
            textDialogo.text += texto[contador];
            contador++;
            yield return new WaitForSeconds(0.03f);
        }
        if (contador >= texto.Length)
        {
            canExit.SetActive(true);
        }
        else
        {
            canExit.SetActive(false);
        }

        canMove = true;
        canJump = true;
    }

    void ResetDialog()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && isNearNPC)
        {
            contador = 0;
            StartCoroutine(DialogSequence());
        }
    }

    public void ExecuteSelectedAttack(int index)
    {
        //metodo de rueda de selector de habilidades
        numAttack = index;

        // Limpiamos cualquier estado de ataque anterior:
        isAttacking = false;
        canAttack = true;
        canMove = true;
        attackTimer = 0f;

        Attack();
    }

    public void SetAtGround(bool value)
    {
        atGround = value;
    }

    public bool IsIdleForBarks()
    {
        if(!isMoving && !isJumping && !isFalling && !isDashing && !isClimbing && !isAttacking)
        {
            return true;
        }
        return false;
    }
}