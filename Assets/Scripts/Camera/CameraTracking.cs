using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraTracking : MonoBehaviour
{
    public GameObject player;

    public float xMin, xMax, yMin, yMax;

    [Header("Crouch Camera")]
    public float crouchYOffset = -1.5f;
    public float crouchSmoothSpeed = 5f;

    [Header("Vine Climb Finish Camera")]
    [SerializeField] private float vineClimbSmoothSpeed = 3f;

    private PlayerController playerController;
    private CinemachinePositionComposer positionComposer;
    private CinemachineCamera cinemachineCamera;
    private CameraShake cameraShake;
    private float baseYOffset;
    private float baseOrthoSize;
    private Vector2 prevShakeOffset;
    private Coroutine zoomCoroutine;

    private float vineClimbOffsetY = 0f;
    private float vineClimbTargetY = 0f;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        positionComposer = GetComponent<CinemachinePositionComposer>();
        cinemachineCamera = GetComponent<CinemachineCamera>();
        cameraShake = GetComponent<CameraShake>();

        if (positionComposer != null)
            baseYOffset = positionComposer.TargetOffset.y;
        if (cinemachineCamera != null)
            baseOrthoSize = cinemachineCamera.Lens.OrthographicSize;
    }

    private void OnEnable()
    {
        PlayerController.OnVineClimbFinishStarted += HandleVineClimbStart;
        PlayerController.OnVineClimbFinishEnded += HandleVineClimbEnd;
        PlayerShield.OnParry += HandleParry;
    }

    private void OnDisable()
    {
        PlayerController.OnVineClimbFinishStarted -= HandleVineClimbStart;
        PlayerController.OnVineClimbFinishEnded -= HandleVineClimbEnd;
        PlayerShield.OnParry -= HandleParry;
    }

    private void HandleVineClimbStart(float offsetY) => vineClimbTargetY = offsetY;

    private void HandleVineClimbEnd(float teleportY, float teleportX)
    {
        // Compensar el teletransporte directamente en TargetOffset (sin lerp) para que
        // Cinemachine vea su objetivo efectivo (player.pos + TargetOffset) sin cambio:
        // Antes: P + C  →  Después: (P+delta) + (C-delta) = P+C  ✓
        if (positionComposer != null)
        {
            var offset = positionComposer.TargetOffset;
            offset.x -= teleportX;
            offset.y -= teleportY;
            positionComposer.TargetOffset = offset;
        }
        vineClimbOffsetY -= teleportY;
        vineClimbTargetY = 0f;
    }

    void Update()
    {
        if (positionComposer == null) return;

        vineClimbOffsetY = Mathf.Lerp(vineClimbOffsetY, vineClimbTargetY, vineClimbSmoothSpeed * Time.deltaTime);

        float targetY = (playerController != null && playerController.isCrouching)
            ? baseYOffset + crouchYOffset
            : baseYOffset;
        targetY += vineClimbOffsetY;

        Vector3 currentOffset = positionComposer.TargetOffset;

        // Eliminar el shake del frame anterior para aislar el offset base del lerp
        currentOffset.x -= prevShakeOffset.x;
        currentOffset.y -= prevShakeOffset.y;

        currentOffset.y = Mathf.Lerp(currentOffset.y, targetY, crouchSmoothSpeed * Time.deltaTime);

        Vector2 shake = cameraShake != null ? cameraShake.Offset : Vector2.zero;
        currentOffset.x += shake.x;
        currentOffset.y += shake.y;
        prevShakeOffset = shake;

        positionComposer.TargetOffset = currentOffset;
    }

    private void HandleParry()
    {
        cameraShake?.Trigger(0.15f, 0.08f);

        if (cinemachineCamera == null) return;
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomPunch());
    }

    private IEnumerator ZoomPunch()
    {
        float zoomedSize = baseOrthoSize * 0.92f;
        float returnDuration = 0.3f;

        SetOrthoSize(zoomedSize);

        float elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetOrthoSize(Mathf.Lerp(zoomedSize, baseOrthoSize, elapsed / returnDuration));
            yield return null;
        }

        SetOrthoSize(baseOrthoSize);
        zoomCoroutine = null;
    }

    private void SetOrthoSize(float size)
    {
        var lens = cinemachineCamera.Lens;
        lens.OrthographicSize = size;
        cinemachineCamera.Lens = lens;
    }
}