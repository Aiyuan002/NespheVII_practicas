using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSliders : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        // Evita que el slider dispare eventos mientras lo inicializas
        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicSource != null ? musicSource.volume : 1f);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxSource != null ? sfxSource.volume : 1f);

        // Asegura que el audio queda aplicado
        ApplyMusicVolume(musicSlider != null ? musicSlider.value : 1f);
        ApplySfxVolume(sfxSlider != null ? sfxSlider.value : 1f);

        // Conecta listeners
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ApplySfxVolume);
    }

    private void OnDestroy()
    {
        // Limpieza (evita listeners duplicados si se re-instancia)
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(ApplyMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(ApplySfxVolume);
    }

    public void ApplyMusicVolume(float value)
    {
        if (musicSource != null) musicSource.volume = value;
    }

    public void ApplySfxVolume(float value)
    {
        if (sfxSource != null) sfxSource.volume = value;
    }
}
