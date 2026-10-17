using UnityEngine;
using UnityEngine.UI;

public class TreasureHealthSlider : MonoBehaviour
{
    [Header("References")]
    public TreasureHealth treasureHealth;
    public Slider slider;

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

    void Start()
    {
        if (treasureHealth != null && slider != null)
        {
            slider.maxValue = treasureHealth.maxHealth;
            slider.value = treasureHealth.CurrentHealth;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;
    }
}