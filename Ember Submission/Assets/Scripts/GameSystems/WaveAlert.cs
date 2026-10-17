using UnityEngine;
using TMPro;
using System.Collections;

public class WaveAlertUI : MonoBehaviour
{
    public WaveManager waveManager;
    public TMP_Text alertText;
    public float displayDuration = 2.5f;
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Coroutine alertRoutine;

    void Awake()
    {
        canvasGroup = alertText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = alertText.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    void OnEnable()
    {
        if (waveManager != null) waveManager.OnWaveStarted += HandleWaveStarted;
    }

    void OnDisable()
    {
        if (waveManager != null) waveManager.OnWaveStarted -= HandleWaveStarted;
    }

    private void HandleWaveStarted(int waveIndex)
    {
        bool isFinal = waveManager.waves != null && waveIndex == waveManager.waves.Count - 1;
        string message = isFinal ? "FINAL WAVE!" : $"Wave {waveIndex + 1} Incoming!";

        if (alertRoutine != null) StopCoroutine(alertRoutine);
        alertRoutine = StartCoroutine(ShowAlert(message));
    }

    private IEnumerator ShowAlert(string message)
    {
        alertText.text = message;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}