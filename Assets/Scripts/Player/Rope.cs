using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public float lifeTime;
    private float timer;
    private SpriteRenderer sr;
    private float blinkTimer;
    public float blinkTime;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime) { Destroy(gameObject); }
        if(timer >= lifeTime - 5) { Blink(); }
    }

    void Blink()
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkTime)
        {
            sr.enabled = !sr.enabled;
            blinkTimer = 0;
        }
    }
}
