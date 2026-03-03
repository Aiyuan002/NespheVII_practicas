using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    public float xMin, xMax, yMin, yMax;

    // Use this for initialization
    void Start()
    {
        //Calcula la distancia entre la camara y el objetivo.
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        //Coloca la camara en la posicion del objetivo y le suma el offset.
        transform.position = player.transform.position + offset;

       // transform.position = new Vector2(Mathf.Clamp(transform.position.x, xMin, xMax), 
         //   Mathf.Clamp(transform.position.y, yMin, yMax));
    }
}