using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!canMove || rb == null)
        {
             // If we shouldn't move, ensure velocity is zero (prevents sliding)
             // Be careful if you NEED vertical velocity for gravity.
             // You might only want to zero out the horizontal component.
             if(rb != null && rb.linearVelocity != Vector2.zero) {
                  rb.linearVelocity = Vector2.zero; // Stop completely
             }
             return; // Don't execute movement logic if stopped
        }
        if (gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.right * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.left * moveSpeed;
        }
    }

    public void StopMovement()
    {
        canMove = false;
        // Optionally force velocity to zero immediately
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Stops completely
        }
        Debug.Log(gameObject.name + " movement stopped.");
    }

    public void StartMovement()
    {
        canMove = true;
        Debug.Log(gameObject.name + " movement resumed.");
        // No need to set velocity here, FixedUpdate will take over
    }

}

    
