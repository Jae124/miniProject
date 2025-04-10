using UnityEngine;
// using UnityEngine.SceneManagement; // Optional: Add later for Restart functionality

public class GameManager : MonoBehaviour
{
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


    // --- Singleton Pattern (Recommended for easy access) ---
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
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
    }
    // --- End Singleton ---

    void Start()
    {
        // Ensure Time Scale is normal at the start
        Time.timeScale = 1f;
        isGameOver = false; // Explicitly set game state

        // Validate base references
        if (playerBaseHealth == null) Debug.LogError("GameManager: Player Base Health not assigned!");
        if (enemyBaseHealth == null) Debug.LogError("GameManager: Enemy Base Health not assigned!");
    }

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

    // Helper method to activate the correct panel
    private void ActivateGameOverPanel(bool playerWon)
    {
         if (playerWon)
        {
            if (youWinPanel != null)
            {
                Debug.Log("Activating Win Panel");
                youWinPanel.SetActive(true);
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