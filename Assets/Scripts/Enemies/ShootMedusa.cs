using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMedusa : MonoBehaviour
{
    public Transform shootPosition;
    public GameObject bullet;
    public GameObject electricidad;

    void Shoot()
    {
        if (shootPosition == null || bullet == null)
            return;

        // Buscar al jugador SIEMPRE de forma fresca
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("[ShootMedusa] No se encontró ningún objeto con tag 'Player'");
            return;
        }

        Vector3 playerWorldPos = playerObj.transform.position;
        Vector3 spawnWorldPos = shootPosition.position;

        // Dirección 2D: del punto de disparo al jugador
        float dx = playerWorldPos.x - spawnWorldPos.x;
        float dy = playerWorldPos.y - spawnWorldPos.y;
        Vector2 dir = new Vector2(dx, dy).normalized;

        Debug.Log($"[ShootMedusa] PlayerObj='{playerObj.name}' pos={playerWorldPos} | SpawnPos={spawnWorldPos} | Dir={dir}");

        // Debug visual
        Debug.DrawLine(spawnWorldPos, spawnWorldPos + (Vector3)(dir * 3f), Color.red, 2f);

        GameObject bala = Instantiate(bullet, spawnWorldPos, Quaternion.identity);
        bala.transform.localScale = Vector3.one;

        BolaMedusa bolaMedusa = bala.GetComponent<BolaMedusa>();
        if (bolaMedusa != null)
        {
            bolaMedusa.SetDirection(dir);
        }
    }

    public void MeleeElectricidad()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null || electricidad == null)
            return;

        Transform playerTransform = playerObj.transform;
        GameObject electric = Instantiate(
            electricidad,
            new Vector2(playerTransform.position.x, playerTransform.position.y + 0.1f),
            Quaternion.identity
        );
        electric.transform.parent = playerTransform;
        Destroy(electric, .5f);
    }
}
