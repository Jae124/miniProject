using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Health))] 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class UnitCombat : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackRate = 1.5f;

    public string opponentUnitTag = "Enemy"; 
    public string opponentBaseTag = "EnemyBase"; 

    [SerializeField] private bool isFighting = false;
    private Health targetHealth;

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

    void OnTriggerEnter2D(Collider2D other)
    {   
        Debug.Log(gameObject.name + " detected trigger/collision with: " + other.gameObject.name + " [Tag: " + other.gameObject.tag + "]");
        HandleDetection(other.gameObject);
    }

    void OnDisable()
    {
        StopFighting();
    }


    private void HandleDetection(GameObject detectedObject)
    {
        // Ignore self, or if already fighting
        if (isFighting || detectedObject == gameObject) return;

        // Check if the detected object has a tag we should attack (Unit OR Base)
        Debug.Log(!detectedObject.CompareTag(gameObject.tag));

        if (!detectedObject.CompareTag(gameObject.tag))
        {
            Debug.Log(detectedObject);
            Health potentialTarget = detectedObject.GetComponent<Health>();

            // Check if it has health and is alive
            if (potentialTarget != null && potentialTarget.GetCurrentHealth() > 0)
            {
                Debug.Log("Fight start");
                StartFighting(potentialTarget);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFighting)
        {
            if (targetHealth == null || targetHealth.GetCurrentHealth() <= 0)
            {
                // Target is dead or destroyed
                StopFighting();
            }
        } 
        
    }

    void StartFighting(Health target)
    {
        if (isFighting || target == null || target.gameObject == this.gameObject) return;

        isFighting = true;
        targetHealth = target;
        Debug.Log($"{gameObject.name} [Tag:{gameObject.tag}] started fighting {target.gameObject.name} [Tag:{target.gameObject.tag}]");

        // Stop Movement
        if (unitMovement != null) unitMovement.StopMovement();
        else if (rb != null) rb.linearVelocity = Vector2.zero; // Fallback

        // Start Attack Coroutine
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackRoutine());
    }


    void StopFighting()
    {
        if (!isFighting) return; // Weren't fighting anyway

        isFighting = false;
        //Debug.Log(gameObject.name + " stopped fighting.");
        string previousTargetName = (targetHealth != null) ? targetHealth.gameObject.name : "Unknown";
        Debug.Log($"{gameObject.name} stopped fighting {previousTargetName}");


        // 1. Stop Attack Coroutine
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // 2. Resume Movement (Important!)
        if (unitMovement != null) unitMovement.StartMovement(); // Assumes UnitMovement has a StartMovement method

        targetHealth = null; // Clear target reference
    }

    IEnumerator AttackRoutine()
    {
        if (targetHealth == null || targetHealth.GetCurrentHealth() <= 0)
        {
             Debug.Log($"{gameObject.name}: Target invalid at AttackRoutine start.");
             StopFighting();
             yield break;
        }
        Debug.Log($"{gameObject.name}: AttackRoutine started against {targetHealth.gameObject.name}");

        // Loop while we are supposed to be fighting
        while (isFighting)
        {
            // Check target validity BEFORE waiting
            if (targetHealth == null || targetHealth.GetCurrentHealth() <= 0) {
                Debug.Log($"{gameObject.name}: Target invalid during AttackRoutine loop (before wait).");
                break;
            }

            // Wait for the specified interval
            yield return new WaitForSeconds(attackRate);

            // Check target validity AGAIN AFTER waiting (and ensure still fighting)
            if (isFighting && targetHealth != null && targetHealth.GetCurrentHealth() > 0)
            {
                Debug.Log($"{gameObject.name} attacks {targetHealth.gameObject.name} for {attackDamage} damage.");
                targetHealth.TakeDamage(attackDamage);
            }
            else
            {
                // Target died or became invalid during the wait, stop the coroutine
                Debug.Log($"{gameObject.name}: Target invalid during AttackRoutine loop (after wait).");
                // Update loop will handle calling StopFighting() formally
                break; // Exit the while loop
            }
        }
        Debug.Log(gameObject.name + " AttackRoutine finished.");
        attackCoroutine = null; // Clear coroutine reference when it ends
    }

}
