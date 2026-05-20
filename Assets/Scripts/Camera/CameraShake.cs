using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Vector2 Offset { get; private set; }

    public void Trigger(float magnitude, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(Run(magnitude, duration));
    }

    private IEnumerator Run(float magnitude, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = 1f - elapsed / duration;
            Offset = new Vector2(
                Random.Range(-1f, 1f) * magnitude * t,
                Random.Range(-1f, 1f) * magnitude * t
            );
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Offset = Vector2.zero;
    }
}
