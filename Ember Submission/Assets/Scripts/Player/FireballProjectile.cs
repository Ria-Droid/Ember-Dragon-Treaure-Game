using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;
    public int maxHits = 1;        
    public float growRate = 0f;    

    private int hits = 0;
    private readonly HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (growRate > 0f)
            transform.localScale += Vector3.one * growRate * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
      

        if (other.GetComponentInParent<PlayerHealth>() != null) return;
        if (other.GetComponentInParent<TreasureHealth>() != null) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target != null)
        {
            if (!alreadyHit.Add(target)) return;
            target.TakeHit(damage);
            hits++;
            if (hits >= maxHits) Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}