using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider master;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;

    private void Start()
    {
        master.value = AudioManager.Instance.GetMaster();
        music.value = AudioManager.Instance.GetMusic();
        sfx.value = AudioManager.Instance.GetSFX();

        master.onValueChanged.AddListener(AudioManager.Instance.SetMaster);
        music.onValueChanged.AddListener(AudioManager.Instance.SetMusic);
        sfx.onValueChanged.AddListener(AudioManager.Instance.SetSFX);
    }
}
