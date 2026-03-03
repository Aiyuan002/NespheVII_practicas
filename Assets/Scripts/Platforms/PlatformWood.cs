using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformWood : MonoBehaviour
{
    private GameObject currentPlatform;

    [SerializeField]
    private BoxCollider2D player;
    private bool isDisabling = false;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (currentPlatform != null)
            {
                StartCoroutine(Disable());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlatformWood"))
        {
            currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlatformWood"))
        {
            //Debug.Log("esta aqui dentro??");
            currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlatformWood"))
        {
            currentPlatform = null;
        }
    }

    private IEnumerator Disable()
    {
        if (!isDisabling && currentPlatform != null)
        {
            isDisabling = true;
            TilemapCollider2D platformCollider = currentPlatform.GetComponent<TilemapCollider2D>();
            if (platformCollider != null)
            {
                platformCollider.enabled = false;
                yield return new WaitForSeconds(.5f);
                platformCollider.enabled = true;
            }
            isDisabling = false;

        }
    }
}
