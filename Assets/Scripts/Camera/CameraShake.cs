using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    float duration = 1.5f;

    void Start()
    {
        StartCoroutine(Shake(4f));
    }

    public IEnumerator Shake(float magnitude)
    {
        float elapsed = 0f;
        Debug.Log("aquientrashake");
        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1f) * magnitude;
            float y = Random.Range(-1, 1f) * magnitude;
            transform.localPosition = new Vector3(
                transform.position.x + x,
                transform.position.y + y,
                transform.position.z
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
