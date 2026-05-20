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

public class PlayerController : MonoBehaviour
{
    public static event Action OnPlayerInteract;
    public static event Action<float> OnVineClimbFinishStarted;
    public static event Action<float, float> OnVineClimbFinishEnded; // teleportY, teleportX
    public static event Action OnPlayerDeath;
    [Header("Testing")]
    public float currentSpeed;

    [Header("UI")]
    public UIController uiController;

    [Header("Character Attributes")]
    public float blinkTimer;
    public float blinkTime;
    public float immuneTimer;
    public float immuneTime;
    private SpriteRenderer[] sr;
    public bool isImmune;
    public bool isDying = false;
    public bool isProtected = false;

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

    [Header("Movement")]
    public float moveSpeed = 1;
    public float runSpeed = 2.5f;
    public bool canRun = true;
    public Vector2 move;
    public bool lookingRight = true;
    public bool canMove;
    public bool isMoving;
    public bool canStandUp = true;
    public bool stopWalls = false;
    public bool wallOnLeft = false;
    public bool wallOnRight = false;
    // isChangingDirection kept for PlayerMainAttack compatibility, always false now
    public bool isChangingDirection = false;

    [Header("Jump")]
    public float firstJumpForce;
    public float secondJumpForce;
    public bool canJump = true;
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
    private bool justWallJumped = false;
    public bool canDoubleJump = false;
    public bool isJumping;

    public float wallClimbFinalImpulseMultiplierForce = 2f;
    public float vineClimbFinalImpulseMultiplierForce = 1.5f;
    [SerializeField] private float airAcceleration = 20f;

    [Header("Split Shoot (Torso/Legs)")]
    [SerializeField] private float splitShootSafetyTime = 0.2f;
    public bool isSplitShooting;
    private float splitShootTimer;
    private string splitLegsStateRequested;
    private SpriteRenderer botSpriteRenderer;

    [Header("Crouch")]
    public float crouchMoveSpeed;
    public bool isCrouching;
    private float crouchValue;
    private bool canCrouch = true;

    [Header("Dash")]
    public float dashDuration;
    public float dashSpeed;
    private bool doDash;
    public bool canDash;
    public bool isDashing;
    private float dashTimer = 0;
    public Transform dashChecker;
    public float dashCheckerRadius;

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
    public bool isFinishingClimbing = false;
    public ClimbZone currentClimbZone;
    public VineZone currentVineZone;
    private bool canGroundThisFrame = true;
    [SerializeField] private float climbFinishHeight = 1f;
    [SerializeField] private float climbFinishDuration = 0.3f;
    [SerializeField] private float failClimbHangDuration = 0.5f;
    [SerializeField] private float returnToNormalAfterFailDuration = 0.5f;
    [SerializeField] private bool pioletDebug;

    public bool CanWallClimb => Piolet.piolet || pioletDebug;

    [Header("Items")]
    public int armorItems;
    public int weaponItems;
    public int ammunitionItems;
    public int meleeItems;
    public int consumableItems;
    public int abilityItems;
    public int missionItmes;

    [Header("Sprites")]
    public Transform spritesParent;
    public GameObject fullSprite;
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

    public Piolet voidPiolet;

    [Header("Respawn/Checkpoint")]
    [HideInInspector] public bool skipDeathAnimation = false;

    [Space(5)]
    [Header("Inventory")]
    public Inventory inventory;

    [Space(5)]
    [Header("Post Processing")]
    public Volume volume;

    [Header("Audio")]
    [SerializeField] public PlayerAudioManager playerAudio;

    [HideInInspector] public bool shootingRight;
    [HideInInspector] public bool shouldChangeLookingDirection;
    [HideInInspector] public string pointShootLegsClip;
    [HideInInspector] public bool isSpellActive;

    private PlayerSpell playerSpell;
    private PlayerMainAttack playerMainAttack;


