// Inside UnitController.cs (or your unit's main script)
using UnityEngine;

public class UnitController : MonoBehaviour
{
    // --- ADD THIS LINE ---
    public CharacterData characterData; // Assign the SO asset in the Prefab Inspector!
    // ---------------------

    // Instance variables
    public int currentLevel = 1; // This might get set by a spawner later
    private int currentHealth;
    // ... other variables for movement, attack cooldowns etc...

    void Awake() // Or Start()
    {
        if (characterData == null) {
            Debug.LogError($"CharacterData is not assigned on {gameObject.name}!");
            return;
        }

        // Initialize instance stats based on SO data and currentLevel
        InitializeStats();
    }

    public void InitializeStats() // Call this when spawned/level changes
    {
         if (characterData == null) return;
         // Use the calculation methods FROM THE SCRIPTABLE OBJECT
         currentHealth = characterData.CalculateHealth(currentLevel);
         // You might store calculated attack/defense here too if needed frequently
         // int currentAttack = characterData.CalculateAttack(currentLevel);
         // int currentDefense = characterData.CalculateDefense(currentLevel);
         Debug.Log($"{characterData.displayName} initialized. Level: {currentLevel}, Health: {currentHealth}");
    }

    // --- Example of using data ---
    public void TakeDamage(int amount)
    {
        if (characterData == null) return;
        int effectiveDamage = Mathf.Max(1, amount - characterData.CalculateDefense(currentLevel)); // Use calculated defense
        currentHealth -= effectiveDamage;
        Debug.Log($"{characterData.displayName} took {effectiveDamage} damage, HP left: {currentHealth}");
        if (currentHealth <= 0) {
            Die();
        }
    }

     void Die() {
          Debug.Log($"{characterData.displayName} died.");
          // Handle death effects, object pooling/destruction
          Destroy(gameObject); // Simple destruction
     }

    // ... other methods for attacking, moving etc ...
    // Attack method might use characterData.CalculateAttack(currentLevel)
}