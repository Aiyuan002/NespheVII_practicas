using System.Collections;
using UnityEngine;

public class PuertaSecreta : MonoBehaviour
{
    public int hitsToBreak = 3;

    private int currentHits;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
{
    currentHits++;

    StartCoroutine(ShakeWall());

    Destroy(collision.gameObject);

    if (currentHits >= hitsToBreak)
    {
        gameObject.SetActive(false);
    }
}
    }

    IEnumerator ShakeWall()
    {
        float shakeTime = 0.15f;

        while (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            transform.position = startPosition + (Vector3)Random.insideUnitCircle * 0.05f;

            yield return null;
        }

        transform.position = startPosition;
    }
}
