using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the Arrow Prefab here.")]
    public GameObject arrowPrefab; // Assign the prefab you created

    [Tooltip("Assign the empty GameObject used as the spawn position.")]
    public Transform arrowSpawnPoint; // Assign the ArrowSpawnPoint child object

    [Header("Shooting Settings")]
    [Tooltip("Time between shots in seconds.")]
    public float fireRate = 3f;

    [Tooltip("Angle (degrees) above horizontal to launch the arrow. 0=horizontal, 90=straight up.")]
    public float launchAngle = 45f; // Exposed launch angle

    [Tooltip("The initial force applied to the arrow.")]
    public float launchForce = 15f; // Exposed launch force

    private float nextFireTime = 0f;

    void Update()
    {
        // Check if enough time has passed to fire again
        if (Time.time >= nextFireTime)
        {
            // Time to shoot!
            Shoot();

            // Set the time for the next shot
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null)
        {
            Debug.LogError("TowerShooter: Prefab or Spawn Point not assigned!");
            return;
        }

        // Instantiate the Arrow at the spawn point
        GameObject arrowInstance = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity); // Start with no rotation

        // Get the Rigidbody2D from the instantiated arrow
        Rigidbody2D rb = arrowInstance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Calculate the launch direction vector based on the angle
            // Convert angle to radians for Mathf.Cos/Sin
            float angleRad = launchAngle * Mathf.Deg2Rad;
            Vector2 launchDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            // Apply the force using Impulse mode for an instant push
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);

            // Debug.Log("Tower fired an arrow with force!"); // Updated debug log
        }
        else
        {
            Debug.LogError("TowerShooter: Instantiated Arrow Prefab is missing a Rigidbody2D!");
        }
    }
}