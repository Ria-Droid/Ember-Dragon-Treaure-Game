using UnityEngine;

public class GoblinAI : EnemyBase
{
    protected override void ApplyStats()
    {
        maxHealth = 1;
    
        attackRange = 2.5f;
        attackDamage = 1;
        attackCooldown = 1.2f;
        fleeHealthPercent = 0.5f;
    }
}
