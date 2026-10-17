using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EmberShrine : MonoBehaviour
{
    [Header("Shrine Effect")]
    public float cooldownReduction = 1.5f;
    public float ultimateCooldownReductionPercent = 0.3f;

    [Header("Floating Animation")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.4f;
    public float rotateSpeed = 40f;

    private Vector3 startPos;
    private ShrineSpawner spawner;
    private bool collected = false;

    void Awake()
    {
        startPos = transform.position;

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    public void Initialize(ShrineSpawner owningSpawner)
    {
        spawner = owningSpawner;
        startPos = transform.position;
    } // <-- ADDED: this closes Initialize() properly

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        FireballAbility fireball = other.GetComponentInParent<FireballAbility>();
        UltimateAbility ultimate = other.GetComponentInParent<UltimateAbility>();

        bool triggered = false;

        if (fireball != null)
        {
            fireball.ReduceCooldown(cooldownReduction);
            triggered = true;
        }

        if (ultimate != null)
        {
            ultimate.ReduceCooldownPercent(ultimateCooldownReductionPercent);
            triggered = true;
        }

        if (triggered)
        {
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;

        if (spawner != null)
        {
            spawner.NotifyShrineCollected(gameObject);
        }

        Destroy(gameObject);
    }
}