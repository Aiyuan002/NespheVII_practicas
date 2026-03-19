using UnityEngine;

public class Traductor : MonoBehaviour
{
    public static Traductor I { get; private set; }

    [field: SerializeField] public bool HasTranslator { get; private set; }

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GiveTranslator()
    {
        HasTranslator = true;
    }

    public void RemoveTranslator()
    {
        HasTranslator = false;
    }







    [Space(5)][Header("Traductor")] public bool isActiveTranslate; // Start is called before the first frame update void Start() { if (!isActiveTranslate) { isActiveTranslate = false; } }
}
