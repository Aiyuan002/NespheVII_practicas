using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointShooting : MonoBehaviour
{
    private Vector3 mousePos;

    private Camera mainCamera;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public bool canFire;

    private float timer;
    public float fireRate;

    public Animator animator;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Debug.Log("ola ola si ");
        Debug.Log("Main camera si o no " + mainCamera);

        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = mainCamera.nearClipPlane;
        Vector2 aimToSprite = (mousePos - transform.position).normalized;

        animator.SetFloat("AimX", aimToSprite.x);
        Debug.Log("mouse x" + aimToSprite.x);
        animator.SetFloat("AimY", aimToSprite.y);
        Vector3 rotation = mousePos - transform.position;

        float angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer >= fireRate)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) && canFire)
        {
            canFire = false;
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
    }
}
