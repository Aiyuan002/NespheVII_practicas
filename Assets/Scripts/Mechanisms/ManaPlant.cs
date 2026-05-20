using UnityEngine;
using System.Collections;

public class ManaPlant : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] private GameObject consumablePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 0.4f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string explodeTrigger = "Explode";
    [SerializeField] private float destroyDelay = 0.5f;

    private bool destroyed;

    public void Hit()
    {
        if (destroyed) return;
        destroyed = true;

        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        if (animator != null)
            animator.SetTrigger(explodeTrigger);

        yield return new WaitForSeconds(spawnDelay);

        if (consumablePrefab != null)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(consumablePrefab, spawnPos, Quaternion.identity);
        }

    }
}
