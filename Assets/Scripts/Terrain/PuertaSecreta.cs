using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaSecreta : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            gameObject.SetActive(false);
    }
}
