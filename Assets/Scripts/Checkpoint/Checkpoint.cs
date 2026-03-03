using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            CheckpointManager.SetCheckpoint(other.transform.position);
            Debug.Log("Checkpoint guardado en " + other.transform.position);
        }
    }
}
