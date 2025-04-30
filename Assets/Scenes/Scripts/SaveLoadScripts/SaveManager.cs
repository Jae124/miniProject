using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public PlayerProgress playerProgress; // Holds the loaded or new progress data
    private string saveFilePath;

    // --- NEW Energy Constants ---
    private const int ENERGY_REFILL_RATE_SECONDS = 300; // << SET YOUR REFILL TIME (e.g., 300 seconds = 5 minutes per energy point)

    // --- Timer variable for periodic updates ---
    private float energyUpdateTimer = 0f;
    private const float ENERGY_UPDATE_INTERVAL = 1.0f; // Check every second

    public string CurrentSelectedMapID { get; set; } // Stores the ID of the map the player just chose

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress_v1.json"); // Added version just in case
        LoadGame();

        CharacterDatabase.Initialize(); //! I think this should happen after loadgame, but not 100% sure
    }

    void Update()
     {
         // --- Periodic Energy Update ---
         // Only update if the game is running and energy isn't full
         if (playerProgress != null && playerProgress.currentEnergy < playerProgress.maxEnergy)
         {
             energyUpdateTimer += Time.deltaTime;
             if (energyUpdateTimer >= ENERGY_UPDATE_INTERVAL)
             {
                 energyUpdateTimer -= ENERGY_UPDATE_INTERVAL; // Reset timer partially
                 UpdateEnergy();
                 // Note: We don't necessarily need to SAVE here every second,
                 // saving on pause/quit is enough to capture progress.
             }
         } else {
             // Reset timer if energy is full so it doesn't accumulate unnecessarily
             energyUpdateTimer = 0f;
         }
     }

    // public void SaveGame()
    // {
        // try
        // {
        //     string json = JsonUtility.ToJson(playerProgress, true);
        //     File.WriteAllText(saveFilePath, json);
        //     // Debug.Log($"Game Saved to {saveFilePath}"); // Optional: Less frequent logging
        // }
        // catch (Exception e) { Debug.LogError($"Save Failed: {e.Message}"); }
    // }

    public void SaveGame()
    {
        // --- Before saving, ensure the energy time is current ---
        // This is crucial if UpdateEnergy hasn't run recently or if energy is maxed out
        if (playerProgress != null && playerProgress.currentEnergy >= playerProgress.maxEnergy) {
             // If energy is maxed, just update the timestamp to now so refill doesn't start until it drops
             playerProgress.lastEnergyUpdateTimeString = DateTime.UtcNow.ToString("o");
        } else if (playerProgress != null) {
            // If not maxed, a final UpdateEnergy call ensures the timestamp reflects partial progress
            // (Optional, depending if Update loop is reliable enough)
            // UpdateEnergy(); // Careful not to cause infinite loops if UpdateEnergy calls SaveGame
        }

        try
        {
            string json = JsonUtility.ToJson(playerProgress, true);
            File.WriteAllText(saveFilePath, json);
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

                // --- Crucial: Update energy after loading ---
                UpdateEnergy();
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

        // --- Ensure energy update happens immediately after creating defaults ---
        UpdateEnergy(); // Calculate initial state correctly based on creation time
    }

    // --- Core Energy Update Logic ---
    private void UpdateEnergy()
    {
        if (playerProgress == null) return; // Not loaded yet
        if (playerProgress.currentEnergy >= playerProgress.maxEnergy) {
             // If already full, just ensure the timestamp is current for next time it drops
             playerProgress.lastEnergyUpdateTimeString = DateTime.UtcNow.ToString("o");
             return; // No refill needed
        }

        DateTime lastUpdateTime;
        // Try to parse the saved time string
        if (DateTime.TryParse(playerProgress.lastEnergyUpdateTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind, out lastUpdateTime))
        {
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan elapsed = currentTime - lastUpdateTime;
            int secondsElapsed = (int)elapsed.TotalSeconds;

            if (secondsElapsed > 0)
            {
                int energyToRefill = secondsElapsed / ENERGY_REFILL_RATE_SECONDS;

                if (energyToRefill > 0)
                {
                    playerProgress.currentEnergy += energyToRefill;

                    // Calculate the exact time the last *full* point was earned
                    DateTime timeOfLastFullRefill = lastUpdateTime.AddSeconds(energyToRefill * ENERGY_REFILL_RATE_SECONDS);
                    playerProgress.lastEnergyUpdateTimeString = timeOfLastFullRefill.ToString("o");

                    // Clamp energy to maximum
                    if (playerProgress.currentEnergy >= playerProgress.maxEnergy)
                    {
                        playerProgress.currentEnergy = playerProgress.maxEnergy;
                        // When maxed out, set last update time to NOW, because refill stops.
                        playerProgress.lastEnergyUpdateTimeString = currentTime.ToString("o");
                    }

                    Debug.Log($"Refilled {energyToRefill} energy. Current: {playerProgress.currentEnergy}");
                     // Optional: Trigger a UI update event here
                }
                 else {
                    // No full point refilled, but time passed. Update timestamp to keep tracking partial progress.
                    // If we DON'T update the timestamp here, the player loses the fractional progress
                    // made during this interval when the next UpdateEnergy runs.
                    // However, only update if current time is actually later than stored time.
                    if(currentTime > lastUpdateTime) {
                         playerProgress.lastEnergyUpdateTimeString = currentTime.ToString("o");
                    }
                 }
            }
        }
        else
        {
            Debug.LogError($"Could not parse lastEnergyUpdateTimeString: {playerProgress.lastEnergyUpdateTimeString}. Resetting time.");
            // Fallback: Reset the time to now if parsing fails
            playerProgress.lastEnergyUpdateTimeString = DateTime.UtcNow.ToString("o");
        }
    }

    // --- Consume Energy Method ---
    public bool ConsumeEnergy(int amount)
    {
        if (playerProgress == null) return false; // Not loaded
        if (amount <= 0) return true; // Consuming 0 or less costs nothing

        // --- Ensure energy is up-to-date before checking ---
        UpdateEnergy();

        if (playerProgress.currentEnergy >= amount)
        {
            // If energy was full before spending, mark the time NOW as the start of refill period
            if (playerProgress.currentEnergy == playerProgress.maxEnergy) {
                playerProgress.lastEnergyUpdateTimeString = DateTime.UtcNow.ToString("o");
            }

            playerProgress.currentEnergy -= amount;
            Debug.Log($"Consumed {amount} energy. Remaining: {playerProgress.currentEnergy}");
            SaveGame(); // Save progress after consuming energy
            // Optional: Trigger a UI update event here
            return true; // Success
        }
        else
        {
            Debug.Log($"Not enough energy. Required: {amount}, Have: {playerProgress.currentEnergy}");
            return false; // Failure
        }
    }

    // --- Accessor Methods ---
    public int GetCurrentEnergy() => playerProgress?.currentEnergy ?? 0; // Null check
    public int GetMaxEnergy() => playerProgress?.maxEnergy ?? 0; // Null check
    public TimeSpan GetTimeUntilNextEnergy()
    {
        if (playerProgress == null || playerProgress.currentEnergy >= playerProgress.maxEnergy)
        {
            return TimeSpan.Zero; // No refill needed or data not ready
        }

        DateTime lastUpdateTime;
        if (DateTime.TryParse(playerProgress.lastEnergyUpdateTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind, out lastUpdateTime))
        {
             DateTime nextRefillTime = lastUpdateTime.AddSeconds(ENERGY_REFILL_RATE_SECONDS);
             TimeSpan timeRemaining = nextRefillTime - DateTime.UtcNow;
             return timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
        }
        else
        {
            return TimeSpan.MaxValue; // Indicate an error or unknown state
        }
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