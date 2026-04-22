using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public PlayerSurfaceDetector surfaceDetector;
    [SerializeField] private CharacterController characterController;

    public bool isMoving;
    public bool isRunning;

    private void Update()
    {
        if (AudioManager.Instance == null || surfaceDetector == null)
            return;

        if (!characterController.atGround)
        {
            AudioManager.Instance.StopLoop();
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        isMoving = moveX != 0 || moveY != 0;
        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (!isMoving)
        {
            AudioManager.Instance.StopLoop();
            return;
        }

        AudioClip clip = GetFootstepClip();
        //Debug.Log("Surface usada por footsteps: " + surfaceDetector.CurrentSurface);
        //Debug.Log("Clip elegido: " + (clip != null ? clip.name : "NULL"));
        AudioManager.Instance.PlayLoop(clip); ;
    }

    private AudioClip GetFootstepClip()
    {
        var sounds = AudioManager.Instance.sounds;

        switch (surfaceDetector.CurrentSurface)
        {
            case SurfaceType.Mud:
                return isRunning ? sounds.runMud : sounds.walkMud;

            case SurfaceType.Grass:
                return isRunning ? sounds.runGrass : sounds.walkGrass;

            case SurfaceType.Wood:
                return isRunning ? sounds.runWood : sounds.walkWood;

            default:
                return isRunning ? sounds.runGrass : sounds.walkGrass;
        }
    }
}
