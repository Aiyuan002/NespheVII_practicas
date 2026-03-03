using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeProjectile : MonoBehaviour
{
    public float speed;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Climb" || collision.transform.tag == "Roof")
        {
            characterController.CreateRope(this.transform, collision.transform.tag);
            Destroy(gameObject);
        }
    }
}
