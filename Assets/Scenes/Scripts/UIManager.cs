using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.UI; // <-- Make sure this 'using' statement is present too!


public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Assign the TextMeshPro UI element for displaying mana here.")]
    public TextMeshProUGUI manaTextDisplay; // Public field to link in Inspector

    [Header("Upgrade UI Elements")] // Added Header for clarity
    [Tooltip("Assign the TextMeshPro UI element for displaying the mana upgrade cost.")]
    public TextMeshProUGUI manaUpgradeCostText; // <--- ADD THIS LINE

    public Button manaUpgradeButton; // <--- THIS IS THE DECLARATION LINE. Check it carefully!


    // --- Accessing ManaManager ---
    // Option 1: Assign ManaManager in Inspector (Simpler if setup allows)
    // [Tooltip("Assign the ManaManager GameObject here.")]
    // public ManaManager manaManager;

    // Option 2: Use Singleton (More robust if ManaManager uses the Singleton pattern)
    private ManaManager manaManager;
    // ---

    void Start()
    {
        // If using Singleton pattern in ManaManager:
        manaManager = ManaManager.Instance;

        // Initial check to prevent errors if setup is wrong
        if (manaManager == null)
        {
            Debug.LogError("UIManager: ManaManager not found! Make sure ManaManager exists and uses the Singleton pattern or is assigned.");
        }
        if (manaTextDisplay == null)
        {
            Debug.LogError("UIManager: ManaTextDisplay is not assigned in the Inspector!");
        }

        if (manaUpgradeCostText == null)
        {
            Debug.LogError("UIManager: ManaUpgradeCostText is not assigned in the Inspector!"); // <--- ADD THIS CHECK
        }
        if (manaUpgradeButton == null)
        {
             Debug.LogWarning("UIManager: ManaUpgradeButton is not assigned in the Inspector! Button interactions might not work as expected if direct reference is needed later."); // <--- ADD THIS CHECK (Optional)
        }

        // --- Set Initial UI State ---
        UpdateManaDisplay(); // Call helper function
        UpdateUpgradeCostDisplay(); // <--- ADD THIS LINE: Set initial cost text
    }

    void Update()
    {
        // Update displays that change frequently (like current mana)
        UpdateManaDisplay();
        // Ensure both references are valid before trying to update
        // if (manaManager != null && manaTextDisplay != null)
        // {
        //     // Get the current mana (using the public property)
        //     float currentMana = manaManager.CurrentMana;

        //     // Format and update the text
        //     // Displaying as an integer (common in such games)
        //     manaTextDisplay.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();

        //     // Alternative: Display with one decimal place
        //     // manaTextDisplay.text = "Mana: " + currentMana.ToString("F1");
        // }
    }

    void UpdateManaDisplay()
    {
        if (manaManager != null && manaTextDisplay != null)
        {
            float currentMana = manaManager.CurrentMana;
            // Displaying as an integer
            manaTextDisplay.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();
            // You could add max mana too: e.g. + "/" + Mathf.FloorToInt(manaManager.maxMana).ToString();
        }
    }

    // --- Helper function to update Upgrade Cost Display ---  // <--- ADD THIS FUNCTION
    void UpdateUpgradeCostDisplay()
    {
        if (manaManager != null && manaUpgradeCostText != null)
        {
            int cost = manaManager.CurrentManaUpgradeCost;
            // You can customize this text format
            manaUpgradeCostText.text = "Cost: " + cost.ToString(); // + " Gold" // (Add resource name if desired)
        }
    }


    // --- Public Function for the Button OnClick Event --- // <--- ADD THIS FUNCTION
    public void OnManaUpgradeButtonClicked()
    {
        Debug.Log("UI Manager: Upgrade button clicked.");

        // Call the actual upgrade logic in ManaManager
        if (ManaManager.Instance != null)
        {
            ManaManager.Instance.UpgradeManaSystem();

            // --- IMPORTANT: Update the cost display immediately after trying to upgrade ---
            // This ensures the UI shows the *new* cost if the upgrade was successful.
            UpdateUpgradeCostDisplay();

            // Optional: Add feedback like playing a sound based on success/failure
            // (This would require UpgradeManaSystem to return a bool, or check resources here)
        }
        else
        {
            Debug.LogError("UIManager: Cannot call UpgradeManaSystem because ManaManager Instance is null!");
        }
    }
}