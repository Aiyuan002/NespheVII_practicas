using UnityEngine;
using System.Collections;

public class TimedDoorZone1 : MonoBehaviour
{
    [Header("Configuración")]
    public float openDuration = 3f;
    public bool staysOpen = false;

    private Animator animator;
    private Collider2D doorCollider;
    private Coroutine closeCoroutine;
    private Coroutine reverseCoroutine;
    private bool isOpen = false;
    public DoorSwitchZone1 linkedSwitch;

    void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        if (reverseCoroutine != null) StopCoroutine(reverseCoroutine);

        animator.enabled = true;
        animator.SetBool("isOpen", true);
        doorCollider.enabled = false;

        if (!staysOpen)
        {
            if (closeCoroutine != null) StopCoroutine(closeCoroutine);
            closeCoroutine = StartCoroutine(CloseAfterDelay());
        }
    }

    public void ForceClose()
    {
        if (!isOpen) return;
        isOpen = false;
        doorCollider.enabled = true;

        reverseCoroutine = StartCoroutine(PlayReverse());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(openDuration);
        ForceClose();
    }

    private IEnumerator PlayReverse()
    {
        animator.Play("TimedDoor", 0, 1f);
        animator.speed = 0f;

        float t = 1f;
        float reverseSpeed = 1f / GetClipLength("TimedDoor");

        while (t > 0f)
        {
            t -= Time.deltaTime * reverseSpeed;
            animator.Play("TimedDoor", 0, Mathf.Clamp01(t));
            yield return null;
        }

        animator.speed = 1f;
        animator.enabled = false;


        if (linkedSwitch != null)
            linkedSwitch.Reset();
    }

    private float GetClipLength(string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 1f;
    }

    public float GetTimeRemaining()
    {
        return 0f;
    }
}
