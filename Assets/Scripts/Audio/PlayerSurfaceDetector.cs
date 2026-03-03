using UnityEngine;

public class PlayerSurfaceDetector : MonoBehaviour
{
    public SurfaceType CurrentSurface { get; private set; } = SurfaceType.Default;

    private void OnTriggerEnter2D(Collider2D other)
    {
        SurfaceTilemap surface = other.GetComponent<SurfaceTilemap>();
        if (surface != null)
            CurrentSurface = surface.surfaceType;
    }
}
