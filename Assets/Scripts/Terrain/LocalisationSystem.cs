using System.Collections.Generic;
using UnityEngine;

public class LocalisationSystem : MonoBehaviour
{
    public enum Language
    {
        Spanish,
        English,
        French
    }

    public Language selectedLanguage;
    public static Language language;

    private static Dictionary<string, string> localisedES;
    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedFR;

    public static bool isInit;

    private void Start()
    {
        selectLanguage();
    }

    private void Update()
    {
        selectLanguage();
    }

    //Seleccionar idioma
    void selectLanguage()
    {
        if (selectedLanguage == Language.Spanish)
        {
            language = Language.Spanish;
        }
        else if (selectedLanguage == Language.English)
        {
            language = Language.English;
        }
        else if (selectedLanguage == Language.French)
        {
            language = Language.French;
        }
    }

    //Cargar los diccionarios
    public static void Init()
    {
        CSVLoader csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        localisedES = csvLoader.GetDictionaryValues("es");
        localisedEN = csvLoader.GetDictionaryValues("en");
        localisedFR = csvLoader.GetDictionaryValues("fr");

        isInit = true;
    }

    //Coger el texto que corresponda segun el idioma
    public static string GetLocalisedValues(string key)
    {
        string value = key;

        if (!isInit)
            Init();

        switch (language)
        {
            case Language.Spanish:
                localisedES.TryGetValue(key, out value);
                break;
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.French:
                localisedFR.TryGetValue(key, out value);
                break;
        }

        return value;
    }
}
