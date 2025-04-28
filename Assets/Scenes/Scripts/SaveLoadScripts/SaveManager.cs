using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public PlayerProgress playerProgress; // Holds the loaded or new progress data
    private string saveFilePath;

    public string CurrentSelectedMapID { get; set; } // Stores the ID of the map the player just chose

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress_v1.json"); // Added version just in case
        LoadGame();
    }

    public void SaveGame()
    {
        try
        {
            string json = JsonUtility.ToJson(playerProgress, true);
            File.WriteAllText(saveFilePath, json);
            // Debug.Log($"Game Saved to {saveFilePath}"); // Optional: Less frequent logging
        }
        catch (Exception e) { Debug.LogError($"Save Failed: {e.Message}"); }
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log($"Game Loaded from {saveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Load Failed: {e.Message}. Loading default.");
                LoadDefaultProgress();
            }
        }
        else
        {
            Debug.Log("No save file found. Creating default progress.");
            LoadDefaultProgress();
        }
    }

    private void LoadDefaultProgress()
    {
        playerProgress = new PlayerProgress();
        // Optionally save immediately after creating defaults, so a file exists next time
        // SaveGame();
    }

    // --- Update Progress Method ---
    public void UpdateStageProgress(string mapID, int stageJustClearedIndex)
    {
        playerProgress.SetHighestStageForMap(mapID, stageJustClearedIndex);
        SaveGame(); // Save after updating progress
    }

    // --- Application Pause/Quit Saving ---
    void OnApplicationPause(bool pauseStatus) { if (pauseStatus) { SaveGame(); } }
    void OnApplicationQuit() { SaveGame(); }

    // --- Convenience Accessors (optional but clean) ---
    public int GetCurrentGold() => playerProgress.currentGold;
    // public int GetHighestLevelCleared() => playerProgress.highestLevelCleared;
    public List<string> GetUnlockedCharacterIDs() => playerProgress.unlockedCharacterIDs;
    public int GetCharacterLevel(string charID) => playerProgress.GetLevelForCharacter(charID);

    // --- Methods to Modify Progress (Examples) ---
    public void AddGold(int amount)
    {
        if (amount < 0) return; // Safety check
        playerProgress.currentGold += amount;
        // SaveGame(); // Consider if you save after every gold change or only at key moments
    }

    public bool SpendGold(int amount)
    {
        if (amount < 0) return false; // Safety check
        if (playerProgress.currentGold >= amount)
        {
            playerProgress.currentGold -= amount;
            // SaveGame(); // Save after successful spending is often good
            return true;
        }
        return false; // Not enough gold
    }

    // public void UpdateHighestLevel(int levelJustCleared)
    // {
    //     if (levelJustCleared > playerProgress.highestLevelCleared)
    //     {
    //         playerProgress.highestLevelCleared = levelJustCleared;
    //         SaveGame(); // Save immediately after beating a new highest level
    //     }
    // }

     public void UnlockCharacter(string characterID)
    {
        if (!playerProgress.unlockedCharacterIDs.Contains(characterID))
        {
            playerProgress.unlockedCharacterIDs.Add(characterID);
            // Also ensure it has level 1 data
            playerProgress.SetLevelForCharacter(characterID, 1);
            SaveGame(); // Save after unlocking a character
             Debug.Log($"Unlocked Character: {characterID}");
        }
    }

     public void UpgradeCharacter(string characterID, int costToUpgrade, int levelIncrease = 1)
    {
        // Check if unlocked (optional, depends on your game logic)
        if (!playerProgress.unlockedCharacterIDs.Contains(characterID)) {
             Debug.LogWarning($"Attempted to upgrade locked character: {characterID}");
             return;
        }

        if (SpendGold(costToUpgrade)) // Use SpendGold to handle deduction and saving
        {
            int currentLevel = playerProgress.GetLevelForCharacter(characterID);
            playerProgress.SetLevelForCharacter(characterID, currentLevel + levelIncrease);
            // SaveGame() is already called within SpendGold if successful
             Debug.Log($"Upgraded {characterID} to level {currentLevel + levelIncrease}");
        }
        else
        {
             Debug.Log($"Not enough gold to upgrade {characterID}");
        }
    }
}