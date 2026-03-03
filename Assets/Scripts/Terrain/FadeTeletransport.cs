using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTeletransport : MonoBehaviour
{
    public Animator animator;
        public void FadeIn()
        {
            animator.Play("FadeTeletransport");
        }
}
