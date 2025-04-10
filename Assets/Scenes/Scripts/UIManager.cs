using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Assign the TextMeshPro UI element for displaying mana here.")]
    public TextMeshProUGUI manaTextDisplay; // Public field to link in Inspector

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
    }

    void Update()
    {
        // Ensure both references are valid before trying to update
        if (manaManager != null && manaTextDisplay != null)
        {
            // Get the current mana (using the public property)
            float currentMana = manaManager.CurrentMana;

            // Format and update the text
            // Displaying as an integer (common in such games)
            manaTextDisplay.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();

            // Alternative: Display with one decimal place
            // manaTextDisplay.text = "Mana: " + currentMana.ToString("F1");
        }
    }
}