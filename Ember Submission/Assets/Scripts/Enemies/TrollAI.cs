using UnityEngine;

public class TrollAI : EnemyBase
{
    [Header("Troll-specific")]
    public float attackWindupTime = 0.4f;

    protected override void ApplyStats()
    {
        maxHealth = 3;
       
        attackRange = 2.5f;
        attackDamage = 2;
        attackCooldown = 2.5f;
        fleeHealthPercent = 0.15f;
    }
}