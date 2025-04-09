using UnityEngine;
using TMPro;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private Health targetHealth; // Assign the Tower's Health script
    [SerializeField] private TextMeshProUGUI healthTextElement; // Assign the TextMeshPro component

    void OnEnable()
    {
        if (healthTextElement == null)
        {
             Debug.LogError("Health Text Element not assigned!", this);
             healthTextElement = GetComponent<TextMeshProUGUI>();
        }
         if (targetHealth == null)
        {
            Debug.LogError("Target Health not assigned! Cannot listen for health changes.", this);
            return;
        }
        targetHealth.OnHealthChanged += UpdateHealthText;
        UpdateHealthText(targetHealth.GetCurrentHealth(), targetHealth.GetMaxHealth());
    }

    void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthText;
        }
    }
    private void UpdateHealthText(int currHealth, int maxHealth)
    {
        if (healthTextElement != null)
        {
            // Use string interpolation for easy formatting
            healthTextElement.text = $"HP: {currHealth} / {maxHealth}";
        }
    }
}
