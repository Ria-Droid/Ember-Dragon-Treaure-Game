using UnityEngine;

public class FireballAbility : MonoBehaviour
{
    [Header("Fireball Setup")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireballSpeed = 20f;
    public float cooldownTime = 1.5f;
    public int damageAmount = 1;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);
    public bool IsReady => cooldownTimer <= 0f;

    [Header("Kael")]
    public KaelAnimator kael;
    public bool faceCameraWhenFiring = false;

    private float cooldownTimer = 0f;

    void Awake()
    {
        if (kael == null) kael = GetComponentInChildren<KaelAnimator>();
    }

    void OnEnable()
    {
        if (kael != null) kael.OnFireFrame += SpawnFireball;
    }

    void OnDisable()
    {
        if (kael != null) kael.OnFireFrame -= SpawnFireball;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && cooldownTimer <= 0f)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("FireballAbility: missing fireballPrefab or firePoint reference.");
            return;
        }

        if (faceCameraWhenFiring && Camera.main != null)
        {
            Vector3 f = Camera.main.transform.forward;
            f.y = 0f;
            if (f.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(f);
        }

        cooldownTimer = cooldownTime;
        SpawnFireball();
    }

    
    private void SpawnFireball()
    {
        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.LookRotation(dir));

        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = dir * fireballSpeed;

        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
        if (projectile != null) projectile.damage = damageAmount;
    }

    public void ReduceCooldown(float amount)
    {
        cooldownTimer = Mathf.Max(0f, cooldownTimer - amount);
    }
}