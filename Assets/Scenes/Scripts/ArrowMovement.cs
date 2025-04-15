using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    // [Tooltip("How fast the arrow travels.")]
    // public float speed = 10f;

    [Tooltip("How long the arrow lives before being destroyed (seconds).")]
    public float lifetime = 5f;

    private Rigidbody2D rb; // Store Rigidbody reference



    void Awake() // Use Awake to get references reliably
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("ArrowMovement: Rigidbody2D component not found!");
        }
    }

    void Start()
    {
        // Destroy the arrow after 'lifetime' seconds to prevent clutter
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the arrow to the right based on its local orientation
        // Using Translate is simple for Kinematic bodies or when ignoring physics forces.
        // transform.Translate(Vector3.right * speed * Time.deltaTime);


        // --- Optional: Orient Arrow with Velocity ---
        if (rb != null && rb.linearVelocity != Vector2.zero) // Check velocity isn't zero to avoid errors
        {
            // Calculate the angle based on the velocity direction
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            // Apply the rotation (adjust if your sprite's default orientation isn't 'right')
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

        

    // --- Collision Detection ---
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the arrow hit something it should interact with (e.g., an enemy)
        // Replace "EnemyTag" with the actual tag you give your enemy units.

        // --- Check for Ground Collision FIRST ---
        // Assumes your ground has the "Ground" layer assigned.
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Arrow hit the ground.");
            // Optionally: Add a small effect here (puff of dust particle)
            Destroy(gameObject); // Destroy arrow immediately
            return; // Stop further checks if it hit the ground
        }

        
        if (other.CompareTag("Enemy")) // Make sure your enemies have the "Enemy" tag!
        {
            Debug.Log("Arrow hit an enemy!");
            // --- Add damage logic here ---
            // Example: Get an EnemyHealth component and call a TakeDamage method
            // EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage(10); // Deal 10 damage, for example
            // }

            // Destroy the arrow immediately after hitting an enemy
            Destroy(gameObject);
        }
        // Optional: Add checks for hitting other things, like the enemy tower
        // else if (other.CompareTag("EnemyTower")) { ... Destroy(gameObject); }
    }
}