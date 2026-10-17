
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHotkeys : MonoBehaviour
{
    public GameStateManager gameStateManager;

#if UNITY_EDITOR
    void Update()
    {
        if (gameStateManager == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gameStateManager.DebugForceResult(
                GameStateManager.GameResult.FlawlessDefense);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gameStateManager.DebugForceResult(
                GameStateManager.GameResult.NarrowSurvival);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gameStateManager.DebugForceResult(
                GameStateManager.GameResult.PlayerDied);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gameStateManager.DebugForceResult(
                GameStateManager.GameResult.TreasureDestroyed);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex);
        }
    }
#endif
}
