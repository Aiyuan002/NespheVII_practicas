using UnityEngine;

public class DamageNumberFloat : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 0.5f;
    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.position;
        GetComponent<MeshRenderer>().sortingOrder = 50;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = startPos + Vector3.up * floatSpeed * timer;
        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
