using UnityEngine;
using UnityEngine.UI; // Required if you want button interaction feedback later

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [SerializeField]
    [Tooltip("The Player Unit Prefab to spawn.")]
    private GameObject unitPrefab; // Assign in Inspector

    [SerializeField]
    [Tooltip("The Transform where the unit will be spawned.")]
    private Transform spawnPoint; // Assign PlayerSpawnPoint GameObject in Inspector

    [SerializeField]
    [Tooltip("The amount of mana required to spawn this unit.")]
    private float manaCost = 2f; // Adjust as needed

    // References to other managers (using Singletons)
    private ManaManager manaManager;
    private GameManager gameManager;

    void Start()
    {
        // Get references to managers
        manaManager = ManaManager.Instance;
        gameManager = GameManager.Instance;

        // Error checking for setup
        if (unitPrefab == null)
            Debug.LogError("PlayerSpawnManager: Unit Prefab not assigned!", this);
        if (spawnPoint == null)
            Debug.LogError("PlayerSpawnManager: Spawn Point not assigned!", this);
        if (manaManager == null)
            Debug.LogError("PlayerSpawnManager: Cannot find ManaManager Instance!", this);
        if (gameManager == null)
            Debug.LogError("PlayerSpawnManager: Cannot find GameManager Instance!", this);
    }

    /// <summary>
    /// Method to be called by the UI Button's OnClick event.
    /// </summary>
    public void SpawnUnit()
    {
        // Don't allow spawning if game is over
        if (gameManager != null && gameManager.IsGameOver)
        {
            Debug.Log("Cannot spawn unit: Game Over!");
            return;
        }

        // Check if ManaManager exists and if enough mana is available
        if (manaManager != null && manaManager.HasEnoughMana(manaCost))
        {
            // Try to spend the mana
            if (manaManager.SpendMana(manaCost))
            {
                // Mana spent successfully, now instantiate the unit
                InstantiateUnit();
            }
            else
            {
                // This case should ideally not happen if HasEnoughMana check passed,
                // but good for robustness.
                Debug.LogWarning("Failed to spend mana even though HasEnoughMana was true.");
            }
        }
        else
        {
            // Not enough mana
            Debug.Log("Not enough mana to spawn unit! Required: " + manaCost);
            // Optional: Add feedback like a sound effect or UI message
        }
    }

    private void InstantiateUnit()
    {
        Debug.Log(unitPrefab);
        if (unitPrefab != null && spawnPoint != null)
        {
            Debug.Log("Spawning Player Unit!");
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation); // Or Quaternion.identity
        }
    }
}