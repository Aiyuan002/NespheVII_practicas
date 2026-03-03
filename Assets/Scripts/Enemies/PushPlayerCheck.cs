using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayerCheck : MonoBehaviour
{
    public bool pushPlayer = false;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            pushPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            pushPlayer = false;
        }
    }
}
