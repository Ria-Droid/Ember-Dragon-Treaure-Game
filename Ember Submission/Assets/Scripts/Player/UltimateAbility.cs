using UnityEngine;
using System.Collections.Generic;

public class UltimateAbility : MonoBehaviour
{
    [Header("Ultimate Setup")]

    public KeyCode key = KeyCode.Q;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public int damageAmount = 2;
    public float cooldownTime = 8f;
    public bool startUnlocked = false;   
    public float ultimateLifeTime = 6f;

    [Header("Projectiles")]
    public int projectileCount = 12;
    public float spreadAngle = 360f;     
    public float fireballSpeed = 15f;
    public float fireballSize = 2f;      
    public int hitsPerFireball = 2;    
    public float growRate = 1f;          

    [Header("Ultimate Look")]
    public Color ultimateColor = new Color(0.5f, 0.85f, 1f, 1f);   // light blue
    public Material ultimateMaterial;    
    [ColorUsage(true, true)]
    public Color ultimateEmission = new Color(0.3f, 0.7f, 1.5f, 1f); 

    private bool isUnlocked;
    private float cooldownTimer;

    public bool IsUnlocked => isUnlocked;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);

    void Awake()
    {
        var regular = GetComponent<FireballAbility>();
        if (regular != null)
        {
            if (fireballPrefab == null) fireballPrefab = regular.fireballPrefab;
            if (firePoint == null) firePoint = regular.firePoint;
        }
    }

    void Start()
    {
        if (startUnlocked) Unlock();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) Unlock();

        if (!isUnlocked) return;

        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(key) && cooldownTimer <= 0f)
            FireUltimate();
    }

    public void Unlock()
    {
        isUnlocked = true;
        cooldownTimer = 0f;
    }

    private void FireUltimate()
    {
        if (fireballPrefab == null || firePoint == null) return;

        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        GameObject fb = Instantiate(fireballPrefab, firePoint.position, Quaternion.LookRotation(dir));
        fb.transform.localScale *= 1.3f;

        ApplyUltimateLook(fb);

        Rigidbody rb = fb.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = dir * fireballSpeed;

        FireballProjectile p = fb.GetComponent<FireballProjectile>();
        if (p != null)
        {
            p.damage = damageAmount;
            p.lifeTime = ultimateLifeTime;
        }

        cooldownTimer = cooldownTime;
    }

    private void ApplyUltimateLook(GameObject fb)
    {
        // 1. Fireball body (MeshRenderers only, so the trail is left alone)
        foreach (var r in fb.GetComponentsInChildren<MeshRenderer>())
        {
            if (ultimateMaterial != null)
            {
                r.material = ultimateMaterial;
            }
            else
            {
                Material m = r.material;   // per-instance copy, regular fireballs unaffected
                if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", ultimateColor);
                if (m.HasProperty("_Color")) m.SetColor("_Color", ultimateColor);
                if (m.HasProperty("_EmissionColor"))
                {
                    m.EnableKeyword("_EMISSION");
                    m.SetColor("_EmissionColor", ultimateEmission);
                }
            }
        }

        // 2. Trail: keep it, just make it light blue and fade out
        foreach (var t in fb.GetComponentsInChildren<TrailRenderer>())
        {
            t.startColor = ultimateColor;
            t.endColor = new Color(ultimateColor.r, ultimateColor.g, ultimateColor.b, 0f);
            t.Clear();
        }

        // 3. Point light
        foreach (var l in fb.GetComponentsInChildren<Light>())
            l.color = ultimateColor;

        // 4. Particles (if the fireball has any)
        foreach (var ps in fb.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.startColor = ultimateColor;
        }
    }

    public void ReduceCooldownPercent(float percent)
    {
        cooldownTimer = Mathf.Max(0f, cooldownTimer - (cooldownTimer * percent));
    }
}