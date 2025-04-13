using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [Header("Mana Settings")]
    [Tooltip("The maximum amount of mana the player can hold.")]
    public float maxMana = 100f; // Public to set max mana in Inspector

    [Tooltip("How much mana regenerates per second.")]
    public float manaRegenRate = 2f; // Public to set regen rate in Inspector

    // Private variable to store the current mana
    private float currentMana;

    // Public property to allow other scripts to READ the current mana safely
    public float CurrentMana => currentMana;
    // Alternative if using older C# versions or prefer a method:
    // public float GetCurrentMana() { return currentMana; }

    // --- Optional: Make it a Singleton for easy access ---
    public static ManaManager Instance { get; private set; }

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Debug.Log("here2!");
            Destroy(gameObject); // Destroy duplicate manager
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: If you need it across scenes
        }
    }
    // --- End Optional Singleton ---


    void Start()
    {
        // Initialize mana (e.g., start full or half full)
        currentMana = maxMana / 2; // Let's start with half mana
    }

    void Update()
    {
        // Regenerate mana over time if not already at max
        if (currentMana < maxMana)
        {
            // Increase mana based on rate and time passed since last frame
            currentMana += manaRegenRate * Time.deltaTime;

            // Clamp the value so it doesn't exceed maxMana
            currentMana = Mathf.Clamp(currentMana, 0f, maxMana);
        }
    }

    /// <summary>
    /// Checks if enough mana is available for a given cost.
    /// </summary>
    /// <param name="cost">The amount of mana required.</param>
    /// <returns>True if currentMana >= cost, false otherwise.</returns>
    public bool HasEnoughMana(float cost)
    {
        return currentMana >= cost;
    }

    /// <summary>
    /// Attempts to spend a given amount of mana.
    /// Only succeeds if HasEnoughMana(cost) is true.
    /// </summary>
    /// <param name="cost">The amount of mana to spend.</param>
    /// <returns>True if mana was successfully spent, false otherwise.</returns>
    public bool SpendMana(float cost)
    {
        if (HasEnoughMana(cost))
        {
            currentMana -= cost;
            return true; // Mana spent successfully
        }
        // Not enough mana
        return false; // Mana spending failed
    }
}