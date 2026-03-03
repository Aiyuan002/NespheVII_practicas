using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlatform : MonoBehaviour
{
    public bool sender;
    public bool receiver;
    public bool returnable;
    public Transform linkedTo;
    [HideInInspector]
    public bool activated;

    private void Start()
    {
        if (sender || receiver && returnable) { activated = true; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetChild(transform.childCount - 1).tag == "GroundCheck" )
        {
            if (activated)
            {
                activated = false;
                linkedTo.GetComponent<PortalPlatform>().activated = false;
                var player = collision.gameObject.GetComponent<CharacterController>();
                if (player.enablePlatform)
                {
                    player.transform.position = linkedTo.transform.position + new Vector3(0, 0.5f);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (sender) { activated = true; }
        if (receiver && returnable) { activated = true; }
    }
}
