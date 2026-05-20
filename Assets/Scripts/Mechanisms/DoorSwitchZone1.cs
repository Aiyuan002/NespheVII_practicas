using UnityEngine;
using System.Collections;

public class DoorSwitchZone1 : MonoBehaviour
{
    [Header("Referencias")]
    public TimedDoorZone1 targetDoor;

    [Header("Configuración")]
    public bool activateOnce = false;
    public float cooldown = 5f;
    public GameObject toggleText;

    private bool isCooldown = false;

    private Animator animator;
    private bool hasBeenUsed = false;
    private bool playerInRange = false;
    private Coroutine reverseCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            Activate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            toggleText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            toggleText.SetActive(false);
        }
    }

    public void Activate()
    {
        if (activateOnce && hasBeenUsed) return;
        if (isCooldown) return;

        hasBeenUsed = true;

        if (reverseCoroutine != null) StopCoroutine(reverseCoroutine);
        animator.enabled = true;
        animator.Play("DoorSwitch", 0, 0f);
        targetDoor.Open();

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }

    public void Reset()
    {
        hasBeenUsed = false;
        if (reverseCoroutine != null) StopCoroutine(reverseCoroutine);
        reverseCoroutine = StartCoroutine(PlayReverse());
    }

    private IEnumerator PlayReverse()
    {
        animator.enabled = true;
        animator.Play("DoorSwitch", 0, 1f); 
        animator.speed = 0f;

        float t = 1f;
        float reverseSpeed = 1f / GetClipLength("DoorSwitch");

        while (t > 0f)
        {
            t -= Time.deltaTime * reverseSpeed;
            animator.Play("DoorSwitch", 0, Mathf.Clamp01(t));
            yield return null;
        }

        animator.speed = 1f;
        animator.enabled = false;
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
}
