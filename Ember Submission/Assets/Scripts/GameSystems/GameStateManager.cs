using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public enum GameResult
    {
        None,
        FlawlessDefense,
        NarrowSurvival,
        PlayerDied,        
        TreasureDestroyed    
    }

    [Header("References")]
    public PlayerHealth playerHealth;
    public WaveManager waveManager;
    public TreasureHealth treasureHealth;
    public UltimateAbility ultimateAbility;

    [Header("Win Tier Threshold")]
    [Range(0f, 1f)]
    public float flawlessHealthThreshold = 0.75f;

    public string EndMessage { get; private set; } = "";

    public event Action<GameResult> OnGameEnded;

    private bool gameEnded = false;

    void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnPlayerDied += HandlePlayerDied;
        else Debug.LogWarning("GameStateManager: playerHealth is NOT assigned");

        if (treasureHealth != null) treasureHealth.OnTreasureDestroyed += HandleTreasureDestroyed;
        else Debug.LogWarning("GameStateManager: treasureHealth is NOT assigned");

        if (waveManager != null)
        {
            waveManager.OnAllWavesComplete += HandleAllWavesComplete;
            waveManager.OnWaveStarted += HandleWaveStarted;
        }
        else Debug.LogWarning("GameStateManager: waveManager is NOT assigned");
    }

    void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnPlayerDied -= HandlePlayerDied;
        if (treasureHealth != null) treasureHealth.OnTreasureDestroyed -= HandleTreasureDestroyed;

        if (waveManager != null)
        {
            waveManager.OnAllWavesComplete -= HandleAllWavesComplete;
            waveManager.OnWaveStarted -= HandleWaveStarted;
        }
    }

    // Safety net in case an event never fires
    void Update()
    {
        if (gameEnded) return;

        if (treasureHealth != null && treasureHealth.IsDead)
            HandleTreasureDestroyed();
        else if (playerHealth != null && playerHealth.IsDead)
            HandlePlayerDied();
    }

    private void HandleWaveStarted(int waveIndex)
    {
        if (waveManager == null || ultimateAbility == null)
            return;

        int finalWaveIndex = waveManager.waves.Count - 1;

        if (waveIndex == finalWaveIndex)
            ultimateAbility.Unlock();
    }

    private void HandlePlayerDied()
    {
        
        EndGame(GameResult.PlayerDied, "GAME OVER - Dragon defeated");
    }

    private void HandleTreasureDestroyed()
    {
        
        EndGame(GameResult.TreasureDestroyed, "GAME OVER - Enemies reached the treasure");
    }

    private void HandleAllWavesComplete()
    {
        if (gameEnded)
            return;

        if (playerHealth != null && playerHealth.IsDead)
        {
            HandlePlayerDied();
            return;
        }

        if (treasureHealth != null && treasureHealth.IsDead)
        {
            HandleTreasureDestroyed();
            return;
        }

        float healthPercent = playerHealth != null ? playerHealth.HealthPercent : 1f;

        GameResult result = healthPercent >= flawlessHealthThreshold
            ? GameResult.FlawlessDefense
            : GameResult.NarrowSurvival;

        EndGame(result, "VICTORY");
    }

    private void EndGame(GameResult result, string message)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        EndMessage = message;

        if (waveManager != null)
            waveManager.StopWaves();

        switch (result)
        {
            case GameResult.FlawlessDefense:
                Debug.Log("[Game] VICTORY - Flawless Defense");
                break;

            case GameResult.NarrowSurvival:
                Debug.Log("[Game] VICTORY - Narrow Survival");
                break;

            case GameResult.PlayerDied:
                Debug.Log("[Game] GAME OVER - Dragon Defeated");
                break;

            case GameResult.TreasureDestroyed:
                Debug.Log("[Game] GAME OVER - Treasure Destroyed");
                break;
        }

        OnGameEnded?.Invoke(result);
    }

    
    public void DebugForceResult(GameResult result)
    {
        switch (result)
        {
            case GameResult.PlayerDied: HandlePlayerDied(); break;
            case GameResult.TreasureDestroyed: HandleTreasureDestroyed(); break;
            case GameResult.FlawlessDefense: EndGame(result, "VICTORY"); break;
            case GameResult.NarrowSurvival: EndGame(result, "VICTORY"); break;
        }
    }

    [ContextMenu("Test Treasure Loss")]
    public void TestTreasureLoss() { HandleTreasureDestroyed(); }

    [ContextMenu("Test Player Loss")]
    public void TestPlayerLoss() { HandlePlayerDied(); }
}