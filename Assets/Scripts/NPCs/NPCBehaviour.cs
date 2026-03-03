using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{

    [Header("NPC Attributes")]
    public Sprite faceImage;

    [Header("UI")]
    public UIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        uiController = GameObject.FindFirstObjectByType<UIController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            uiController.EnabledNPCCanvas(gameObject.name, faceImage);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            uiController.DisabledNPCCanvas();
        }
    }
}
