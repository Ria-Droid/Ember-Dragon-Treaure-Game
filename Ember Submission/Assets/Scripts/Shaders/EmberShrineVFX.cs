using UnityEngine;

public class EmberShrineVFX : MonoBehaviour
{
    [Header("Shrine Settings")]
    public float cooldownReduction = 1.5f;
    public Renderer shrineRenderer;
    public float pulseSpeed = 2f;
    public float pulseMinIntensity = 0.5f;
    public float pulseMaxIntensity = 2f;

    private static readonly int EmissionIntensityID = Shader.PropertyToID("_RimIntensity");
    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (shrineRenderer == null) return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float intensity = Mathf.Lerp(pulseMinIntensity, pulseMaxIntensity, t);

        shrineRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(EmissionIntensityID, intensity);
        shrineRenderer.SetPropertyBlock(propBlock);
    }

    void OnTriggerEnter(Collider other)
    {
        FireballAbility fireball = other.GetComponent<FireballAbility>();
        if (fireball != null)
        {
            fireball.ReduceCooldown(cooldownReduction);
        }
    }
}
