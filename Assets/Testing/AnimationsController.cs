using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationsController : MonoBehaviour
{
    public Animator topAnim;
    public Animator botAnim;

    public void Button()
    {
        topAnim.SetBool("Walk", false);
        botAnim.SetBool("Walk", false);
        topAnim.SetBool("Run", false);
        botAnim.SetBool("Run", false);
    }

    public void WalkButton()
    {
        topAnim.SetBool("Walk", true);
        botAnim.SetBool("Walk", true);
        topAnim.SetBool("Run", false);
        botAnim.SetBool("Run", false);
    }

    public void RunButton()
    {
        topAnim.SetBool("Walk", false);
        botAnim.SetBool("Walk", false);
        topAnim.SetBool("Run", true);
        botAnim.SetBool("Run", true);
    }

    public void VerticalShootButton()
    {
        topAnim.Play("VerticalShoot");
    }

    public void HorizontalShootButton()
    {
        topAnim.Play("HorizontalShoot");
    }

    public void DiagonalShootButton()
    {
        topAnim.Play("DiagonalShoot");
    }
}
