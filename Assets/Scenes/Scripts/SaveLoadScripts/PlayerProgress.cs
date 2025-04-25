using System.Collections.Generic; // Needed for Lists
using UnityEngine; // Needed for Debug.Log if you add methods here

[System.Serializable] // Make it saveable by JsonUtility
public class PlayerProgress
{
    // --- Specific Data Fields ---
    public int highestLevelCleared;
    public List<string> unlockedCharacterIDs; // Stores IDs like "BasicSoldier", "ArcherCat", "TankGolem"
    public List<string> characterLevelIDs;     // Parallel list: IDs of characters whose levels are stored
    public List<int> characterLevels;         // Parallel list: The level corresponding to the ID at the same index
    public int currentGold;

    // --- Constructor for Default Values (New Game) ---
    public PlayerProgress()
    {
        highestLevelCleared = 0; // Start at level 0 (meaning level 1 is the next to play)
        currentGold = 100;       // Starting gold

        // Start with one basic character unlocked at level 1
        unlockedCharacterIDs = new List<string>() { "BasicSoldier" }; // << CHANGE "BasicSoldier" to your actual starting unit ID
        characterLevelIDs = new List<string>() { "BasicSoldier" }; // << Must match above
        characterLevels = new List<int>() { 1 };                   // Start at level 1
    }

    // --- Helper Method (Convenient place to put this logic) ---
    // Gets the level of a character, returning 1 if not found (assuming level 1 is base)
    public int GetLevelForCharacter(string characterID)
    {
        int index = characterLevelIDs.IndexOf(characterID);
        if (index != -1) // Found the character ID?
        {
            return characterLevels[index]; // Return its level
        }
        else
        {
            Debug.LogWarning($"Character level data not found for ID: {characterID}. Returning default level 1.");
            return 1; // Default level if data doesn't exist for some reason
        }
    }

    // Sets or adds the level for a character
    public void SetLevelForCharacter(string characterID, int level)
    {
        int index = characterLevelIDs.IndexOf(characterID);
        if (index != -1) // Found? Update existing level
        {
            characterLevels[index] = level;
        }
        else // Not found? Add new entry to both lists
        {
            characterLevelIDs.Add(characterID);
            characterLevels.Add(level);
        }
    }
}