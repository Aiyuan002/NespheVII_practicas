using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public PlayerSurfaceDetector surfaceDetector;
    [SerializeField] private PlayerController characterController;

    public bool isMoving;
    public bool isRunning;

    private void Update()
    {
        if (AudioManager.Instance == null || surfaceDetector == null || characterController == null)
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

        if (clip != null)
            AudioManager.Instance.PlayLoop(clip);
        else
            AudioManager.Instance.StopLoop();
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

            case SurfaceType.Rock:
                return isRunning ? sounds.runRock : sounds.walkRock;

            case SurfaceType.RockySand:
                return isRunning ? sounds.runRockySand : sounds.walkRockySand;

            default:
                return null;
        }
    }
}
