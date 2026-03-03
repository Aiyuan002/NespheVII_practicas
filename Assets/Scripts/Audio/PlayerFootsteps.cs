using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public PlayerSurfaceDetector surfaceDetector;

    public bool isMoving;
    public bool isRunning;

    private void Update()
    {
        if (!isMoving)
        {
            AudioManager.Instance.StopLoop();
            return;
        }

        AudioClip clip = GetFootstepClip();
        AudioManager.Instance.PlayLoop(clip);
    }

    private AudioClip GetFootstepClip()
    {
        var sounds = AudioManager.Instance.sounds;

        switch (surfaceDetector.CurrentSurface)
        {
            case SurfaceType.Mud:
                return isRunning ? sounds.runMud : sounds.walkMud;

            case SurfaceType.Grass:
            default:
                return isRunning ? sounds.runGrass : sounds.walkGrass;
        }
    }
}
