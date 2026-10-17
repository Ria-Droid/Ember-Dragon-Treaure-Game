using UnityEngine;
using UnityEngine.UI;

// Replaces the asset's HealthBarController. Reads from PlayerHealth instead of PlayerStats.
// Each heart is worth 2 health points: full = 2, half = 1, empty = 0.
public class PlayerHeartsHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;

    private const int HealthPerHeart = 2;

    private GameObject[] heartContainers;
    private Image[] heartFills;

    private void Start()
    {
        if (playerHealth == null || heartsParent == null || heartContainerPrefab == null)
        {
            Debug.LogError("PlayerHeartsHUD: assign Player Health, Hearts Parent and Heart Container Prefab in the Inspector.");
            return;
        }

        BuildHearts();

        playerHealth.OnHealthChanged += HandleHealthChanged;
        UpdateHearts(playerHealth.CurrentHealth, playerHealth.maxHealth);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void BuildHearts()
    {
        int heartCount = Mathf.CeilToInt((float)playerHealth.maxHealth / HealthPerHeart);

        heartContainers = new GameObject[heartCount];
        heartFills = new Image[heartCount];

        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartContainerPrefab, heartsParent, false);
            heartContainers[i] = heart;

            Transform fill = heart.transform.Find("HeartFill");

            if (fill != null)
            {
                heartFills[i] = fill.GetComponent<Image>();
            }
            else
            {
                Debug.LogError("PlayerHeartsHUD: heart prefab has no child named 'HeartFill'.");
            }
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        UpdateHearts(current, max);
    }

    private void UpdateHearts(int current, int max)
    {
        if (heartFills == null)
            return;

        for (int i = 0; i < heartFills.Length; i++)
        {
            if (heartFills[i] == null)
                continue;

            // How many health points belong to this heart (0 to 2)
            int pointsInThisHeart = current - (i * HealthPerHeart);

            // 2 points = 1.0 (full), 1 point = 0.5 (half), 0 = empty
            heartFills[i].fillAmount = Mathf.Clamp01((float)pointsInThisHeart / HealthPerHeart);
        }
    }
}
