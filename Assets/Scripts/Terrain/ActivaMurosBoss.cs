using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class ActivaMurosBoss : MonoBehaviour
{
    public bool activar;

    Animator animator;

    public GameObject objectoAnimacion;
    public GameObject transition;
    public Transform playerTransform;
    public CinemachineCamera vcamJuego;
    public Collider2D[] colliders;
    public BossCangrejo bossCangrejo;

    [SerializeField] float freezeDelay = 0.5f;

    Animator transi;
    PlayerController playerController;
    Rigidbody2D playerRb;

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += Restart;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= Restart;
    }

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        transi = transition.GetComponent<Animator>();
        activar = false;

        if (vcamJuego == null)
            vcamJuego = FindFirstObjectByType<CinemachineCamera>();

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (bossCangrejo == null)
            bossCangrejo = GetComponentInParent<BossCangrejo>();

        playerController = playerTransform.GetComponent<PlayerController>();
        playerRb = playerTransform.GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WaitCamera());
        }
    }

    public void Restart()
    {
        activar = false;
        animator.SetTrigger("Restart");
        EnableColliders(true);
        objectoAnimacion.SetActive(false);
        bossCangrejo?.ResetBoss();
    }

    private void EnableColliders(bool enable)
    {
        foreach (var collider in colliders)
        {
            collider.enabled = enable;
        }
    }

    IEnumerator WaitCamera()
    {
        yield return new WaitForSeconds(freezeDelay);

        playerController.SetControlEnabled(false);
        playerRb.linearVelocity = Vector2.zero;
        playerController.ForceIdleAnimation();

        transi.Play("TransicionEntreCamaras");
        yield return new WaitForSeconds(0.5f);

        activar = true;
        animator.enabled = true;
        animator.SetTrigger("Start");
        ParallaxBackground[] parallaxes = FindObjectsByType<ParallaxBackground>(FindObjectsSortMode.None);
        Dictionary<ParallaxBackground, float> originalZ = new Dictionary<ParallaxBackground, float>();
        foreach (var p in parallaxes)
        {
            originalZ[p] = p.transform.position.z;
            p.enabled = false;
        }

        vcamJuego.Follow = null;
        vcamJuego.LookAt = null;    

        objectoAnimacion.SetActive(true);
        yield return null;

        PlayableDirector director = objectoAnimacion.GetComponent<PlayableDirector>();
        if (director != null)
        {
            yield return new WaitForSeconds((float)director.duration);
        }

        foreach (var p in parallaxes)
        {
            Vector3 pos = p.transform.position;
            pos.z = originalZ[p];
            p.transform.position = pos;
            p.enabled = true;
        }

        vcamJuego.Follow = playerTransform;
        vcamJuego.LookAt = playerTransform;

        playerController.SetControlEnabled(true);
        EnableColliders(false);
    }
}
