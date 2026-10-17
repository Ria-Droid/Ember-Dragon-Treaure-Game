using UnityEngine;
using TMPro;

public class CooldownUI : MonoBehaviour
{
    public FireballAbility fireball;
    public UltimateAbility ultimate;
    public TMP_Text fireballText;
    public TMP_Text ultimateText;

    private bool ultimateUsed = false;

    void Start()
    {
        if (ultimateText != null) ultimateText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Fireball: countdown, then READY
        if (fireball != null && fireballText != null)
        {
            fireballText.text = fireball.IsReady
                ? "Fireball: READY"
                : $"Fireball: {fireball.CooldownRemaining:0.0}s";
        }

        // Ultimate: hidden until the final wave
        if (ultimate != null && ultimateText != null)
        {
            bool show = ultimate.IsUnlocked;
            if (ultimateText.gameObject.activeSelf != show)
                ultimateText.gameObject.SetActive(show);

            if (!show) return;

            if (ultimate.CooldownRemaining > 0f)
            {
                ultimateUsed = true;
                ultimateText.text = $"Ultimate: {ultimate.CooldownRemaining:0.0}s";
            }
            else
            {
                ultimateText.text = ultimateUsed
                    ? "Ultimate ready to use again! (Q)"
                    : "Ultimate unlocked! Press Q";
            }
        }
    }
}