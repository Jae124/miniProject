// CharacterUpgradeManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // Use this if using TextMeshPro elements

public class CharacterUpgradeManager : MonoBehaviour
{
    // --- Assign in Inspector ---
    public GameObject characterGridPanel; // The Panel with the Grid Layout Group
    public GameObject characterInfoPanel; // The Panel on the right
    public GameObject characterIconButtonPrefab; // Your icon button prefab

    // References to elements WITHIN the Info Panel
    public Image infoCharacterImage;
    public TextMeshProUGUI infoCharacterNameText; // Use TextMeshProUGUI if using TMP
    public TextMeshProUGUI infoLevelText;
    public TextMeshProUGUI infoHealthText;
    public TextMeshProUGUI infoAttackText;
    public TextMeshProUGUI infoDefenseText;
    public Button infoUpgradeButton;
    public TextMeshProUGUI infoUpgradeCostText;
    // public Button infoCloseButton; // If you added one
    // ---------------------------

    private string currentlySelectedCharacterID = null; // Track selected character

    void Start()
    {
        PopulateCharacterGrid();
        characterInfoPanel.SetActive(false); // Start with info hidden

        // Add listener for the upgrade button
        if (infoUpgradeButton != null) {
             infoUpgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
         // Add listener for close button if you have one
        // if (infoCloseButton != null) {
        //     infoCloseButton.onClick.AddListener(() => characterInfoPanel.SetActive(false));
        // }
    }

    void PopulateCharacterGrid()
    {
        // Clear existing icons first (if any)
        foreach (Transform child in characterGridPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<string> allCharacterIDs = CharacterDatabase.GetAllCharacterIDs(); // Get all defined characters
        List<string> unlockedIDs = SaveManager.Instance?.playerProgress?.unlockedCharacterIDs ?? new List<string>(); // Get unlocked list

        // Sort or order IDs if needed (e.g., alphabetically or by tier)
        // allCharacterIDs.Sort();

        foreach (string charID in allCharacterIDs)
        {
            GameObject iconGO = Instantiate(characterIconButtonPrefab, characterGridPanel.transform);
            CharacterIconButton iconButton = iconGO.GetComponent<CharacterIconButton>();
            CharacterData charData = CharacterDatabase.GetCharacterData(charID); // Get static data

            if (iconButton != null && charData != null)
            {
                bool isUnlocked = unlockedIDs.Contains(charID);
                // Pass the ShowCharacterInfo method as the callback
                iconButton.Setup(charID, charData.iconSprite, isUnlocked, ShowCharacterInfo);
            }
            else
            {
                 Debug.LogError($"Failed to setup icon for {charID}. Button or CharData missing.");
                 Destroy(iconGO);
            }
        }
    }

    // Called by CharacterIconButton when an unlocked icon is clicked
    void ShowCharacterInfo(string characterID)
    {
        currentlySelectedCharacterID = characterID; // Store selection
        CharacterData charData = CharacterDatabase.GetCharacterData(characterID);
        int currentLevel = SaveManager.Instance.playerProgress.GetLevelForCharacter(characterID);

        if (charData == null) {
             Debug.LogError($"Cannot show info, character data null for {characterID}");
             characterInfoPanel.SetActive(false);
             return;
        }

        // --- Populate Info Panel UI ---
        if (infoCharacterImage != null) {
             infoCharacterImage.sprite = charData.largeDisplaySprite; // Use larger sprite if available
             infoCharacterImage.enabled = (infoCharacterImage.sprite != null); // Hide if no sprite
        }
        if (infoCharacterNameText != null) infoCharacterNameText.text = charData.displayName;
        if (infoLevelText != null) infoLevelText.text = $"Level: {currentLevel}";

        // Calculate and display stats based on current level
        if (infoHealthText != null) infoHealthText.text = $"Health: {charData.CalculateHealth(currentLevel)}";
        if (infoAttackText != null) infoAttackText.text = $"Attack: {charData.CalculateAttack(currentLevel)}";
        if (infoDefenseText != null) infoDefenseText.text = $"Defense: {charData.CalculateDefense(currentLevel)}";

        // Calculate and display upgrade cost
        int upgradeCost = charData.CalculateUpgradeCost(currentLevel);
        if (infoUpgradeCostText != null) infoUpgradeCostText.text = $"Cost: {upgradeCost}";

        // Enable/disable upgrade button based on gold and maybe max level
        bool canAfford = SaveManager.Instance.GetCurrentGold() >= upgradeCost;
        // bool isMaxLevel = currentLevel >= MAX_CHARACTER_LEVEL; // Define MAX_CHARACTER_LEVEL
        if (infoUpgradeButton != null) {
             infoUpgradeButton.interactable = canAfford; // && !isMaxLevel;
        }

        // --- Show the Panel ---
        characterInfoPanel.SetActive(true);
    }

    void OnUpgradeButtonClicked()
    {
        if (string.IsNullOrEmpty(currentlySelectedCharacterID)) return; // No character selected

        CharacterData charData = CharacterDatabase.GetCharacterData(currentlySelectedCharacterID);
        int currentLevel = SaveManager.Instance.playerProgress.GetLevelForCharacter(currentlySelectedCharacterID);
        int upgradeCost = charData.CalculateUpgradeCost(currentLevel);


        // Use the SaveManager method to handle upgrade logic (gold check, level up, save)
        SaveManager.Instance.UpgradeCharacter(currentlySelectedCharacterID, upgradeCost);

        // --- Refresh the Info Panel AFTER upgrade attempt ---
        // (This will update level, stats, cost, and button interactability)
        ShowCharacterInfo(currentlySelectedCharacterID);

         // Optional: Add feedback like a sound effect or particle burst
    }

    // --- Back Button Logic ---
    // Create a public function for a Back Button's OnClick() event in the Inspector
     public void GoBackToPreviousScene() // Or specific scene like MapSelectionScene
    {
        // You might need to store the previous scene name in SaveManager
        // or just always go back to a specific scene
        SceneManager.LoadScene("MapSelectionScene"); // Example
    }

     void OnDestroy() {
          if (infoUpgradeButton != null) infoUpgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
          // Remove close button listener if you have one
     }
}