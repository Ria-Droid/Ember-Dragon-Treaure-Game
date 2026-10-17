using UnityEngine;
using System.Collections;

public class TreasureController : MonoBehaviour
{
    [Header("References")]
    public TreasureHealth treasureHealth;
    public Renderer treasureRenderer;   
    public AudioSource audioSource;     
    public AudioClip hitSound;

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.15f;

    [Header("Flash Settings")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;

    private Vector3 originalPosition;
    private Color originalColor;
    private Coroutine shakeRoutine;
    private Coroutine flashRoutine;

    void Awake()
    {
        originalPosition = transform.localPosition;

        if (treasureRenderer != null)
        {
            originalColor = treasureRenderer.material.color;
        }
    }

    void OnEnable()
    {
        if (treasureHealth != null)
        {
            treasureHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    void OnDisable()
    {
        if (treasureHealth != null)
        {
            treasureHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        //Shake
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(Shake());

        //Flash
        if (treasureRenderer != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(Flash());
        }

        //Sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeStrength;
            float offsetZ = UnityEngine.Random.Range(-1f, 1f) * shakeStrength;
            transform.localPosition = originalPosition + new Vector3(offsetX, 0f, offsetZ);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    private IEnumerator Flash()
    {
        treasureRenderer.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        treasureRenderer.material.color = originalColor;
    }
} 