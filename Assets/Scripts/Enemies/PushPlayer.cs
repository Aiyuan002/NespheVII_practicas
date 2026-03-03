using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlayer : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //PONER PUSH
        if (collision.transform.tag == "Player")
        {
            PushPlayerVoid();
        }
    }

    private void PushPlayerVoid()
    {
        Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();

        Vector2 pushDirection = (player.transform.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        float adjustedForce = 0.5f / (distance + 0.5f);

        rbPlayer.AddForce(pushDirection * adjustedForce, ForceMode2D.Impulse);
    }
}
