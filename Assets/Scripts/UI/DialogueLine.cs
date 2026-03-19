using System;
using UnityEngine;
using UnityEngine.Localization;

public enum SpeakerId
{
    Protagonist,
    Native
}

public enum DialogueTextStyle
{
    Normal,
    Alien
}

[Serializable]
public class DialogueLine
{
    public SpeakerId speaker;
    public LocalizedString text;
    public DialogueTextStyle textStyle = DialogueTextStyle.Normal;
}