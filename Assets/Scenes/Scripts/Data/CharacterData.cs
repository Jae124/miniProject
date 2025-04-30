using UnityEngine;
using System.Collections.Generic; // If you add lists later

// This attribute allows you to create instances of this data type directly in the Project window
[CreateAssetMenu(fileName = "New CharacterData", menuName = "Game Data/Character Data")]
public class CharacterData : ScriptableObject // Inherit from ScriptableObject
{
    [Header("Identification")]
    public string characterID; // Unique ID (e.g., "BasicSoldier", "ArcherCat") - MUST BE UNIQUE
    public string displayName; // Name shown in UI (e.g., "Soldier", "Archer Cat")

    [Header("Visuals")]
    public Sprite iconSprite; // Small icon for the upgrade grid
    public Sprite largeDisplaySprite; // Larger image for the info panel
    // public GameObject unitPrefab; // Optional: Reference to the actual unit prefab if needed elsewhere

    [Header("Base Stats (at Level 1)")]
    public int baseHealth = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;

    [Header("Leveling & Costs")]
    public int baseUpgradeCost = 50; // Gold cost to go from Level 1 to Level 2
    // How much stats increase per level *after* level 1
    public float healthPerLevel = 20f;
    public float attackPerLevel = 3f;
    public float defensePerLevel = 1f;
    // How much the upgrade cost multiplies each level (e.g., 1.3 means +30% cost each level)
    public float costIncreaseFactor = 1.3f;
    // public int maxLevel = 10; // Optional: Define a max level

    // --- Calculation Methods ---
    // These belong here, keeping data and related calculations together

    public int CalculateHealth(int level)
    {
        if (level <= 1) return baseHealth;
        return baseHealth + Mathf.RoundToInt(healthPerLevel * (level - 1));
    }

    public int CalculateAttack(int level)
    {
        if (level <= 1) return baseAttack;
        return baseAttack + Mathf.RoundToInt(attackPerLevel * (level - 1));
    }

    public int CalculateDefense(int level)
    {
        if (level <= 1) return baseDefense;
        return baseDefense + Mathf.RoundToInt(defensePerLevel * (level - 1));
    }

    public int CalculateUpgradeCost(int currentLevel)
    {
        // Cost to upgrade FROM the currentLevel (to currentLevel + 1)
        if (currentLevel <= 0) currentLevel = 1; // Safety check
        // Optional: Check against maxLevel if defined
        // if (maxLevel > 0 && currentLevel >= maxLevel) return int.MaxValue; // Cannot upgrade further

        // Cost increases exponentially based on the level you are upgrading FROM
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(costIncreaseFactor, currentLevel - 1));
    }
}