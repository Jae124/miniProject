using System.Collections.Generic; // Needed for Lists
using UnityEngine; // Needed for Debug.Log if you add methods here
using System; // Needed for DateTime

[System.Serializable] // Make it saveable by JsonUtility
public class PlayerProgress
{
    // --- Specific Data Fields ---
    // public int highestLevelCleared;
    public List<string> unlockedCharacterIDs; // Stores IDs like "BasicSoldier", "ArcherCat", "TankGolem"
    public List<string> characterLevelIDs;     // Parallel list: IDs of characters whose levels are stored
    public List<int> characterLevels;         // Parallel list: The level corresponding to the ID at the same index
    public int currentGold;

    // --- NEW Energy Fields ---
    public int currentEnergy;
    public int maxEnergy;
    public string lastEnergyUpdateTimeString; // Store DateTime as ISO 8601 string

    // --- New Fields for Per-Map Progress ---
    // Key: Map ID (e.g., "ForestMap", "DesertMap", "IceMap", "VolcanoMap")
    // Value: Highest stage index cleared within that map (0 means none cleared yet)
    public List<string> mapIDs;
    public List<int> highestStageClearedPerMap;

    // --- Constructor for Default Values (New Game) ---
    public PlayerProgress()
    {
        // highestLevelCleared = 0; // Start at level 0 (meaning level 1 is the next to play)
        currentGold = 100;       // Starting gold

        // Start with one basic character unlocked at level 1
        unlockedCharacterIDs = new List<string>() { "BasicSoldier" }; // << CHANGE "BasicSoldier" to your actual starting unit ID
        characterLevelIDs = new List<string>() { "BasicSoldier" }; // << Must match above
        characterLevels = new List<int>() { 1 };                   // Start at level 1

        // Initialize map progress (assuming 4 maps with IDs below)
        mapIDs = new List<string>() { "ForestMap", "DesertMap", "IceMap", "VolcanoMap" }; // << CHANGE THESE IDs AS NEEDED
        highestStageClearedPerMap = new List<int>() { 0, 0, 0, 0 }; // Start all maps at 0 cleared

        // --- NEW Energy Defaults ---
        maxEnergy = 20; // << SET YOUR DESIRED MAX ENERGY CAPACITY
        currentEnergy = maxEnergy; // Start with full energy
        // Store the time the game was created as the last update time
        lastEnergyUpdateTimeString = DateTime.UtcNow.ToString("o"); // "o" = ISO 8601 format        
    }

    // --- Helper to Get Highest Stage for a Specific Map ---
    public int GetHighestStageForMap(string mapID)
    {
        int index = mapIDs.IndexOf(mapID);
        if (index != -1)
        {
            return highestStageClearedPerMap[index];
        }
        else
        {
            Debug.LogWarning($"Map progress data not found for ID: {mapID}. Returning 0.");
            return 0; // Default if map ID is somehow missing
        }
    }

    // --- Helper to Set Highest Stage for a Specific Map ---
    public void SetHighestStageForMap(string mapID, int stageIndex)
    {
        int index = mapIDs.IndexOf(mapID);
        if (index != -1)
        {
            // Only update if the new stage is higher than the current highest
            if (stageIndex > highestStageClearedPerMap[index])
            {
                highestStageClearedPerMap[index] = stageIndex;
            }
        }
        else
        {
            Debug.LogWarning($"Could not set stage progress. Map ID not found: {mapID}");
        }
    }

    // (K

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