using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSlider : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Slider slider;

    void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
        }
    }

    void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    void Start()
    {
        if (playerHealth != null && slider != null)
        {
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.CurrentHealth;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;
    }
}