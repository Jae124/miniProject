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
        //print out 1 to the console
        //Debug.Log("Initializing Health Bar"); -- okay, so we get here. 
        targetHealth = target;

        //log getting here
        Debug.Log("got here");
        
        if (targetHealth == null)
        {
            Debug.LogError("Target Health is null for health bar!", this.gameObject);
            gameObject.SetActive(false); // Hide if no target
            return;
        }


        // print targethealth to the console
        //Debug.Log("Target Health: " + targetHealth.gameObject.name); - ok, so we get here too.

        // Subscribe to health changes
        // Debug.Log($"Subscribing to OnHealthChanged for target: {targetHealth.gameObject.name}");
        targetHealth.OnHealthChanged += UpdateHealthBar;

        // Set initial position relative to target
        transform.localPosition = offset;

        // Set initial health value
        // log the current health and max health
        //Debug.Log("Current Health: " + targetHealth.GetCurrentHealth() + ", Max Health: " + targetHealth.GetMaxHealth());
        UpdateHealthBar(targetHealth.GetCurrentHealth(), targetHealth.GetMaxHealth());

        // Optional: Subscribe to target's death to destroy the health bar
        //targetHealth.OnDeath += DestroyHealthBar; // Assumes Health script has public event Action OnDeath;
        if (targetHealth.GetCurrentHealth() <= 0) 
        {
            Debug.Log("here3!");
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        // IMPORTANT: Unsubscribe when the health bar is disabled/destroyed
        
        if (targetHealth != null)
        {
            Debug.Log("here4!");
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
                Debug.Log("Current Health: " + targetHealth.GetCurrentHealth() + ", Max Health: " + targetHealth.GetMaxHealth());
                healthSlider.value = (float)currentHealth / maxHealth; // Slider value is 0-1 range
                Debug.Log("Health Slider Value: " + healthSlider.value); // Log the slider value
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
