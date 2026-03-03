using System.Collections.Generic;
using UnityEngine;

public class CSVLoader : MonoBehaviour
{
    private TextAsset csvFile;
    private char lineSeparator = '\n';
    private char fieldSeparator = ';';

    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("localisation");
    }

    public Dictionary<string, string> GetDictionaryValues(string attributeId)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        int attributeIndex = -1;
        string[] lines = csvFile.text.Split(lineSeparator); 
        string[] headers = lines[0].Split(fieldSeparator); 

        //Comprobar con que cabecera coincide el idioma
        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(attributeId))
            {
                attributeIndex = i;
            }
        }

        for (int i = 0; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(fieldSeparator);

            if (fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key))
                {
                    continue;
                }

                var value = fields[attributeIndex];
                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }
}
