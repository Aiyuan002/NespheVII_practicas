using UnityEngine;
using UnityEngine.UI;


public class CreditsAutoScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float duration = 30f;
    [SerializeField] private bool playOnStart = true;

    private float timer;
    private bool isPlaying;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        if (playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        timer = 0f;
        isPlaying = true;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void Stop()
    {
        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);

        scrollRect.verticalNormalizedPosition = t;

        if (t >= 1f)
        {
            isPlaying = false;
        }
    }

}
