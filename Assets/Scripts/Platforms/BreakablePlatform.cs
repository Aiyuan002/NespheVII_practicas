using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public float timeToBreak;
    private float breakTimer;

    private Animator platformAnimator;
    public bool startTimer;
    //private bool startTimer;


    private void Start()
    {
        breakTimer = timeToBreak;
        platformAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (startTimer)
        {
            Debug.Log("entro aquí");
            breakTimer -= Time.deltaTime;
            if (breakTimer <= 0)
            {
                breakTimer = timeToBreak;
                startTimer = false;
                Debug.Log("Me rompo");
                platformAnimator.SetTrigger("Break");
                Destroy(GetComponent<BoxCollider2D>(), 1.5f);
                Destroy(this.gameObject, 2f);
            }
        }
        /*
        if (startTimer)
        {
            breakTimer -= Time.deltaTime;
            if(breakTimer <= 0) 
            {
                breakTimer = timeToBreak;
                startTimer = false;
                transform.GetChild(transform.childCount - 1).SetParent(null);
                gameObject.SetActive(false);
            }
        }*/
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetChild(transform.childCount - 1).tag == "GroundCheck")
        {
            startTimer = true;
        }
    }*/
}
