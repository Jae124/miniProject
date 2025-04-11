using UnityEngine;
using UnityEngine.UI; 
using System;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Health targetHealth; // Reference to the unit's health script
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.6f, 0f);

    public void Initialize(Health target)
    {
        targetHealth = target;
        if (targetHealth == null)
        {
            Debug.LogError("Target Health is null for health bar!", this.gameObject);
            gameObject.SetActive(false); // Hide if no target
            return;
        }

        // Subscribe to health changes
        targetHealth.OnHealthChanged += UpdateHealthBar;

        // Set initial position relative to target
        transform.localPosition = offset;

        // Set initial health value
        UpdateHealthBar(targetHealth.GetCurrentHealth(), targetHealth.GetMaxHealth());

        // Optional: Subscribe to target's death to destroy the health bar
        //targetHealth.OnDeath += DestroyHealthBar; // Assumes Health script has public event Action OnDeath;
        if (targetHealth.GetCurrentHealth() <= 0) 
        {
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        // IMPORTANT: Unsubscribe when the health bar is disabled/destroyed
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
            // targetHealth.OnDeath -= DestroyHealthBar; // Unsubscribe from death
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
             // Prevent division by zero if maxHealth is somehow 0
             if (maxHealth > 0)
             {
                healthSlider.value = (float)currentHealth / maxHealth; // Slider value is 0-1 range
             }
             else
             {
                 healthSlider.value = 0;
             }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
