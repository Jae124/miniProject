using UnityEngine;
using UnityEngine.UI; // Required if you want button interaction feedback later

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spawning Configuration")]
    [SerializeField]
    [Tooltip("The Player Unit Prefab to spawn.")]
    private GameObject unitPrefab; // Assign in Inspector
    [SerializeField] private GameObject healthBarPrefab;

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
        if (healthBarPrefab == null)
            Debug.LogError("PlayerSpawnManager: Health bar not assigned!", this);
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
        if (unitPrefab != null && spawnPoint != null)
        {
            Debug.Log("Spawning Player Unit!");
            GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation); // Or Quaternion.identity
            Health unitHealth = unitInstance.GetComponent<Health>();

            if (unitHealth == null)
            {
                Debug.LogError("Spawned unit is missing Health component!", unitInstance);
                // Maybe destroy unitInstance here if health is critical
                return;
            }

            if (healthBarPrefab == null)
            {
            Debug.LogError("HealthBar Prefab not assigned in PlayerSpawnManager Inspector!", this);
            return; // Cannot proceed without the prefab
            }

            // 2. Spawn the Health Bar
            GameObject healthBarInstance = Instantiate(healthBarPrefab); // Spawn at world origin initially
            Debug.Log($"Spawned Health Bar: {healthBarInstance.name}"); // Add log
            // 3. Parent the Health Bar to the Unit
            //    Setting worldPositionStays = false makes its position relative to the parent's origin
            healthBarInstance.transform.SetParent(unitInstance.transform, false);
            Debug.Log($"Set Parent of {healthBarInstance.name} to {unitInstance.name}. New Parent: {healthBarInstance.transform.parent?.name}"); // Add log


            // 4. Reset Health Bar's Local Scale (optional, but good practice after parenting)
            //healthBarInstance.transform.localScale = Vector3.one; // Or whatever scale your prefab expects locally

            WorldSpaceHealthBar healthBarScript = healthBarInstance.GetComponent<WorldSpaceHealthBar>();
            if (healthBarScript != null)
            {
                Debug.Log($"Found HealthBar script on {healthBarInstance.name}. Initializing..."); // Add log
                healthBarScript.Initialize(unitHealth); // Pass the unit's health component
            }
            else
            {
                Debug.LogError("Health Bar Prefab is missing WorldSpaceHealthBar script!", healthBarInstance);
            }
        }
    }
}