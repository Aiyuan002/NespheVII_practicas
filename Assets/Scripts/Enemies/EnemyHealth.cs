using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int currentHealth;
    private int maxHealth = 100;

    // Otras variables y métodos...

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    internal float GetMaxHealth()
    {
        return maxHealth;
    }
}
