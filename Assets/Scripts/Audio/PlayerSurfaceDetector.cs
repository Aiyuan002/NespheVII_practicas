using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSurfaceDetector : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Transform feetPoint;

    [Header("Tiles de cada superficie")]
    [SerializeField] private TileBase[] grassTiles;
    [SerializeField] private TileBase[] mudTiles;
    [SerializeField] private TileBase[] woodTiles;
    [SerializeField] private TileBase[] rockTiles;
    [SerializeField] private TileBase[] rockySandTiles;

    public SurfaceType CurrentSurface { get; private set; } = SurfaceType.None;

    private void Update()
    {
        DetectSurface();
    }

    private void DetectSurface()
    {
        if (groundTilemap == null || feetPoint == null)
        {
            CurrentSurface = SurfaceType.None;
            return;
        }

        Vector3Int cellPosition = groundTilemap.WorldToCell(feetPoint.position);
        TileBase currentTile = groundTilemap.GetTile(cellPosition);

        if (currentTile == null)
        {
            CurrentSurface = SurfaceType.None;
            return;
        }

        if (ContainsTile(grassTiles, currentTile))
            CurrentSurface = SurfaceType.Grass;

        else if (ContainsTile(mudTiles, currentTile))
            CurrentSurface = SurfaceType.Mud;

        else if (ContainsTile(woodTiles, currentTile))
            CurrentSurface = SurfaceType.Wood;

        else if (ContainsTile(rockTiles, currentTile))
            CurrentSurface = SurfaceType.Rock;

        else if (ContainsTile(rockySandTiles, currentTile))
            CurrentSurface = SurfaceType.RockySand;

        else
            CurrentSurface = SurfaceType.None;
    }

    private bool ContainsTile(TileBase[] tileArray, TileBase targetTile)
    {
        if (tileArray == null || targetTile == null) return false;

        foreach (TileBase tile in tileArray)
        {
            if (tile == targetTile)
                return true;
        }

        return false;
    }
}
