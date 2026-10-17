
using UnityEngine;
using System;

public class TreasureHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    public float HealthPercent =>
        maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

    public event Action<int, int> OnHealthChanged;
    public event Action OnTreasureDestroyed;

    void Awake()
    {
        if (maxHealth <= 0)
        {
            maxHealth = 5;
        }

        currentHealth = maxHealth;

        Debug.Log(
            "TREASURE INITIALISED: " +
            currentHealth + "/" + maxHealth
        );
    }

    public void TakeEnemyDamage()
    {
        if (IsDead)
            return;

        
        int amount = 1;

        Debug.Log(
            "TREASURE REACHED BY ENEMY | Damage: " +
            amount
        );

        currentHealth = Mathf.Max(
            0,
            currentHealth - amount
        );

        Debug.Log(
            "TREASURE HEALTH: " +
            currentHealth +
            "/" +
            maxHealth
        );

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0)
        {
            Debug.Log("TREASURE DESTROYED!");
            OnTreasureDestroyed?.Invoke();
        }
    }
}