using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    public bool InDialogue { get; private set; }
    public event Action<bool> OnDialogueChanged;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDialogue(bool inDialogue)
    {
        if (InDialogue == inDialogue) return;
        InDialogue = inDialogue;
        OnDialogueChanged?.Invoke(InDialogue);
    }
}
