using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SecuenceCongratultionsManager : MonoBehaviour
{
 [Header("GameObjects de la secuencia")]
    public GameObject gameObjectA;
    public GameObject gameObjectB;

    [Header("Configuración")]
    public float tiempoGameObjectA = 5f;
    public string nombreSiguienteEscena;

    void Start()
    {
        if (gameObjectA != null) gameObjectA.SetActive(true);
        if (gameObjectB != null) gameObjectB.SetActive(false);

        StartCoroutine(SecuenciaEscena());
    }

    IEnumerator SecuenciaEscena()
    {
        yield return new WaitForSeconds(tiempoGameObjectA);
        if (gameObjectA != null) gameObjectA.SetActive(false);
        if (gameObjectB != null) gameObjectB.SetActive(true);

        yield return new WaitForSeconds(5f);


        SceneManager.LoadScene("Credits");
    }
}
