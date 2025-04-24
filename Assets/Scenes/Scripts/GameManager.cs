using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Optional: Add later for Restart functionality
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private LevelManager lm;

    [Header("Game State")]
    [SerializeField] // Use SerializeField to see private fields in Inspector
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver; // Public read-only property

    [Header("Base References")]
    [Tooltip("Assign the Player Base GameObject here.")]
    public Health playerBaseHealth; // Assign in Inspector

    [Tooltip("Assign the Enemy Base GameObject here.")]
    public Health enemyBaseHealth; // Assign in Inspector

    [Header("UI Panels")] // We'll add these fields now for Task 13
    [Tooltip("Assign the 'You Win' UI Panel GameObject here.")]
    public GameObject youWinPanel;

    [Tooltip("Assign the 'You Lose' UI Panel GameObject here.")]
    public GameObject youLosePanel;

    [Header("Post-Stage UI")]
    [SerializeField] private GameObject postStagePanel; // Assign the panel you just created
    [SerializeField] private TMPro.TextMeshProUGUI outcomeText; // Assign the "Victory!"/"Defeat!" text element
    [SerializeField] private Button nextStageButton; 

    [Header("Scene Configuration")]
    [Tooltip("Exact name of the Stage Select scene file")]
    [SerializeField] private string stageSelectSceneName = "StageSelectScene";
    [Tooltip("OPTIONAL: Manually define next stage scene name if not using naming convention")]
    [SerializeField] private string nextStageSceneNameOverride = "";

    // --- Singleton Pattern (Recommended for easy access) ---
    public static GameManager Instance { get; private set; }

    void Start()
    {
        lm = LevelManager.Instance;
        // Accessing the LevelManager instance via the Singleton
        if (lm != null)
        {
            // Call a public function on LevelManager
            lm.NotifyStageStarted();

            // Access a public variable or property (less ideal than using methods)
            int difficulty = lm.GetCurrentDifficulty(); // Use a getter method preferably
            // int difficulty = LevelManager.Instance.stageDifficultyLevel; // Direct access if public
            Debug.Log("GameManager fetched difficulty from LevelManager: " + difficulty);
            // Ensure Time Scale is normal at the start
            Time.timeScale = 1f;
            isGameOver = false; // Explicitly set game state

            // Validate base references
            if (playerBaseHealth == null) Debug.LogError("GameManager: Player Base Health not assigned!");
            if (enemyBaseHealth == null) Debug.LogError("GameManager: Enemy Base Health not assigned!");
        }
        else
        {
            Debug.LogError("GameManager could not find the LevelManager Instance! Is LevelManager in the scene and active?");
        }
    }

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Debug.Log("here!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Usually not needed for a single-level game manager
        }

        // Ensure panels are initially off (safety check)
        if (youWinPanel != null) youWinPanel.SetActive(false);
        if (youLosePanel != null) youLosePanel.SetActive(false);
        if(postStagePanel != null) postStagePanel.SetActive(false);
    }
    // --- End Singleton ---

    void Update()
    {
        // Don't check health if the game is already over
        if (isGameOver) return;

        // Check win/loss conditions (can be done here or triggered by Health.Die())
        // Let's have Health.Die() trigger it for cleaner event-driven logic.
        // See Task 13 modification to Health.cs

        /* Alternative check directly in Update (less recommended):
        if (playerBaseHealth != null && playerBaseHealth.CurrentHealth <= 0)
        {
            GameOver(false); // Player base destroyed = player loses
        }
        else if (enemyBaseHealth != null && enemyBaseHealth.CurrentHealth <= 0)
        {
            GameOver(true); // Enemy base destroyed = player wins
        }
        */
    }

    /// <summary>
    /// Called when the game ends (either win or lose).
    /// </summary>
    /// <param name="playerWon">True if the player won, false if they lost.</param>
    public void GameOver(bool playerWon)
    {
        // Prevent GameOver from running multiple times
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over! Player Won: " + playerWon);

        // --- Stop Gameplay ---
        // Option 1: Pause time (simplest, pauses EVERYTHING including animations)
        Time.timeScale = 0f;

        // Option 2: Flag-based (more complex, requires modifying other scripts)
        // Spawner scripts, UnitMovement scripts etc. would need to check:
        // if (GameManager.Instance.IsGameOver) { return; // or stop actions }
        // This allows animations/UI to potentially continue.
        // For now, Time.timeScale = 0 is fine for the MVP.

        // --- Activate UI Panels (Moved from direct call here to Awake/Inspector link) ---
        ActivateGameOverPanel(playerWon);
    }

    private void ShowPostStageScreen(bool didWin)
    {
        if (postStagePanel == null) {
             Debug.LogError("Post Stage Panel not assigned!");
             return;
        }

        postStagePanel.SetActive(true); // Show the panel
        Time.timeScale = 0f; // Pause the game

        // Update outcome text
        if (outcomeText != null)
        {
            outcomeText.text = didWin ? "Victory!" : "Defeat!";
        }

        // --- Configure Next Stage Button ---
        if (nextStageButton != null)
        {
            // Check if the player won AND if there IS a next stage to go to
            bool canProceed = didWin && CheckIfNextStageExists(); // Implement CheckIfNextStageExists

            nextStageButton.interactable = canProceed;

            // Optional: Change color if disabled
            // Image nextButtonImage = nextStageButton.GetComponent<Image>();
            // if(nextButtonImage != null) nextButtonImage.color = canProceed ? Color.white : Color.grey;
        }
    }

    public void RetryCurrentStage()
    {
        Debug.Log("Retrying current stage...");
        Time.timeScale = 1f; // IMPORTANT: Reset timescale
        // Reload the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextStage()
    {
        Time.timeScale = 1f; // IMPORTANT: Reset timescale

        int nextLevelNumber = lm.stageDifficultyLevel + 1;
        string nextSceneName = DetermineNextSceneName(nextLevelNumber);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
             // Optional: Update SelectedStageInfo if your loading depends on it
             SelectedStageInfo.SelectedDifficulty = nextLevelNumber;
             SelectedStageInfo.SceneToLoad = nextSceneName;

             Debug.Log($"Loading Next Stage - Scene: {nextSceneName}, Difficulty: {nextLevelNumber}");
             SceneManager.LoadScene(nextSceneName);
        }
        else
        {
             Debug.LogError("Could not determine or load next stage. Returning to Stage Select.");
             ReturnToStageSelect(); // Fallback if next stage isn't found
        }
    }

    public void ReturnToStageSelect()
    {
        Debug.Log($"Returning to scene: {stageSelectSceneName}");
        Time.timeScale = 1f; // IMPORTANT: Reset timescale

        if (string.IsNullOrEmpty(stageSelectSceneName)) {
             Debug.LogError("Stage Select Scene Name not set!");
             return;
        }
        SceneManager.LoadScene(stageSelectSceneName);
    }

    private bool CheckIfNextStageExists()
    {
        // Simple check: Assume there's a next stage for now
        // More robust check needed for final level
        string nextScene = DetermineNextSceneName(lm.stageDifficultyLevel + 1);

        // Very basic check (better: use build settings or explicit list)
        // This check only works if the scene name could be determined
        return !string.IsNullOrEmpty(nextScene);

        // TODO: Implement a proper check based on your total number of levels
        // or checking if 'nextScene' exists in Build Settings.
        // Example using total levels:
        // int totalLevels = 5; // Define this somewhere
        // return stageDifficultyLevel < totalLevels;
    }

    private string DetermineNextSceneName(int nextLevel) {
         // Option 1: Use override field if set
         if(!string.IsNullOrEmpty(nextStageSceneNameOverride)) {
             return nextStageSceneNameOverride;
         }

         // Option 2: Use naming convention
         return "GameScene_Level" + nextLevel; // Adjust if your naming is different

         // Option 3: Look up in a data structure (if you have allStageData assigned)
         // foreach (var stage in allStageData) {
         //     if (stage.difficultyLevel == nextLevel) return stage.sceneName;
         // }
         // return ""; // Not found
    }


    // Helper method to activate the correct panel
    private void ActivateGameOverPanel(bool playerWon)
    {
         if (playerWon)
        {
            if (youWinPanel != null)
            {
                Debug.Log("Activating Win Panel");
                youWinPanel.SetActive(true);
                lm.StageWon();
            }
             else Debug.LogError("Win Panel not assigned to GameManager!");
        }
        else
        {
            if (youLosePanel != null)
            {
                Debug.Log("Activating Lose Panel");
                youLosePanel.SetActive(true);
            }
             else Debug.LogError("Lose Panel not assigned to GameManager!");
        }
    }


    // --- Optional: Add a Restart Method ---
    // public void RestartGame()
    // {
    //     Time.timeScale = 1f; // IMPORTANT: Reset time scale before loading scene
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    // }
}