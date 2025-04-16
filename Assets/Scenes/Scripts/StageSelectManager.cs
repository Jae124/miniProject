using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [System.Serializable] // Makes this struct show up nicely in the Inspector
    public struct StageData
    {
        public string sceneName; // The filename of the scene to load for this stage
        public int difficultyLevel; // The difficulty associated with this stage
        // Add other data if needed (e.g., public string displayName;)
    }

    public StageData stage1Data = new StageData { sceneName = "GameScene_Level1", difficultyLevel = 1 };
    public StageData stage2Data = new StageData { sceneName = "GameScene_Level2", difficultyLevel = 2 };
    public StageData stage3Data = new StageData { sceneName = "GameScene_Level3", difficultyLevel = 3 };

    // Public function called by Stage 1 Button
    public void SelectStage1()
    {
        LoadStage(stage1Data);
    }

    // Public function called by Stage 2 Button
    public void SelectStage2()
    {
        LoadStage(stage2Data);
    }

    // Public function called by Stage 3 Button
    public void SelectStage3()
    {
        LoadStage(stage3Data);
    }

    // --- Generic Load Function ---
    private void LoadStage(StageData data)
    {
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
