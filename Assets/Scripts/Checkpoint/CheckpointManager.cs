using UnityEngine;

public static class CheckpointManager
{
    private static Vector3 lastCheckpoint = Vector3.zero;

   
    public static void SetCheckpoint(Vector3 pos)
    {
        lastCheckpoint = pos;
    }


    public static Vector3 GetCheckpoint()
    {
        return lastCheckpoint;
    }
}
