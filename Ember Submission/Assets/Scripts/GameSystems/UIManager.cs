using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public WaveManager waveManager;
    public GameStateManager gameStateManager;

    [Header("HUD Elements")]
    public Slider healthSlider;
    public TMP_Text waveText;

    [Header("End Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;
    public TMP_Text resultText;         
    public TMP_Text loseMessageText;    

    [Header("Lose Messages")]
    public string playerDiedMessage = "GAME OVER\nDragon defeated";
    public string treasureLostMessage = "GAME OVER\nEnemies reached the treasure";

    public float endScreenDelay = 2.3f;

    void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged += UpdateHealthBar;
        if (waveManager != null) waveManager.OnWaveStarted += UpdateWaveText;
        if (gameStateManager != null) gameStateManager.OnGameEnded += ShowEndScreen;
    }

    void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= UpdateHealthBar;
        if (waveManager != null) waveManager.OnWaveStarted -= UpdateWaveText;
        if (gameStateManager != null) gameStateManager.OnGameEnded -= ShowEndScreen;
    }

    void Start()
    {
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);

        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
        }
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    private void UpdateWaveText(int waveIndex)
    {
        if (waveText == null) return;
        waveText.text = $"Wave {waveIndex + 1}";
    }

    private void ShowEndScreen(GameStateManager.GameResult result)
    {
        StartCoroutine(ShowEndScreenRoutine(result));
    }

    private IEnumerator ShowEndScreenRoutine(GameStateManager.GameResult result)
    {
        bool isLoss = result == GameStateManager.GameResult.PlayerDied
                   || result == GameStateManager.GameResult.TreasureDestroyed;

        
        if (result == GameStateManager.GameResult.PlayerDied)
            yield return new WaitForSecondsRealtime(endScreenDelay);

        Time.timeScale = 0f;

        if (isLoss)
        {
            if (loseScreen != null) loseScreen.SetActive(true);

            if (loseMessageText == null)
            {
                Debug.LogWarning("UIManager: loseMessageText is NOT assigned in the Inspector");
            }
            else
            {
                loseMessageText.text = result == GameStateManager.GameResult.TreasureDestroyed
                    ? treasureLostMessage
                    : playerDiedMessage;
            }
        }
        else
        {
            if (winScreen != null) winScreen.SetActive(true);

            if (resultText != null)
            {
                resultText.text = result == GameStateManager.GameResult.FlawlessDefense
                    ? "DRAGON WINS!- FLAWLESS DEFENSE"
                    : "DRAGON WINS!-NARROW SURVIVAL";
            }
        }
    }
}