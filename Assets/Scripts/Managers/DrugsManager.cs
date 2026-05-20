using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrugsManager : MonoBehaviour
{
    [SerializeField] private Volume _drugsVolume;
    [SerializeField] private float _loadDuration;
    private ColorAdjustments _colorAdjustments;
    private LensDistortion _lensDistorsion;
    [SerializeField] private float _fadeDuration = 2f;
    [SerializeField] private float _holdDuration = 6f;

    private void Awake()
    {
        _drugsVolume.profile.TryGet(out _colorAdjustments);
        _drugsVolume.profile.TryGet(out _lensDistorsion);
        _drugsVolume.weight = 0f;
    }

    public void TriggerEffect()
    {
        StartCoroutine(EffectRoutine());
    }

    private IEnumerator EffectRoutine()
    {
        // Fade in
        for (float i = 0; i < _fadeDuration; i += Time.deltaTime)
        {
            _drugsVolume.weight = i / _fadeDuration;
            yield return null;
        }
        _drugsVolume.weight = 1f;

        // Hold
        yield return new WaitForSeconds(_holdDuration);

        // Fade out
        for (float i = 0; i < _fadeDuration; i += Time.deltaTime)
        {
            _drugsVolume.weight = 1f - (i / _fadeDuration);
            yield return null;
        }
        _drugsVolume.weight = 0f;
    }

    private void Update()
    {
        if (_colorAdjustments != null && _drugsVolume.weight > 0f)
        {
            _colorAdjustments.hueShift.value = Mathf.Sin(Time.time) * 180f;
            _lensDistorsion.xMultiplier.value = 0.5f - (Mathf.Sin(Time.time) * 0.5f);
        }
    }
}
