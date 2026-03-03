using UnityEngine;
using System.Collections;
using UnityEngine.Localization.Settings;

public class LocalizationController : MonoBehaviour
{
    private bool _active = false;

    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);
    }

    public void ChangeLocale(int localeID)
    {
        if (_active)
        {
            return;
        }
        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int LocaleID)
    {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LocaleID];
        PlayerPrefs.SetInt("LocaleKey", LocaleID);
        _active = false;
    }
}