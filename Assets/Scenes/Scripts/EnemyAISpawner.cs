using UnityEngine;
using System.Collections.Generic;

public class EnemyAISpawner : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [SerializeField]
    [Tooltip("The Enemy Unit Prefab to spawn.")]
    private List<GameObject> enemyPrefabs = new List<GameObject>(); // Assign in Inspector

    [SerializeField]
    [Tooltip("The Transform where the enemy unit will be spawned.")]
    private Transform spawnPoint; // Assign EnemySpawnPoint GameObject in Inspector

    [SerializeField]
    [Tooltip("Time in seconds between each enemy spawn.")]
    private float spawnInterval = 5.0f; // Adjust as needed
    private float timer = 0f;// Internal timer
    private GameManager gameManager; // Reference to game manager
    private int waveNumber = 0;

    void Start()
    {
        gameManager = GameManager.Instance;

        // Error checking
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
            SpawnEnemyWave();
            // Reset timer (subtracting interval is slightly more accurate over time than setting to 0)
            timer -= spawnInterval;
            // Alternatively, simpler reset: timer = 0f;
        }
    }

    void SpawnEnemyWave() // Example wave logic
    {
        waveNumber++;
        Debug.Log($"Starting Wave {waveNumber}");

        // --- EXAMPLE: Spawn different enemies based on wave ---
        int enemiesToSpawn = 2 + waveNumber; // Simple increasing difficulty

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // --- Logic to CHOOSE which enemy type ---
            int enemyIndex = 0; // Default to the first enemy (e.g., Grunt)

            if (waveNumber > 1 && Random.Range(0, 3) == 0 && enemyPrefabs.Count > 1) // 1/3 chance for Ranger after wave 1
            {
                enemyIndex = 1;
            }
            if (waveNumber > 2 && Random.Range(0, 5) == 0 && enemyPrefabs.Count > 2) // 1/5 chance for Brute after wave 2
            {
                enemyIndex = 2;
            }
            // Add more complex logic here based on wave data, etc.

            SpawnEnemyByIndex(enemyIndex);
        }
    }

    public void SpawnEnemyByIndex(int index)
    {
        if (index < 0 || index >= enemyPrefabs.Count || enemyPrefabs[index] == null)
        {
            Debug.LogError($"Invalid enemy index {index} or prefab not assigned at that index.");
            return;
        }

        GameObject enemyToSpawnPrefab = enemyPrefabs[index];
        SpawnEnemy(enemyToSpawnPrefab); // Call the actual instantiation logic
    }

    // --- Core Instantiation Logic ---
    private void SpawnEnemy(GameObject specificEnemyPrefab)
    {
        if (spawnPoint == null) { Debug.LogError("Spawn point not set!"); return; }

        GameObject enemyInstance = Instantiate(specificEnemyPrefab, spawnPoint.position, spawnPoint.rotation); // Use spawn point's rotation often
        Health enemyHealth = enemyInstance.GetComponent<Health>();
        if (enemyHealth == null) { Debug.LogError("Spawned enemy missing Health script!", enemyInstance); return; }
        Debug.Log($"Spawned '{enemyInstance.name}'");


        // --- Health Bar Setup (Identical logic as before) ---
        /*if (healthBarPrefab != null)
        {
            GameObject healthBarInstance = Instantiate(healthBarPrefab);
            if (healthBarInstance != null)
            {
                healthBarInstance.transform.SetParent(enemyInstance.transform, false);
                healthBarInstance.transform.localScale = Vector3.one;
                Canvas healthBarCanvas = healthBarInstance.GetComponent<Canvas>();
                if (healthBarCanvas != null && healthBarCanvas.worldCamera == null)
                {
                    if (mainGameCamera != null) { healthBarCanvas.worldCamera = mainGameCamera; }
                    else if (Camera.main != null) { healthBarCanvas.worldCamera = Camera.main; }
                    else { Debug.LogError("No camera found for health bar!"); }
                }
                WorldSpaceHealthBar healthBarScript = healthBarInstance.GetComponent<WorldSpaceHealthBar>();
                if (healthBarScript != null) { healthBarScript.Initialize(enemyHealth); }
                else { Debug.LogError("Health Bar Prefab missing script!"); }
            } else { Debug.LogError("Failed to instantiate Health Bar!"); }
        }*/

    }
}