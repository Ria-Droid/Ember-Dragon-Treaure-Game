using UnityEngine;
using UnityEngine.UI;

// One heart image per health point. 
public class PlayerHeartsSimple : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Image[] hearts;

    [Header("Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHearts;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHearts;
        }
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            UpdateHearts(playerHealth.CurrentHealth, playerHealth.maxHealth);
        }
    }

    private void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
                continue;

            // Show the heart only if the player still has that much health
            hearts[i].enabled = i < current;
        }
    }
}
