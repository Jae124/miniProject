using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseManger : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Assign the parent panel for the Pause Menu elements")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Scene Navigation")]
    [Tooltip("Exact name of the Stage Select scene file")]
    [SerializeField] private string stageSceneName = "StageScene";

    public static bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        if (pauseMenuPanel == null) return; // Safety check

        pauseMenuPanel.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze game time!
        isPaused = true;
        Debug.Log("Game Paused");
        // Optional: Unlock cursor if needed
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel == null) return; // Safety check

        pauseMenuPanel.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume normal game time!
        isPaused = false;
        Debug.Log("Game Resumed");
        // Optional: Lock cursor if needed for gameplay
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    public void ExitToStageSelect()
    {
        Debug.Log($"Exiting to scene: {stageSceneName}");

        // !!! CRITICAL: Reset timescale BEFORE loading the next scene !!!
        Time.timeScale = 1f;
        isPaused = false; // Reset pause state

        if (string.IsNullOrEmpty(stageSceneName))
        {
            Debug.LogError("Stage Select Scene Name is not set in PauseManager Inspector!");
            return;
        }

        SceneManager.LoadScene(stageSceneName);
    }
}
