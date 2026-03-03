using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel;

    [SerializeField] 
    public GameObject[] subPanels;

    public Slider musicSlider;
    public Slider effectsSlider;

    public AudioSource musicAudioSource;
    public AudioSource effectsAudioSource;

    public void Start()
    {
        musicSlider.value = musicAudioSource.volume;
        effectsSlider.value = effectsAudioSource.volume;

        UpdateMusicVolume(musicSlider.value);  // Esto establece el volumen de la música
        UpdateEffectsVolume(effectsSlider.value);  // Esto establece el volumen de los efectos

        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        effectsSlider.onValueChanged.AddListener(UpdateEffectsVolume);
    }

    public void ToggleOptions()
    {
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            subPanels[0].SetActive(false);
            subPanels[1].SetActive(false);
        }
        else
        {
            optionsPanel.SetActive(true);
            subPanels[0].SetActive(true);
            subPanels[1].SetActive(true);
        }
    }

    // Función para actualizar el volumen de la música
    public void UpdateMusicVolume(float value)
    {
        musicAudioSource.volume = value;
    }

    // Función para actualizar el volumen de los efectos
    public void UpdateEffectsVolume(float value)
    {
        effectsAudioSource.volume = value;
    }
}