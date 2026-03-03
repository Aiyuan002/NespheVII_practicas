using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Traductor : MonoBehaviour
{
    [Space(5)]
    [Header("Traductor")]
    public bool isActiveTranslate;

    // Start is called before the first frame update
    void Start()
    {
        if (!isActiveTranslate)
        {
            isActiveTranslate = false;
        }
    }
}
