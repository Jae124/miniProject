using UnityEngine;

public class EnemyAISpawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [SerializeField]
    [Tooltip("The Enemy Unit Prefab to spawn.")]
    private GameObject enemyPrefab; // Assign in Inspector

    [SerializeField]
    [Tooltip("The Transform where the enemy unit will be spawned.")]
    private Transform spawnPoint; // Assign EnemySpawnPoint GameObject in Inspector

    [SerializeField]
    [Tooltip("Time in seconds between each enemy spawn.")]
    private float spawnInterval = 5.0f; // Adjust as needed

    // Internal timer
    private float timer = 0f;

    // Reference to game manager
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;

        // Error checking
        if (enemyPrefab == null)
            Debug.LogError("EnemyAISpawner: Enemy Prefab not assigned!", this);
        if (spawnPoint == null)
            Debug.LogError("EnemyAISpawner: Spawn Point not assigned!", this);
         if (gameManager == null)
            Debug.LogError("EnemyAISpawner: Cannot find GameManager Instance!", this);

        // Optional: Start the timer offset slightly or add initial delay
        // timer = spawnInterval / 2f;
    }

    void Update()
    {
        // Don't spawn if game is over
        if (gameManager != null && gameManager.IsGameOver)
        {
            return;
        }

        // Increment timer
        timer += Time.deltaTime;

        // Check if it's time to spawn
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            // Reset timer (subtracting interval is slightly more accurate over time than setting to 0)
            timer -= spawnInterval;
            // Alternatively, simpler reset: timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            Debug.Log("Spawning Enemy Unit!");
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation); // Or Quaternion.identity
        }
    }
}