    // ── LIFECYCLE ─────────────────────────────────────────────────────────────

    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Jump.performed += ctx => CheckJump();
        controls.Gameplay.Jump.canceled += ctx => CheckSecondJump();
        controls.Gameplay.Jump.canceled += ctx => doJump = false;
        controls.Gameplay.Crouch.performed += ctx => crouchValue = ctx.ReadValue<float>();
        controls.Gameplay.Crouch.performed += ctx => Crouch();
        controls.Gameplay.Crouch.canceled += ctx =>
        {
            crouchValue = 0;
            if (atGround && !isClimbing)
                CheckTop();
            else
            {
                isCrouching = false;
                crouchSprite.SetActive(false);
                if (!isSplitShooting && !isSpellActive)
                    normalSprite.SetActive(true);
            }
        };
        controls.Gameplay.Dash.performed += ctx => Dash();
        controls.Gameplay.Run.performed += ctx => Run();
        controls.Gameplay.Run.canceled += ctx => currentSpeed = moveSpeed;
        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
        controls.Gameplay.Climb.performed += ctx => Climb();
        // COLGADO DISABLED
        //controls.Gameplay.RoofCrouch.performed += ctx => RoofCrouch();
        //controls.Gameplay.RoofCrouch.canceled += ctx => StopRoofCrouch();
        controls.Gameplay.Interact.performed += ctx => OnPlayerInteract?.Invoke();
    }

    private void Start()
    {
        uiController = GameObject.Find("UI").GetComponentInChildren<UIController>();

        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed;
        muerteSprite.SetActive(false);
        canMove = true;

        playerSpell = GetComponent<PlayerSpell>();
        playerMainAttack = GetComponent<PlayerMainAttack>();

        if (shootSprite != null)
            shootSprite.SetActive(false);

        if (botAnimator != null)
            botSpriteRenderer = botAnimator.GetComponent<SpriteRenderer>();

        sr = GetComponentsInChildren<SpriteRenderer>();
        UpdateFacingScale();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        if (isDying) return;
        UpdateJumpingFlag();
        HandleRunSpeedGuard();
        HandleSplitShoot();
        HandleClimbStateTransitions();
        HandleImmunity();

        if (canJump) Jump();
        HandleFallDetection();
        HandleDash();
        HandleMovement();
    }


    // ── FACING ────────────────────────────────────────────────────────────────

    private void UpdateFacingScale()
    {
        if (spritesParent != null)
            spritesParent.localScale = lookingRight ? Vector3.one : new Vector3(-1, 1, 1);
    }

    public void SetFacingDirection(bool facingRight)
    {
        lookingRight = facingRight;
        UpdateFacingScale();
    }


    // ── MOVEMENT ──────────────────────────────────────────────────────────────

    private void HandleMovement()
    {
        if (!canMove || (DialogueManager.I != null && DialogueManager.I.IsOpen)) return;

        bool hasInput = ResolveMovementInput();
        if (!hasInput) return;

        ApplyMovementVelocity();
        UpdateMovementAnimations();
    }

    private bool ResolveMovementInput()
    {
        if (move.x != 0)
        {
            bool pressingOppositeToFacing = (move.x > 0) != lookingRight;
            bool isRunning = currentSpeed == runSpeed;
            bool isPointShooting = playerMainAttack != null && playerMainAttack.isPointShooting;

            if (pressingOppositeToFacing && isRunning && !isPointShooting && !isClimbing)
            {
                ChangeRunDirection();
            }
            else
            {
                move = new Vector2(Mathf.Sign(move.x), 0);
                if (!isSplitShooting && !(isClimbing && !isRoofTouching))
                {
                    lookingRight = move.x > 0;
                    UpdateFacingScale();
                }
            }
            return true;
        }

        if (move.y != 0 && isClimbing && !isRoofTouching)
        {
            move = new Vector2(0, Mathf.Sign(move.y));
            return true;
        }

        // Sin input: parar movimiento y animaciones
        isMoving = false;
        if (!justWallJumped)
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        StopMovementAnimations();
        return false;
    }

    private void StopMovementAnimations()
    {
        if (isCrouching)
        {
            crouchAnimator.SetBool("Walk", false);
        }
        else if (isClimbing)
        {
            if (isVineTouching)
                trepaAnimator.SetBool("Walk", false);
            // COLGADO DISABLED
            //else if (isRoofTouching && isRoofCrouch)
            //    colgadoAnimator.SetBool("CrouchWalk", false);
            //else if (isRoofTouching)
            //    colgadoAnimator.SetBool("Walk", false);
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
    }

    private void ApplyMovementVelocity()
    {
        // No tocar move.x por que da información del input, si hay que tocar algo se toca inputX
        float inputX = isClimbing ? 0f : move.x;

        // En el aire siempre tienes que poder cambiar de dirección
        if (!atGround && !isClimbing && !isSplitShooting && inputX != 0)
        {
            lookingRight = inputX > 0;
            UpdateFacingScale();
        }

        bool blockedByWall = (inputX > 0 && wallOnRight) || (inputX < 0 && wallOnLeft);
        float moveX = blockedByWall ? 0f : inputX;

        if (!atGround && !isClimbing)
        {
            // Si estas moviendote rápido por alguna razon, no sobreescribir esa velocidad con la de moverte normal solo por pulsar una dirección
            float targetX = moveX * currentSpeed;
            bool sameDir = Mathf.Sign(moveX) == Mathf.Sign(rb.linearVelocityX);
            bool fasterThanInput = Mathf.Abs(rb.linearVelocityX) > Mathf.Abs(targetX);
            float newX = (sameDir && fasterThanInput)
                ? rb.linearVelocityX
                : Mathf.MoveTowards(rb.linearVelocityX, targetX, airAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocityY);
        }
        else if (isWallTouching && !isClimbing)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = new Vector2(moveX, move.y) * currentSpeed;
        }

        isMoving = true;
    }

    private void UpdateMovementAnimations()
    {
        if (isCrouching)
        {
            crouchAnimator.SetBool("Walk", true);
            return;
        }

        if (isClimbing)
        {
            if (isVineTouching)
            {
                trepaAnimator.SetBool("Walk", true);
            }
            // COLGADO DISABLED
            //else if (isRoofTouching)
            //{
            //    if (isRoofCrouch)
            //        colgadoAnimator.SetBool("CrouchWalk", true);
            //    else
            //        colgadoAnimator.SetBool("Walk", true);
            //}
            else // wall
            {
                climbAnimator.SetBool("WalkUp", move.y > 0);
                climbAnimator.SetBool("WalkDown", move.y < 0);
            }
            return;
        }

        // Ground: walk vs run
        bool isRunning = currentSpeed != moveSpeed;
        if (normalAnimator.enabled == false)
        {
            botAnimator.SetBool("Land", false);
            botAnimator.SetBool("Walk", !isRunning);
            botAnimator.SetBool("Run", isRunning);
        }
        else
        {
            normalAnimator.SetBool("Walk", !isRunning);
            normalAnimator.SetBool("Run", isRunning);
        }
    }

    void Run()
    {
        if (!isCrouching && !stopWalls && !isJumping && !isFalling && !isSplitShooting && !isWallTouching && canRun)
            currentSpeed = runSpeed;
    }

    // Cambia hacia donde está mirando el personaje
    void ChangeRunDirection()
    {
        lookingRight = !lookingRight;
        UpdateFacingScale();
    }

    private void HandleRunSpeedGuard()
    {
        if (!canRun && currentSpeed == runSpeed)
            currentSpeed = moveSpeed;
    }


    // ── JUMP ──────────────────────────────────────────────────────────────────

    private void UpdateJumpingFlag()
    {
        isJumping = !atGround && rb.linearVelocity.y > 0f;
    }

    void Jump()
    {
        if (!doJump) return;

        if (isClimbing)
        {
            JumpFromClimb();
            return;
        }

        if (isSpellActive)
            playerSpell?.ForceEndAttack();



        enablePlatform = false;
        atGround = false;
        canDash = false;
        numOfJumps = 1;

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

        if (isSplitShooting)
        {
            botAnimator.SetBool("Land", false);
            botAnimator.Play("Jump");
        }
        else
        {
            normalAnimator.Play("Jump");
            normalAnimator.SetBool("Land", false);
        }

        // No saltar si te estas dando con la cabeza contra el techo
        if (isRoofTouching)
        {
            doJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
        }
    }

    private void JumpFromClimb()
    {
        canMove = true;
        rb.gravityScale = 1;
        ExitClimbToNormal();
        isClimbing = false;
        currentSpeed = moveSpeed;
        canClimb = false;
        StartCoroutine(ResetCanClimb());

        float wallPushX;
        if (isWallClimbing)
        {
            wallPushX = lookingRight ? -1f : 1f;    // aleja de la pared
            SetFacingDirection(!lookingRight);
        }
        else if (isVineTouching && move.x != 0)
        {
            wallPushX = Mathf.Sign(move.x);         // A = izquierda, D = derecha
            SetFacingDirection(move.x > 0);
        }
        else
        {
            wallPushX = 0f;                         // salta recto
        }

        justWallJumped = true;
        enablePlatform = false;
        atGround = false;
        canDash = false;
        numOfJumps = 1;
        doJump = false;
        jumpTimer = 0;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(wallPushX * firstJumpForce, firstJumpForce), ForceMode2D.Impulse);

        normalAnimator.Play("Jump");
        normalAnimator.SetBool("Land", false);
    }

    void SecondJump()
    {
        if (!canDoubleJump || !canJump || numOfJumps != 1 || !allowSecondJump) return;

        enablePlatform = false;
        allowSecondJump = false;
        canJump = false;
        numOfJumps = 0;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(0, secondJumpForce), ForceMode2D.Impulse);
        isFalling = false;

        botAnimator.Play("DoubleJump");
    }

    void CheckJump()
    {
        if (atGround || isClimbing)
            doJump = true;
        else
            SecondJump();
    }

    void CheckSecondJump()
    {
        allowSecondJump = rb.linearVelocity.y != 0;
    }

    public void JumpWithPlatform(float jumpForce, bool platform)
    {
        if (platform)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            atGround = false;
            canGroundThisFrame = false;
            StartCoroutine(ResetCanGroundThisFrame());

            normalAnimator.Play("Jump");
            normalAnimator.SetBool("Land", false);
        }
        numOfJumps = 1;
        allowSecondJump = true;
    }

    void RestartJump()
    {
        rb.linearVelocity = Vector2.zero;
        numOfJumps = 2;
        allowSecondJump = false;
        jumpTimer = 0;
        if (!isCrouching)
            canJump = true;
    }


    // ── FALL ──────────────────────────────────────────────────────────────────

    private void HandleFallDetection()
    {
        if (atGround || rb.linearVelocity.y >= -0.1f) return;

        if (!heightDamage && rb.linearVelocity.y < -maxJumpSpeed)
        {
            heightDamage = true;
            GetDamage();
        }

        Fall();
    }

    void Fall()
    {
        if (isFalling) return;

        if (isClimbing)
        {
            dashSprite.SetActive(false);
            crouchSprite.SetActive(false);
            ExitClimbToNormal();
            isClimbing = false;
            currentSpeed = moveSpeed;
        }

        isFalling = true;
        atGround = false;
        canDash = false;
        enablePlatform = true;

        if (isSplitShooting)
            botAnimator.SetBool("Falling", true);
        normalAnimator.SetBool("Land", false);
        normalAnimator.SetBool("Falling", true);
    }

    public void Grounding()
    {
        if (!canGroundThisFrame) return;

        rb.linearVelocity = Vector2.zero;
        justWallJumped = false;
        RestartJump();
        atGround = true;
        canStandUp = true;
        canDash = true;
        playerSpell?.OnGrounded();
        isFalling = false;
        heightDamage = false;

        if (!isCrouching && crouchSprite.activeSelf && !isSplitShooting && !isClimbing)
        {
            crouchSprite.SetActive(false);
            normalSprite.SetActive(true);
        }

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


    // ── DASH ──────────────────────────────────────────────────────────────────

    void Dash()
    {
        if (!canDash) return;
        if (CheckPlayerTooCloseToWall()) return;

        normalSprite.SetActive(false);
        crouchSprite.SetActive(false);
        climbSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        meleeSprite.SetActive(false);
        dashSprite.SetActive(true);

        isDashing = true;
        doDash = true;
        canMove = false;
        dashTimer = 0;
    }

    private void HandleDash()
    {
        if (!doDash) return;

        dashTimer += Time.deltaTime;
        canDash = false;
        canJump = false;

        rb.linearVelocity = new Vector2(dashSpeed * (lookingRight ? 1 : -1), 0);

        if (dashTimer > dashDuration)
            ForceStopDash();

        if (CheckPlayerTooCloseToWall())
            ForceStopDash();
    }

    void ForceStopDash()
    {
        var posY = transform.position.y;
        isDashing = false;
        doDash = false;
        canMove = true;
        canDash = true;
        canJump = true;
        canStandUp = true;
        normalAnimator.transform.localPosition = Vector3.zero;
        normalAnimator.SetBool("Land", true);
        CheckTop();
        SwitchToNormalSprite();
        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
    }

    private bool CheckPlayerTooCloseToWall()
    {
        Vector2 dir = lookingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(dashChecker.position, dir, dashCheckerRadius, LayerMask.GetMask("Walls")).collider != null;
    }


    // ── CLIMB ─────────────────────────────────────────────────────────────────

    private void HandleClimbStateTransitions()
    {
        HandleClimbDetachment();
        HandleClimbAutoAttach();
        ChangeWallRoofClimb();
    }

    private void HandleClimbDetachment()
    {
        if (isWallTouching || isRoofTouching || isVineTouching || isFinishingClimbing) return;

        // Solo se entra en este if el frame que dejas de estar tocando la pared/diana pero todavía estas escalando
        if (isClimbing)
        {
            if (move.y > 0)
            {
                // Te has subido al sitio
                isWallTouching = false;
                isVineTouching = false;
                canGroundThisFrame = false;
                canClimb = false;
                StartCoroutine(ResetCanGroundThisFrame());
                StartCoroutine(ResetCanClimb());
            }

            isWallClimbing = false;
            isRoofClimbing = false;
            ExitClimbToNormal();
            currentSpeed = moveSpeed;
        }

        rb.gravityScale = 1;
        isClimbing = false;
    }

    private void HandleClimbAutoAttach()
    {
        if (!isVineTouching && !isWallTouching) return;

        if (!doJump) Climb();
        if (!isClimbing && canClimb)
            rb.linearVelocity = Vector2.zero;
    }

    public void Climb()
    {
        if (!canClimb) return;

        // Esto es especificamente para cuando estas en una plataforma y pulsas s pa agarrarte a la diana que hay debajo
        if (isVineTouching && (rb.linearVelocity.y < -0.05f || isCrouching))
            atGround = false;

        bool canAttach = ((isWallTouching || isVineTouching) && !atGround) || isRoofTouching;
        if (!canAttach) return;
        if (isWallTouching && !isVineTouching && !CanWallClimb) return;

        justWallJumped = false;
        isCrouching = false;
        if (isSplitShooting) EndSplitShoot();
        normalSprite.SetActive(false);
        crouchSprite.SetActive(false);
        dashSprite.SetActive(false);

        if (isVineTouching)
        {
            climbSprite.SetActive(false);
            trepaSprite.SetActive(true);
            rb.gravityScale = 0.01f;
            // Para evitar que te enganches a dos metros de la diana
            if (currentVineZone != null)
            {
                Vector3 p = transform.position;
                transform.position = new Vector3(currentVineZone.GetReferencePointX(), p.y, p.z);
            }
        }
        // COLGADO DISABLED
        //else if (isRoofTouching)
        //{
        //    isRoofClimbing = true;
        //    colgadoSprite.SetActive(true);
        //    rb.gravityScale = 0.01f;
        //}
        else // wall
        {
            trepaSprite.SetActive(false);
            isWallClimbing = true;
            climbSprite.SetActive(true);
            rb.gravityScale = 0f;
        }

        isClimbing = true;
        isFalling = false;
        RestartJump();
        canDash = false;
        rb.linearVelocity = Vector2.zero;
        currentSpeed = climbMoveSpeed;
    }

    // COLGADO DISABLED 
    void ChangeWallRoofClimb()
    {
        //if (isWallClimbing && isRoofTouching)
        //{
        //    isWallClimbing = false;
        //    isRoofClimbing = true;
        //    colgadoSprite.SetActive(true);
        //    climbSprite.SetActive(false);
        //    isWallTouching = false;
        //}
        //else if (isRoofClimbing && isWallTouching)
        //{
        //    isWallClimbing = true;
        //    isRoofClimbing = false;
        //    colgadoSprite.SetActive(false);
        //    climbSprite.SetActive(true);
        //    isRoofTouching = false;
        //}
    }

    public void StartWallClimbing()
    {
        if (isSplitShooting) EndSplitShoot();
        normalSprite.SetActive(false);
        crouchSprite.SetActive(false);
        dashSprite.SetActive(false);
        shootSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        isWallClimbing = true;
        isClimbing = true;
        isFalling = false;
        rb.gravityScale = 0f;
        canDash = false;
        currentSpeed = climbMoveSpeed;
        climbSprite.SetActive(true);
        climbAnimator.SetTrigger("Start");
        doJump = false;
        RestartJump();
        canMove = false;
    }

    public void FailWallClimb()
    {
        StartCoroutine(FailWallClimbCoroutine());
    }

    private IEnumerator FailWallClimbCoroutine()
    {
        canClimb = false;
        canMove = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        normalSprite.SetActive(false);
        crouchSprite.SetActive(false);
        dashSprite.SetActive(false);
        shootSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        climbSprite.SetActive(true);
        climbAnimator.SetTrigger("Try");

        yield return new WaitForSeconds(failClimbHangDuration);

        rb.gravityScale = 1f;
        rb.linearVelocity += new Vector2(lookingRight ? -0.5f : 0.5f, 0);

        yield return new WaitForSeconds(returnToNormalAfterFailDuration);

        rb.linearVelocity -= new Vector2(lookingRight ? -0.5f : 0.5f, 0);

        ExitClimbToNormal();
        canMove = true;
        StartCoroutine(ResetCanClimb());
    }

    public void StartFinishVineClimb(float teleportY, float teleportX = 0f)
    {
        if (isFinishingClimbing) return;
        isFinishingClimbing = true;
        StartCoroutine(FinishVineClimbCoroutine(teleportY, teleportX));
    }

    private IEnumerator FinishVineClimbCoroutine(float teleportY, float teleportX)
    {
        canMove = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        int prevStateHash = trepaAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        trepaAnimator.SetTrigger("Finish");

        yield return null;

        // Espera a salir del estado anterior y a que termine la transición
        while (trepaAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == prevStateHash ||
               trepaAnimator.IsInTransition(0))
            yield return null;

        // Ahora sí estamos en el estado Finish: notificar cámara y esperar a que termine
        OnVineClimbFinishStarted?.Invoke(teleportY);
        while (trepaAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        float actualX = teleportX * (lookingRight ? 1f : -1f);
        OnVineClimbFinishEnded?.Invoke(teleportY, actualX);
        ExitClimbToNormal();
        transform.position += new Vector3(actualX, teleportY, 0);

        isClimbing = false;
        isVineTouching = false;
        isWallClimbing = false;
        rb.gravityScale = 1f;
        currentSpeed = moveSpeed;
        canMove = true;
        canClimb = false;
        StartCoroutine(ResetCanClimb());

        // Mantener isFinishingClimbing=true un step de física más para que
        // GroundChecker no lance TeleportToTop justo al aterrizar
        yield return new WaitForFixedUpdate();
        isFinishingClimbing = false;
    }

    public void StartFinishingClimb()
    {
        if (isFinishingClimbing) return;
        isFinishingClimbing = true;
        climbAnimator.SetTrigger("Finish");
        StartCoroutine(FinishClimbCoroutine());
    }

    private IEnumerator FinishClimbCoroutine()
    {
        canMove = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        float firstMovementTime = 1f;
        float restTime = 0.3f;

        float elapsed = 0f;
        float startY = transform.position.y;
        float targetY = startY + climbFinishHeight;
        float startX = transform.position.x;
        float targetX = startX + (lookingRight ? 0.1f : -0.1f);

        while (elapsed < firstMovementTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / climbFinishDuration);
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, targetY, t), transform.position.z);
            yield return null;
        }

        yield return new WaitForSeconds(restTime);

        while (elapsed < climbFinishDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / climbFinishDuration);
            transform.position = new Vector3(Mathf.Lerp(startX, targetX, t), Mathf.Lerp(startY, targetY, t), transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(targetX, targetY, transform.position.z);

        isFinishingClimbing = false;
        isClimbing = false;
        isWallClimbing = false;
        isRoofClimbing = false;
        rb.gravityScale = 1f;
        ExitClimbToNormal();
        currentSpeed = moveSpeed;
        canClimb = false;
        StartCoroutine(ResetCanClimb());
        canMove = true;
    }

    // Pasar de Climb a Normal
    private void ExitClimbToNormal()
    {
        climbSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        normalSprite.SetActive(true);
    }


    // ── CROUCH ────────────────────────────────────────────────────────────────

    void Crouch()
    {
        if (!(!isDashing && !isClimbing && !isSpellActive && atGround && canCrouch)) return;

        if (crouchValue > 0.35f || !canStandUp)
        {
            if (isSplitShooting)
                EndSplitShoot();

            normalSprite.SetActive(false);
            shootSprite.SetActive(false);
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
        }
    }

    // COLGADO DISABLED
    void RoofCrouch()
    {
        //if (isClimbing && isRoofTouching)
        //{
        //    isRoofCrouch = true;
        //    colgadoSprite.GetComponent<SpriteRenderer>().flipX = true;
        //    colgadoAnimator.SetBool("Crouch", true);
        //}
    }

    // COLGADO DISABLED
    void StopRoofCrouch()
    {
        //if (!isRoofCrouch) return;
        //isRoofCrouch = false;
        //colgadoSprite.GetComponent<SpriteRenderer>().flipX = false;
        //colgadoAnimator.SetBool("Crouch", false);
    }

    void CheckTop()
    {
        if (canStandUp && controls.Gameplay.Crouch.phase != InputActionPhase.Started && !isClimbing)
            SwitchToNormalSprite();
        else
            Crouch();
    }

    void SwitchToNormalSprite()
    {
        if (isDashing || isSpellActive) return;

        crouchValue = 0;
        canMove = true;
        canJump = true;
        playerSpell?.OnGrounded();
        crouchSprite.SetActive(false);
        dashSprite.SetActive(false);
        climbSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        normalSprite.SetActive(true);
        isCrouching = false;
        currentSpeed = moveSpeed;
        normalAnimator.SetBool("Land", true);
    }


    // ── SPLIT SHOOT ───────────────────────────────────────────────────────────

    private void HandleSplitShoot()
    {
        if (!isSplitShooting) return;

        splitShootTimer += Time.deltaTime;
        UpdateLegsWhileSplitShooting();

        if (splitShootTimer >= GetCurrentTopClipLengthOrFallback())
            EndSplitShoot();
    }

    public bool CanUseSplitShoot()
    {
        return !isCrouching && !isClimbing && !isRoofCrouch && !isSpellActive;
    }

    public void BeginSplitShoot(string topStateName)
    {
        if (!CanUseSplitShoot()) return;

        if (spritesParent != null)
            spritesParent.localScale = Vector3.one;

        if (normalSprite != null) normalSprite.SetActive(false);
        if (crouchSprite != null) crouchSprite.SetActive(false);
        if (shootSprite != null) shootSprite.SetActive(true);

        if (topAnimator != null)
        {
            topAnimator.Play(topStateName, -1, 0f);
            topAnimator.SetBool("Land", false);
        }

        if (botAnimator != null)
            botAnimator.SetBool("Land", false);

        canMove = true;
        isSplitShooting = true;
        splitShootTimer = 0f;
        splitLegsStateRequested = null;

        UpdateLegsWhileSplitShooting();
    }

    public void EndSplitShoot()
    {
        isSplitShooting = false;
        splitShootTimer = 0f;
        splitLegsStateRequested = null;
        pointShootLegsClip = null;

        if (shootSprite != null) shootSprite.SetActive(false);
        if (!isCrouching && normalSprite != null) normalSprite.SetActive(true);

        if (topAnimator != null) topAnimator.SetBool("Land", true);
        if (botAnimator != null) botAnimator.SetBool("Land", true);
        normalAnimator.SetBool("Land", true);

        topAnimator.transform.localPosition = Vector3.zero;
        botAnimator.transform.localPosition = Vector3.zero;

        if (botSpriteRenderer != null)
            botSpriteRenderer.flipX = false;

        lookingRight = shootingRight;
        UpdateFacingScale();
    }

    private void UpdateLegsWhileSplitShooting()
    {
        if (botAnimator == null) return;

        if (!atGround)
        {
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
                botAnimator.SetBool("WalkBack", false);
                botAnimator.SetBool("Run", true);
            }
            else
            {
                botAnimator.SetBool("Run", false);
                bool forward = (move.x > 0 && shootingRight) || (move.x < 0 && !shootingRight);
                botAnimator.SetBool("Walk", forward);
                botAnimator.SetBool("WalkBack", !forward);
            }

            if (playerMainAttack != null && playerMainAttack.isPointShooting && botSpriteRenderer != null)
                botSpriteRenderer.flipX = move.x < 0;
        }
        else
        {
            botAnimator.SetBool("Walk", false);
            botAnimator.SetBool("WalkBack", false);
            botAnimator.SetBool("Run", false);

            if (botSpriteRenderer != null)
                botSpriteRenderer.flipX = false;

            if (!string.IsNullOrEmpty(pointShootLegsClip))
                TryPlayLegsState(pointShootLegsClip);
        }

        splitLegsStateRequested = null;
    }

    private void TryPlayLegsState(string stateName)
    {
        if (botAnimator == null) return;
        if (botAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) return;

        splitLegsStateRequested = stateName;
        botAnimator.Play(stateName, -1, 0f);
    }

    private float GetCurrentTopClipLengthOrFallback()
    {
        if (topAnimator == null) return splitShootSafetyTime;
        var len = topAnimator.GetCurrentAnimatorStateInfo(0).length;
        return len > 0.01f ? len : splitShootSafetyTime;
    }


    // ── DAMAGE & DEATH ────────────────────────────────────────────────────────

    private void HandleImmunity()
    {
        if (isImmune) DamageBlink();
    }

    private void CancelAllActions()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;

        playerSpell?.ForceEndAttack();
        playerMainAttack?.SetEnabled(false);

        isDashing = false;
        doDash = false;
        isCrouching = false;
        isClimbing = false;
        isWallClimbing = false;
        isRoofClimbing = false;
        isFinishingClimbing = false;
        isFalling = false;
        isSplitShooting = false;
        isSpellActive = false;
        isImmune = false;
        doJump = false;
        move = Vector2.zero;
        canMove = false;
        canJump = false;
        canDash = false;

        RestoreSpriteRenderers();
        fullSprite.SetActive(false);
        dashSprite.SetActive(false);
        crouchSprite.SetActive(false);
        climbSprite.SetActive(false);
        trepaSprite.SetActive(false);
        colgadoSprite.SetActive(false);
        meleeSprite.SetActive(false);
        if (shootSprite != null) shootSprite.SetActive(false);
        normalSprite.SetActive(true);
    }

    public void GetDamage()
    {
        if (isDying) return;

        if (normalSprite.activeSelf)
            normalAnimator.SetTrigger("Hit");

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

        StopAllCoroutines();
        StartCoroutine(DeathAndRespawn());
    }

    void DamageBlink()
    {
        immuneTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkTime)
        {
            foreach (var r in sr)
                r.enabled = !r.enabled;
            blinkTimer = 0;
        }

        if (immuneTimer >= immuneTime)
        {
            foreach (var r in sr)
                r.enabled = true;
            isImmune = false;
            immuneTimer = 0;
            blinkTimer = 0;
        }
    }

    public void RestoreSpriteRenderers()
    {
        foreach (SpriteRenderer r in sr)
            r.enabled = true;
    }

    public void KillInstantly()
    {
        if (isDying) return;
        skipDeathAnimation = true;
        StopAllCoroutines();
        StartCoroutine(DeathAndRespawn());
    }

    private IEnumerator DeathAndRespawn()
    {
        isDying = true;
        CancelAllActions();

        volume.profile.TryGet<Vignette>(out Vignette vig);

        if (!skipDeathAnimation)
        {
            normalAnimator.SetTrigger("Die");
            yield return null; // esperar un frame para que el animator haga la transición
            float deathAnimDuration = normalAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(Mathf.Max(deathAnimDuration, 0.1f) + 1f);
        }
        else
        {
            normalSprite.SetActive(false);
            yield return new WaitForSeconds(2f);
            skipDeathAnimation = false;
        }

        if (vig) vig.intensity.value = 1f;
        yield return new WaitForSeconds(0.5f);

        OnPlayerDeath?.Invoke();
        transform.position = CheckpointManager.GetCheckpoint();

        uiController.lifes = 3;
        uiController.UpdateLivesIndicator();
        uiController.currentHealth = uiController.maxHealth;
        uiController.healthSlider.value = uiController.maxHealth;
        uiController.healthText.text = $"{uiController.maxHealth}/{uiController.maxHealth}";

        normalSprite.SetActive(true);
        normalAnimator.SetTrigger("Respawn");
        normalAnimator.SetBool("Falling", false);
        normalAnimator.SetBool("Land", true);

        if (vig) vig.intensity.value = 0f;

        SetControlEnabled(true);
        isDying = false;

        // Inmunidad breve al respawnear para evitar daño inmediato
        isImmune = true;
        immuneTimer = 0f;
        blinkTimer = 0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
       UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
   );
    }

    public void CollisionWithEnemy(Vector3 forceDirection)
    {
        if (isDying) return;

        if (!isImmune)
            GetDamage();

        if (isDying) return; // GetDamage puede haber activado la muerte

        StartCoroutine(PlayerCantMoveFor(0.5f));
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(forceDirection * 3f, ForceMode2D.Impulse);
    }


    // ── COLLISIONS ────────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            canStandUp = false;

        if (collision.CompareTag("EnemyProjectile"))
        {
            if (!isImmune && !isDying && !isProtected)
                GetDamage();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Ammunition1") || collision.CompareTag("Ammunition2") || collision.CompareTag("Ammunition3"))
        {
            Destroy(collision.gameObject);
            uiController.GetAmmunition(collision.transform.tag);
        }

        if (collision.CompareTag("Treasure"))
            collision.gameObject.GetComponent<Chest>().OpenChest();

        if (collision.CompareTag("Armor")) { armorItems++; Destroy(collision.gameObject); }
        else if (collision.CompareTag("Weapon")) { weaponItems++; Destroy(collision.gameObject); }
        else if (collision.CompareTag("Ammunition")) { ammunitionItems++; Destroy(collision.gameObject); }
        else if (collision.CompareTag("Melee")) { meleeItems++; Destroy(collision.gameObject); }
        else if (collision.CompareTag("Consumable")) { /* nada */ }
        else if (collision.CompareTag("Ability"))
        {
            canClimb = true;
            abilityItems++;
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Botas"))
        {
            canDoubleJump = true;
            abilityItems++;
            uiController.ActiveIconBoots();
            AudioManager.Instance?.PlayPickupItem(AudioManager.Instance.sounds.pickupItem);
            Destroy(collision.gameObject);
            FindFirstObjectByType<SpellMenuManager>()?.UnlockItem(SpellMenuManager.CollectibleItem.Boots);
            ItemPickupMessage.I?.ShowBootsMessage();
        }
        else if (collision.CompareTag("Mission")) { missionItmes++; Destroy(collision.gameObject); }
        else if (collision.CompareTag("Traductor"))
        {
            Traductor.I?.GiveTranslator();
            uiController.ActiveIconTranslate();
            AudioManager.Instance?.PlayPickupItem(AudioManager.Instance.sounds.pickupItem);
            Destroy(collision.transform.parent.gameObject);
            FindFirstObjectByType<SpellMenuManager>()?.UnlockItem(SpellMenuManager.CollectibleItem.Translator);
            ItemPickupMessage.I?.ShowTranslatorMessage();
        }
        else if (collision.CompareTag("Piolet"))
        {
            Piolet piolet = collision.GetComponent<Piolet>();
            uiController.ActiveIconPiolet();
            piolet.PioletColeccionado();
            AudioManager.Instance?.PlayPickupItem(AudioManager.Instance.sounds.pickupItem);
            FindFirstObjectByType<SpellMenuManager>()?.UnlockItem(SpellMenuManager.CollectibleItem.Piolet);
            ItemPickupMessage.I?.ShowPioletMessage();
        }
        else if (collision.CompareTag("HealthPotion")) { Destroy(collision.gameObject); }
        else if (collision.CompareTag("Enemy"))
        {
            Vector2 forceDir = new Vector2(
                collision.transform.position.x < transform.position.x ? 1f : -1f,
                collision.transform.position.y < transform.position.y ? 2f : 1f
            );
            CollisionWithEnemy(forceDir);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && (isCrouching || isDashing))
        {
            canStandUp = true;
            CheckTop();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Consumable")) && isDashing)
            ForceStopDash();
    }


    // ── UTILITY / PUBLIC API ──────────────────────────────────────────────────

    public Vector3 GetTransform() => transform.position;

    public void ExecuteSelectedAttack(int index)
    {
        playerSpell?.ExecuteSelectedAttack(index);
    }

    public void SetAtGround(bool value)
    {
        atGround = value;
    }

    public bool IsIdleForBarks()
    {
        return !isMoving && !isJumping && !isFalling && !isDashing && !isClimbing && !isSpellActive;
    }

    public void SetControlEnabled(bool enabled)
    {
        canMove = enabled;
        canJump = enabled;
        canDash = enabled;
        canCrouch = enabled;

        playerMainAttack?.SetEnabled(enabled);
        playerSpell?.SetEnabled(enabled);

        if (!enabled)
            move = Vector2.zero;
    }

    public void ForceIdleAnimation()
    {
        currentSpeed = moveSpeed;
        normalAnimator.SetBool("Walk", false);
        normalAnimator.SetBool("Run", false);
        botAnimator.SetBool("Walk", false);
        botAnimator.SetBool("Run", false);
    }

    private IEnumerator PlayerCantMoveFor(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private IEnumerator ResetCanGroundThisFrame()
    {
        yield return new WaitForSeconds(0.1f);
        canGroundThisFrame = true;
    }

    private IEnumerator ResetCanClimb()
    {
        yield return new WaitForSeconds(0.3f);
        canClimb = true;
    }

    // Hay que hacer que esto se pueda leer desde PC
    public int attackDamage => playerSpell != null ? playerSpell.attackDamage : 0;
}
