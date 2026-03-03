using UnityEngine;

public class FadeInOnSceneLoad : MonoBehaviour
{
    public Animator pantallaNegraAnimator;
    public string fadeInAnimacion = "FadeIn";

    void Start()
    {
        if (PlayerPrefs.GetInt("TransicionActiva", 0) == 1)
        {
            pantallaNegraAnimator.Play(fadeInAnimacion);
            PlayerPrefs.SetInt("TransicionActiva", 0); 
        }
    }
}
