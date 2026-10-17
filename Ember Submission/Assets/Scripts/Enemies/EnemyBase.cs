using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyBase : MonoBehaviour, IDamageable
{
    

    [Header("Stats (set in ApplyStats() of each enemy subclass)")]
    public int maxHealth = 3;
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float aggroRange = 8f;
    public float fleeHealthPercent = 0.25f;
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;
    public float treasureReachDistance = 2f;

    [Header("References")]
    public Transform treasureTarget;
    public Transform[] spawnPoints;
    public Animator animator;

    protected NavMeshAgent agent;
    private Transform player;

    

    private int currentHealth;
    private float attackTimer;

    private bool removed = false;

    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0 || removed;

    public event Action OnReachedTreasure;
    public static event Action OnAnyEnemyDied;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        
        ApplyStats();

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        currentHealth = maxHealth;

        FindPlayer();
        if (GetComponent<EnemyHealthBar>() == null)
            gameObject.AddComponent<EnemyHealthBar>();
    }

    protected virtual void ApplyStats() { }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    
    protected float PlayerDistance()
    {
        Vector3 a = transform.position;
        Vector3 b = player.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    protected virtual void Update()
    {
        if (removed)
            return;

        if (player == null)
            FindPlayer();

        if (animator != null)
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.name == "InAttackRange" &&
                    parameter.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(
                        "InAttackRange",
                        player != null && PlayerDistance() <= attackRange
                    );
                    break;
                }
            }
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

         
        if (player != null)
        {
            if (PlayerDistance() <= attackRange)
            {
                AttackIfInRange();
                return;
            }
        }

       
        Advance();
    }

    private void Advance()
    {
        if (removed)
            return;

        Transform destination = treasureTarget;

       
        if (player != null && PlayerDistance() <= aggroRange)
            destination = player;

        if (agent != null && agent.enabled && agent.isOnNavMesh && destination != null)
        {
            agent.isStopped = false;
            agent.SetDestination(destination.position);
        }

        if (player != null && PlayerDistance() <= attackRange)
        {
            
            return;
        }

        if (treasureTarget != null && HasReachedTreasure())
        {
            ReachTreasure();
        }
    }

    private bool HasReachedTreasure()
    {
        Vector3 enemyPos = transform.position;
        Vector3 target = treasureTarget.position;

        Collider col = treasureTarget.GetComponentInChildren<Collider>();
        if (col != null)
            target = col.bounds.ClosestPoint(enemyPos);

        enemyPos.y = 0f;
        target.y = 0f;

        float dist = Vector3.Distance(enemyPos, target);

     

        return dist <= treasureReachDistance;
    }

    private void ReachTreasure()
    {
        if (removed)
            return;

        removed = true;

        StopAgent();

        if (treasureTarget != null)
        {
            TreasureHealth treasure =
                treasureTarget.GetComponentInParent<TreasureHealth>();

            if (treasure != null)
            {
                treasure.TakeEnemyDamage();

                
            }
            else
            {
               
            }
        }

        OnReachedTreasure?.Invoke();

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        OnAnyEnemyDied?.Invoke();

        Destroy(gameObject);
    }

    private void Respawn()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                $"{name}: no spawn points assigned, cannot respawn. Destroying instead."
            );

            removed = true;
            StopAgent();
            OnAnyEnemyDied?.Invoke();
            Destroy(gameObject);
            return;
        }

        Transform respawnPoint =
            spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        if (agent != null && agent.enabled)
        {
            agent.Warp(respawnPoint.position);
        }
        else
        {
            transform.position = respawnPoint.position;
        }

        
    }

    private void AttackIfInRange()
    {
       

        if (removed)
            return;

        if (player == null)
        {
            
            return;
        }

       
        if (PlayerDistance() > attackRange)
        {
            
            return;
        }

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        if (attackTimer <= 0f)
        {
            PlayerHealth target =
                player.GetComponentInParent<PlayerHealth>();

            if (target == null)
            {
                target =
                    player.GetComponentInChildren<PlayerHealth>();
            }

            if (target != null)
            {
                target.TakeHit(attackDamage);

                
            }
            else
            {
                Debug.LogError(
                    $"{name} tried to attack but PlayerHealth could not be found!"
                );
            }

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            attackTimer = attackCooldown;
        }
    }

    private void Flee()
    {
        if (removed || player == null)
            return;

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        Vector3 fleeDirection =
            (transform.position - player.position).normalized;

        Vector3 fleeTarget =
            transform.position + fleeDirection * 5f;

        agent.SetDestination(fleeTarget);
    }

    public void TakeHit(int amount)
    {
        if (removed)
            return;

        currentHealth =
            Mathf.Max(0, currentHealth - amount);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

       
    }

    private void Die()
    {
        if (removed)
            return;

        removed = true;

        StopAgent();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        OnAnyEnemyDied?.Invoke();

        Destroy(gameObject, 1.5f);
    }

    private void StopAgent()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }
}