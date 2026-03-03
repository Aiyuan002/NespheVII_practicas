using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;


public class CharacterMonodialogue : MonoBehaviour
{
    public LocalizedString[] phrases;
    public TextMeshProUGUI textUI;
    public float textVelocity = 0.05f;

    private int index = 0;
    private bool writing = false;
    private string currenPhrase = "";

    void Start()
    {
        ShowNextPhrase();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (writing)
            {
                StopAllCoroutines();
                textUI.text = currenPhrase;
                writing = false;
            }
            else
            {
                index++;
                ShowNextPhrase();
            }
        }
    }

    void ShowNextPhrase()
    {
        if (index < phrases.Length)
        {
            currenPhrase = phrases[index].GetLocalizedString();
            StartCoroutine(HandWritingEffect(currenPhrase));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator HandWritingEffect(string textoCompleto)
    {
        writing = true;
        textUI.text = "";
        foreach (char letra in textoCompleto.ToCharArray())
        {
            textUI.text += letra;
            yield return new WaitForSeconds(textVelocity);
        }
        writing = false;
    }
}

