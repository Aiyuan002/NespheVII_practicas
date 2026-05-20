using UnityEngine;
using System.Collections;
public class CampFireControlPoint : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 10f;

    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float minVolume = 0.15f;

    [SerializeField] public AudioSource source;


    private void Start()
    {
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        yield return null;
        if (AudioManager.Instance != null && AudioManager.Instance.sounds != null)
            source.clip = AudioManager.Instance.sounds.fireCamp;

        source.loop = true;
        source.Play();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float t = Mathf.Clamp01(distance / maxDistance);
        float newVol = Mathf.Lerp(maxVolume, minVolume, t);

        source.volume = newVol;
    }
}
