using UnityEngine;

public class LevelManager : MonoBehaviour
{    
    public static LevelManager Instance { get; private set; }

    public int thisLevelNumber = 1; // Assign this in the Inspector for each game scene
    public int stageDifficultyLevel = 1;

    // Key to store the highest level completed in PlayerPrefs
    private const string HighestLevelCompletedKey = "HighestStageCompleted";

    // Call this function when the player wins the level
    public void TriggerLevelComplete()
    {
        Debug.Log($"Level {thisLevelNumber} Complete!");

        // Get the highest level previously completed (default to 0 if none saved)
        int highestCompleted = PlayerPrefs.GetInt(HighestLevelCompletedKey, 0);

        // If the level just completed is higher than the saved highest, update it
        if (thisLevelNumber >= highestCompleted)
        {
            PlayerPrefs.SetInt(HighestLevelCompletedKey, thisLevelNumber);
            PlayerPrefs.Save(); // Good practice to save immediately
            Debug.Log($"New Highest Level Completed Saved: {thisLevelNumber}");
        }
    }

    public void OnEnemyBaseDestroyed() // Called by the base's Health script perhaps
    {
        TriggerLevelComplete();
    }

    public void NotifyStageStarted()
    {
        Debug.Log($"LevelManager notified: Stage {stageDifficultyLevel} started.");
        // ... other level start logic ...
    }

    public int GetCurrentDifficulty()
    {
        return stageDifficultyLevel;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; // <<< IS THIS LINE DEFINITELY HERE AND REACHABLE?
            stageDifficultyLevel = SelectedStageInfo.SelectedDifficulty;
            // Optional: DontDestroyOnLoad(gameObject);
        }
    }

    public void StageWon()
    {
        Debug.Log($"Stage {stageDifficultyLevel} Won!");

        // Get the current highest level completed
        int highestCompleted = PlayerPrefs.GetInt(HighestLevelCompletedKey, 0);

        // If this stage's level is higher than what's saved, update it
        if (stageDifficultyLevel > highestCompleted)
        {
            PlayerPrefs.SetInt(HighestLevelCompletedKey, stageDifficultyLevel);
            PlayerPrefs.Save(); // Save changes immediately
            Debug.Log($"New Highest Level Completed Saved: {stageDifficultyLevel}");
        }
        else
        {
             Debug.Log($"Highest Level Completed ({highestCompleted}) remains unchanged.");
        }
        

        // --- Now decide what happens next ---
        // Option 1: Go back to Stage Select
         // FindObjectOfType<PauseManager>()?.ExitToStageSelect(); // Use existing function if applicable
         // Or directly:
         // Time.timeScale = 1f; // Ensure time is running
         // SceneManager.LoadScene("StageSelectScene");

        // Option 2: Load the next level automatically (if applicable)
        // int nextLevel = stageDifficultyLevel + 1;
        // string nextSceneName = "GameScene_Level" + nextLevel; // Assuming naming convention
        // Check if next scene exists before loading? (More complex)
        // Time.timeScale = 1f;
        // SceneManager.LoadScene(nextSceneName);

         // Option 3: Show a "
    }
}

