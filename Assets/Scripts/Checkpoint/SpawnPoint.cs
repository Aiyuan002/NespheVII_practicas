using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private bool isDefault = true;

    void Awake()
    {
        if (isDefault)
            CheckpointManager.SetCheckpoint(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
