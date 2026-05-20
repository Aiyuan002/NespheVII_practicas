using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaSecreta : MonoBehaviour
{
    private int hitsToBreak = 2;

    private int currentHits;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public void ReceiveHit()
    {
        currentHits++;
        StartCoroutine(ShakeWall());

        if (currentHits >= hitsToBreak)
            gameObject.SetActive(false);
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
