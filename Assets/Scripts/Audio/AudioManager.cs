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

    [Header("Library")]
    public SoundLibrary sounds;

    [Header("Volume Curve")]
    [SerializeField] private AnimationCurve volumeCurve;

    private const string MASTER = "MasterVol";
    private const string MUSIC = "MusicVol";
    private const string SFX = "SFXVol";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadVolumes();
    }

    // ---------------- VOLUME ----------------

    public void SetMaster(float value) => SetVolume(MASTER, value);
    public void SetMusic(float value) => SetVolume(MUSIC, value);
    public void SetSFX(float value) => SetVolume(SFX, value);

    private void SetVolume(string param, float value)
    {
        value = Mathf.Clamp01(value);
        audioMixer.SetFloat(param, Mathf.Lerp(-80f, 0f, volumeCurve.Evaluate(value)));
        PlayerPrefs.SetFloat(param, value);
    }

    private void LoadVolumes()
    {
        SetMaster(PlayerPrefs.GetFloat(MASTER, 0.5f));
        SetMusic(PlayerPrefs.GetFloat(MUSIC, 0.5f));
        SetSFX(PlayerPrefs.GetFloat(SFX, 0.5f));
    }

    public float GetMaster() => PlayerPrefs.GetFloat(MASTER, 0.5f);
    public float GetMusic() => PlayerPrefs.GetFloat(MUSIC, 0.5f);
    public float GetSFX() => PlayerPrefs.GetFloat(SFX, 0.5f);

    // ---------------- PLAY METHODS ----------------

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
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

    public void StopMusic() => musicSource.Stop();
}

