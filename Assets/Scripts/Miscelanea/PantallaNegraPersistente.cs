using UnityEngine;

public class PantallaNegraPersistente : MonoBehaviour
{
    private static PantallaNegraPersistente instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}
