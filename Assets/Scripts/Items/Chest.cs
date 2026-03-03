using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite openSprite;
    public GameObject treasure;
    //public Transform treasurePosition;
    private bool isOpen = false;

    public void OpenChest()
    {
        if (!isOpen)
        {
            isOpen = !isOpen;
            gameObject.GetComponent<SpriteRenderer>().sprite = openSprite;
            GameObject treasureInstance = Instantiate(treasure, this.transform.position, Quaternion.identity);
            //Debug.Log("Treasure position: " + treasurePosition.position);
            treasureInstance.GetComponent<Rigidbody2D>().AddForce(treasureInstance.transform.right * 1.5f, ForceMode2D.Impulse);
        }
    }
}
