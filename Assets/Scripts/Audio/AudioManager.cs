using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource pickupSource;
    

    [Header("Library")]
    public SoundLibrary sounds;

    [Header("Volume Curve")]
    [SerializeField] private AnimationCurve volumeCurve;

    private const string MASTER = "MasterVol";
    private const string MUSIC = "MusicVol";
    private const string SFX = "SFXVol";
    private float lastSFXVol = -1f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Forzar volume a 1 para que el mixer tenga control total
        sfxSource.volume = 1f;
        loopSource.volume = 1f;
        musicSource.volume = 1f;

        StartCoroutine(LoadVolumesNextFrame());
    }

    private IEnumerator LoadVolumesNextFrame()
    {
        yield return null;
        LoadVolumes();
    }

    private void Update()
    {
        if (sfxSource.volume != 1f)
            Debug.Log($"[Audio] sfxSource.volume es: {sfxSource.volume}", this);

        audioMixer.GetFloat(SFX, out float currentVol);
        if (Mathf.Abs(currentVol - lastSFXVol) > 0.01f)
        {
            Debug.Log($"[Audio] SFXVol cambió a: {currentVol}", this);
            lastSFXVol = currentVol;
        }
    }
    public void PlayPickup(AudioClip clip)
{
    if (clip == null || pickupSource == null) return;
    pickupSource.PlayOneShot(clip, 1f);
}

    // ---------------- VOLUME ----------------

    public void SetMaster(float value) => SetVolume(MASTER, value);
    public void SetMusic(float value) => SetVolume(MUSIC, value);
    public void SetSFX(float value) => SetVolume(SFX, value);

    private void SetVolume(string param, float value)
    {
        value = Mathf.Clamp01(value);
        float db = Mathf.Lerp(-80f, 0f, volumeCurve.Evaluate(value));
        bool ok = audioMixer.SetFloat(param, db);
        Debug.Log($"[Audio] SetFloat({param}, {db}) → ok:{ok} whfoiqwfqoiwjfiqhw");
        PlayerPrefs.SetFloat(param, value);
    }

    private void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat(MASTER, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC, 0.5f);
        float sfx = PlayerPrefs.GetFloat(SFX, 0.5f);

        Debug.Log($"[Audio] LoadVolumes → master:{master} music:{music} sfx:{sfx}");
        Debug.Log($"[Audio] volumeCurve evaluate 0.5 = {volumeCurve.Evaluate(0.5f)}");

        SetMaster(master);
        SetMusic(music);
        SetSFX(sfx);

        Debug.Log($"[Audio] sfxSource.volume tras load: {sfxSource.volume}");
    }

    public float GetMaster() => PlayerPrefs.GetFloat(MASTER, 0.5f);
    public float GetMusic() => PlayerPrefs.GetFloat(MUSIC, 0.5f);
    public float GetSFX() => PlayerPrefs.GetFloat(SFX, 0.5f);

    // ---------------- PLAY METHODS ----------------

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        Debug.Log($"[Audio] PlaySFX: {clip.name} | volume: {sfxSource.volume} | mixer mute: ?");
        sfxSource.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip)
    {
        if (clip == null) return;
        if (loopSource.clip == clip && loopSource.isPlaying) return;

        loopSource.clip = clip;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopLoop()
    {
        if (loopSource.isPlaying)
            loopSource.Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void PlayPickupItem(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void StopMusic() => musicSource.Stop();
}

