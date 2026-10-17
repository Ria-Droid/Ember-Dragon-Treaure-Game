using UnityEngine;
using UnityEngine.UI;

public class TreasureDiamondUI : MonoBehaviour
{
    [Header("References")]
    public TreasureHealth treasureHealth;
    public Image[] diamonds;

    [Header("Diamond Sprite")]
    public Sprite fullDiamond;

    private void OnEnable()
    {
        if (treasureHealth != null)
        {
            treasureHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (treasureHealth != null)
        {
            treasureHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void Start()
    {
        if (treasureHealth != null)
        {
            UpdateDiamonds(
                treasureHealth.CurrentHealth,
                treasureHealth.maxHealth
            );
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        UpdateDiamonds(current, max);
    }

    private void UpdateDiamonds(int current, int max)
    {
        if (diamonds == null || diamonds.Length == 0)
            return;

        for (int i = 0; i < diamonds.Length; i++)
        {
            if (diamonds[i] == null)
                continue;

            if (i < current)
            {
                diamonds[i].sprite = fullDiamond;
                diamonds[i].enabled = true;
            }
            else
            {
                diamonds[i].enabled = false;
            }
        }
    }
}