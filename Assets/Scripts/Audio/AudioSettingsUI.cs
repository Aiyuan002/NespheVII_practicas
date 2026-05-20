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
        if (AudioManager.Instance == null) return;

        master.onValueChanged.AddListener(AudioManager.Instance.SetMaster);
        music.onValueChanged.AddListener(AudioManager.Instance.SetMusic);
        sfx.onValueChanged.AddListener(AudioManager.Instance.SetSFX);

        master.SetValueWithoutNotify(AudioManager.Instance.GetMaster());
        music.SetValueWithoutNotify(AudioManager.Instance.GetMusic());
        sfx.SetValueWithoutNotify(AudioManager.Instance.GetSFX());
    }
}
