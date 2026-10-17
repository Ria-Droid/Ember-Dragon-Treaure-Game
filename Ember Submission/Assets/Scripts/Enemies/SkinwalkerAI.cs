using UnityEngine;

public class SkinwalkerAI : EnemyBase
{
    [Header("Skinwalker-specific")]
    public float enrageHealthPercent = 0.5f;
    private bool isEnraged = false;

    protected override void ApplyStats()
    {
        maxHealth = 3;
       
        attackRange = 3f;
        attackDamage = 1;
        attackCooldown = 1f;
        fleeHealthPercent = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if (!isEnraged && (float)CurrentHealth / maxHealth <= enrageHealthPercent)
        {
            Enrage();
        }

        if (IsDead)
            return;
    }

    private void Enrage()
    {
        isEnraged = true;

        moveSpeed *= 1.5f;
        attackDamage += 1;

        
        if (agent != null)
            agent.speed = moveSpeed;

        
        if (animator != null)
            animator.SetBool("Enraged", true);
    }
}