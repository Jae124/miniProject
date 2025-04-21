using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))] 
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform targetTransform; // Set by UnitCombat script

    private Rigidbody2D rb;
    private Animator animator; 
    private bool canMove = true;

    void Awake()
    {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) Debug.LogError("Animator component missing!", this);
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
            if (!canMove && animator != null)
            {
                 animator.SetBool("isMoving", false);
            }
            return; // Don't execute movement logic if stopped
        }

        bool isCurrentlyMoving = false;
        if (targetTransform != null) // Or however you decide to move
        {
             // Move towards target logic...
             // rb.velocity = ...;

             // Check if velocity magnitude is significant enough to count as moving
             // Threshold helps ignore tiny drifts or physics jitter
             if (rb.linearVelocity.sqrMagnitude > 0.01f) // Use squared magnitude for efficiency
             {
                isCurrentlyMoving = true;
             }
        }

        if (targetTransform != null)
        {
            // Move towards the assigned target
            Vector2 direction = ((Vector2)targetTransform.position - rb.position).normalized;
            Vector2 targetVelocity = direction * moveSpeed;
            // Preserve Y velocity for gravity, adjust X velocity
            rb.linearVelocity = new Vector2(targetVelocity.x, rb.linearVelocity.y);
             // Or rb.velocity = targetVelocity; // if no gravity
        }
        else
        {
            if (gameObject.CompareTag("Player"))
            {
                rb.linearVelocity = Vector2.right * moveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.left * moveSpeed;
            }
        }
        if (animator != null)
        {
            animator.SetBool("isMoving", isCurrentlyMoving);
        }
    }

    public void StopMovement()
    {
        canMove = false;
        // Optionally force velocity to zero immediately
        if (animator != null) animator.SetBool("isMoving", false);
        if (rb != null) rb.linearVelocity = Vector2.zero; // Stops completely
        //Debug.Log(gameObject.name + " movement stopped.");
    }

    public void StartMovement()
    {
        canMove = true;
        //Debug.Log(gameObject.name + " movement resumed.");
        // No need to set velocity here, FixedUpdate will take over
    }

    // --- Methods for Combat Script to set/clear target ---
    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
         // Debug.Log($"{gameObject.name} movement target set to {newTarget?.name ?? "null"}");
    }

    public void ClearTarget()
    {
        targetTransform = null;
         // Debug.Log($"{gameObject.name} movement target cleared.");
    }

}

    
