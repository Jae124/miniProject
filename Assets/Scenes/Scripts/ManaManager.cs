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


    // mana upgrade settings header
    [Header("Mana Upgrade Settings")]
    [Tooltip("The amount of mana to add when upgrading.")]

    public float manaUpgradeAmount = 20f; // Amount to add when upgrading

    [Tooltip("The cost of the mana upgrade.")]
    public int initialManaUpgradeCost = 50; // Cost of the upgrade

    [Tooltip("How much the upgrade cost multiplies each time (e.g., 1.5 = 50% increase).")]
    public float manaUpgradeCostMultiplier = 1.5f;

    [Tooltip("How much Mana Regen Rate increases per upgrade (can be 0).")]
    public float regenRateIncreaseAmount = 0.5f;

    // Private variable to store the current cost
    private int currentManaUpgradeCost;



    // Public property to allow other scripts to READ the upgrade cost safely
    public int CurrentManaUpgradeCost => currentManaUpgradeCost;







    // --- Optional: Make it a Singleton for easy access ---
    public static ManaManager Instance { get; private set; }

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate ManaManager found. Destroying this one.");

            Destroy(gameObject); // Destroy duplicate manager
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: If you need it across scenes
        }
        currentManaUpgradeCost = initialManaUpgradeCost; // Initialize the upgrade cost
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

    public void UpgradeManaSystem()
    {
        // --- Step 1: Check if the player can afford it ---
        // We need access to the script managing the *other* resource (Gold, XP, etc.)
        // Assuming you have a ResourceManager Singleton:
        // if (ResourceManager.Instance == null)
        // {
        //     Debug.LogError("ResourceManager Instance not found! Cannot process upgrade.");
        //     return;
        // }
        // no need for resource manager, we are using manasystem to upgrade mana system

        // Check if the player has enough resource (e.g., Gold)
        if (SpendMana(currentManaUpgradeCost)) // Assuming ResourceManager has 'HasEnoughResource'
        {
            
            // --- Step 1: Apply the upgrades ---
            maxMana += manaUpgradeAmount;
            manaRegenRate += regenRateIncreaseAmount;

            // --- Step 2: Increase the cost for the next upgrade ---
            // Use Mathf.RoundToInt or CeilingToInt to keep it as an integer cost
            currentManaUpgradeCost = Mathf.RoundToInt(currentManaUpgradeCost * manaUpgradeCostMultiplier);

            // --- Step 3: Give Feedback (Optional: UI Manager should ideally handle this) ---
            Debug.Log($"Mana System Upgraded! New Max Mana: {maxMana}, New Regen Rate: {manaRegenRate}. Next Upgrade Cost: {currentManaUpgradeCost}");

            // Optional: If this script WERE directly updating UI (less recommended):
            // UpdateUpgradeCostDisplay(); // A function to update the cost text

            // More Recommended: Trigger an event that the UIManager listens for
            // OnManaUpgraded?.Invoke(); // Define an event 'public event System.Action OnManaUpgraded;'
        }
        else
        {
            // Not enough resources
            Debug.Log($"Not enough resources to upgrade mana. Cost: {currentManaUpgradeCost}");
            // Optional: Play a "cannot afford" sound or show a message via UIManager
        }
    }




}