using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageSelectManager : MonoBehaviour
{
    [System.Serializable] // Makes this struct show up nicely in the Inspector
    public struct StageData
    {
        public string sceneName; // The filename of the scene to load for this stage
        public int difficultyLevel; // The difficulty associated with this stage
        // Add other data if needed (e.g., public string displayName;)
        public Button stageButton;
    }

    public StageData stage1Data = new StageData { sceneName = "GameScene_Level1", difficultyLevel = 1 };
    public StageData stage2Data = new StageData { sceneName = "GameScene_Level2", difficultyLevel = 2 };
    public StageData stage3Data = new StageData { sceneName = "GameScene_Level3", difficultyLevel = 3 };
    private List<StageData> allStages;
    private const string HighestLevelCompletedKey = "HighestStageCompleted";

    void Start()
    {
        // Populate the list (do this better if you have many stages)
        allStages = new List<StageData> { stage1Data, stage2Data, stage3Data };

        // Load the highest level completed by the player
        int highestLevelCompleted = PlayerPrefs.GetInt(HighestLevelCompletedKey, 0); // Default to 0
        Debug.Log($"Highest Level Completed Loaded: {highestLevelCompleted}");

        // Update button interactability based on progress
        UpdateStageButtons(highestLevelCompleted);
    }

    void UpdateStageButtons(int highestLevelCompleted)
    {
        foreach (StageData stage in allStages)
        {
            if (stage.stageButton == null)
            {
                Debug.LogWarning($"Button reference missing for stage {stage.difficultyLevel}");
                continue;
            }

            // A stage is playable if its difficulty level (or prerequisite level number)
            // is less than or equal to the highest completed level PLUS ONE.
            // Example: If highest completed is 0, only level 1 (0+1) is playable.
            // Example: If highest completed is 1, levels 1 & 2 (1+1) are playable.
            bool isUnlocked = stage.difficultyLevel <= (highestLevelCompleted + 1);

            stage.stageButton.interactable = isUnlocked;
            // Optional: Add visual feedback for locked buttons
            // e.g., change color, show a lock icon
            // Image buttonImage = stage.stageButton.GetComponent<Image>();
            // if (buttonImage != null) {
            //     buttonImage.color = isUnlocked ? Color.white : Color.grey; // Example color change
            // }
            // You might have a separate "LockIcon" GameObject as a child of the button to show/hide.
        }
    }

    // Public function called by Stage 1 Button
    public void SelectStage1(){ LoadStage(stage1Data); }

    // Public function called by Stage 2 Button
    public void SelectStage2(){ LoadStage(stage2Data); }

    // Public function called by Stage 3 Button
    public void SelectStage3(){ LoadStage(stage3Data); }

    // --- Generic Load Function ---
    private void LoadStage(StageData data)
    {
        int highestLevelCompleted = PlayerPrefs.GetInt(HighestLevelCompletedKey, 0);
        if(data.difficultyLevel > (highestLevelCompleted + 1))
        {
             Debug.LogError($"Attempted to load locked stage {data.difficultyLevel}. Highest completed: {highestLevelCompleted}");
             return;
        }
        
        if (string.IsNullOrEmpty(data.sceneName))
        {
            Debug.LogError("Scene Name for the selected stage is not set in StageSelectManager!");
            return;
        }

        // Store the selected data in our static class
        SelectedStageInfo.SelectedDifficulty = data.difficultyLevel;
        SelectedStageInfo.SceneToLoad = data.sceneName;
        // Optional: Store other data if you added it to StageData and SelectedStageInfo
        // SelectedStageInfo.StageName = data.displayName;

        Debug.Log($"Loading Stage - Scene: {SelectedStageInfo.SceneToLoad}, Difficulty: {SelectedStageInfo.SelectedDifficulty}");

        // Load the scene
        SceneManager.LoadScene(SelectedStageInfo.SceneToLoad);
    }

    // Optional: Function for a Back Button
    public void GoBack(string previousMenuSceneName = "Start_Menu_Scene") // Pass the name of your main menu scene
    {
        if (string.IsNullOrEmpty(previousMenuSceneName))
        {
            Debug.LogError("Previous Menu Scene Name is not set!");
            return;
        }
        Debug.Log($"Returning to {previousMenuSceneName}...");
        SceneManager.LoadScene(previousMenuSceneName);
    }
    
}
