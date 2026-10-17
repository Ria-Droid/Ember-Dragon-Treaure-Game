
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 10;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;

    void Awake()
    {
        currentHealth = maxHealth;

        Debug.Log("[Player] Health initialized: " +
                  currentHealth + "/" + maxHealth);
    }

    public void TakeHit(int amount)
    {
        if (IsDead) return;

        int previousHealth = currentHealth;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (currentHealth != previousHealth)
        {
            Debug.Log("[Player] Health: " +
                      currentHealth + "/" + maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            var kael = GetComponentInChildren<KaelAnimator>();

            if (kael != null)
                kael.PlayDeath();

            OnPlayerDied?.Invoke();

            var pc = GetComponent<PlayerController>();
            if (pc != null)
                pc.canMove = false;

            var fb = GetComponent<FireballAbility>();
            if (fb != null)
                fb.enabled = false;
        }
    }

    public void Heal(int amount)
    {
        int previousHealth = currentHealth;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (currentHealth != previousHealth)
        {
            Debug.Log("[Player] Health: " +
                      currentHealth + "/" + maxHealth);
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public float HealthPercent => (float)currentHealth / maxHealth;
}
