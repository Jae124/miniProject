using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Health))] 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class UnitCombat : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackRate = 1.5f;
    [SerializeField] private bool isFighting = false;

    private Health targetEnemyHealth;
    private Health myHealth;
    private Rigidbody2D rb; // Or Rigidbody rb; for 3D
    private UnitMovement unitMovement; // Assuming you have a movement script
    private Coroutine attackCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        myHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        unitMovement = GetComponent<UnitMovement>(); 

        if (unitMovement == null)
        {
            Debug.LogWarning("UnitMovement script not found on " + gameObject.name + ". Stopping behavior might not work correctly.");
        }
    }

    void OnDisable()
    {
        StopFighting();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Attempting Collision Enter on " + gameObject.name);
        // Check if we collided with something that has Health and isn't ourselves
        if (!isFighting && collision.gameObject != gameObject)
        {
            Health potentialTarget = collision.gameObject.GetComponent<Health>();
            // Optional: Check Tag as well
            bool isSame = collision.gameObject.CompareTag(gameObject.tag); // tag check
            
            if (potentialTarget != null && !isSame) 
            {
                StartFighting(potentialTarget);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFighting)
        {
            if (targetEnemyHealth == null || targetEnemyHealth.GetCurrentHealth() <= 0)
            {
                // Target is dead or destroyed
                StopFighting();
            }
        } 
        
    }

    void StartFighting(Health target)
    {
        if (isFighting) return; // Already fighting someone

        isFighting = true;
        targetEnemyHealth = target;
        //Debug.Log(gameObject.name + " started fighting " + target.gameObject.name);

        // 1. Stop Movement
        if (unitMovement != null)
        {
            unitMovement.StopMovement(); // Assumes UnitMovement has a StopMovement method
        }
        else
        {
             // Fallback: Directly stop rigidbody IF no movement script reference
             if (rb != null)
             {
                 rb.linearVelocity = Vector2.zero; // Or Vector3.zero for 3D
             }
        }

        if (attackCoroutine != null) StopCoroutine(attackCoroutine); // Stop previous if any
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    void StopFighting()
    {
        if (!isFighting) return; // Weren't fighting anyway

        isFighting = false;
        //Debug.Log(gameObject.name + " stopped fighting.");

        // 1. Stop Attack Coroutine
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // 2. Resume Movement (Important!)
        if (unitMovement != null)
        {
            unitMovement.StartMovement(); // Assumes UnitMovement has a StartMovement method
        }
        // No need for Rigidbody fallback here, movement script should handle resuming

        targetEnemyHealth = null; // Clear target reference
    }

    IEnumerator AttackRoutine()
    {
        //Debug.Log(gameObject.name + " AttackRoutine started.");
        // Loop while we are supposed to be fighting
        while (isFighting)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(attackRate);

            // Check AGAIN if target is still valid AFTER waiting
            if (isFighting && targetEnemyHealth != null && targetEnemyHealth.GetCurrentHealth() > 0)
            {
                //Debug.Log(gameObject.name + " attacks " + targetEnemyHealth.gameObject.name + " for " + attackDamage + " damage.");
                targetEnemyHealth.TakeDamage(attackDamage);
            }
            else
            {
                // Target died or became invalid during the wait, stop the coroutine
                //Debug.Log(gameObject.name + " Target invalid, stopping AttackRoutine.");
                // Update loop will handle calling StopFighting() formally
                break; // Exit the while loop
            }
        }
        //Debug.Log(gameObject.name + " AttackRoutine finished.");
        attackCoroutine = null; // Clear coroutine reference when it ends
    }

}
