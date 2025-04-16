using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Health))] 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class UnitCombat : MonoBehaviour
{
    [SerializeField] private int baseAttackDamage = 10;
    [SerializeField] private float damageMultiplier = 0.15f; // 15% more damage
    [SerializeField] private int attackDamage = 10;
    public float attackRate = 1.5f;
    [Tooltip("How close the unit needs to be to the target to stop moving and start attacking.")]
    public float attackRange = 1.5f;

    public string opponentUnitTag = "Enemy"; 
    public string opponentBaseTag = "EnemyBase"; 

    [SerializeField] private bool isFighting = false;
    private Health targetHealth;

    private Health myHealth;
    private Rigidbody2D rb; // Or Rigidbody rb; for 3D
    private UnitMovement unitMovement; // Assuming you have a movement script
    private Coroutine attackCoroutine;
    private Transform currentTargetTransform; // Store the target's transform for distance checks

    public void InitializeForStage(int stageDifficulty)
    {
        float multiplier = 1.0f + (damageMultiplier * (stageDifficulty - 1));
        attackDamage = Mathf.RoundToInt(baseAttackDamage * multiplier);
        // Debug.Log($"{gameObject.name} initialized for Stage Difficulty {stageDifficulty} with Attack: {attackDamage}");
    }

    void Awake()
    {
        myHealth = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        unitMovement = GetComponent<UnitMovement>(); 
        attackDamage = baseAttackDamage;

        if (unitMovement == null)
        {
            Debug.LogWarning("UnitMovement script not found on " + gameObject.name + ". Stopping behavior might not work correctly.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        //Debug.Log(gameObject.name + " detected trigger/collision with: " + other.gameObject.name + " [Tag: " + other.gameObject.tag + "]");
        HandlePotentialTarget(other.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
     {
         // Only try to acquire a new target if we aren't already fighting
         // and don't currently have a valid target transform assigned
         if (!isFighting && currentTargetTransform == null)
         {
             HandlePotentialTarget(other.gameObject);
         }
     }

     void OnTriggerExit2D(Collider2D other) {
         if (currentTargetTransform != null && other.transform == currentTargetTransform) {
             // If the object leaving IS our current target, clear it
             // This doesn't stop combat immediately if already fighting, Update handles range check.
             Debug.Log($"{gameObject.name} target {other.name} exited trigger range.");
             currentTargetTransform = null;
              // Optionally, if NOT fighting, tell movement script target is gone
              if (!isFighting && unitMovement != null) {
                  unitMovement.ClearTarget(); // Assumes UnitMovement has ClearTarget method
              }
         }
     }

    void OnDisable()
    {
        StopFighting();
    }

    private void HandlePotentialTarget(GameObject detectedObject)
    {
        // Ignore self, or if we ALREADY have a target we are pursuing/fighting
        // We only acquire the *first* valid target we detect for simplicity now.
        if (currentTargetTransform != null || detectedObject == gameObject) return;

        // Check if the detected object is a valid opponent
        if (detectedObject.CompareTag(opponentUnitTag) || detectedObject.CompareTag(opponentBaseTag))
        {
            Health potentialTarget = detectedObject.GetComponent<Health>();

            // Check if it has health and is alive
            if (potentialTarget != null && potentialTarget.GetCurrentHealth() > 0)
            {
                // Found a potential target! Store its transform.
                currentTargetTransform = detectedObject.transform;
                 Debug.Log($"{gameObject.name} acquired potential target: {currentTargetTransform.name}");

                // Tell the movement script to move towards this target
                if (unitMovement != null)
                {
                    unitMovement.SetTarget(currentTargetTransform); // Assumes UnitMovement has SetTarget
                }
            }
        }
    }

    void Update()
    {
        if (isFighting) // --- If currently in combat ---
        {
            // 1. Check if target died or was destroyed
            if (targetHealth == null || targetHealth.GetCurrentHealth() <= 0)
            {
                Debug.Log($"{gameObject.name}: Target {currentTargetTransform?.name ?? "Unknown"} died or was destroyed. Stopping fight.");
                StopFighting();
                return; // Exit update for this frame
            }

            // 2. Check if target moved OUT of attack range
            float distanceToTarget = Vector2.Distance(transform.position, currentTargetTransform.position);
            if (distanceToTarget > attackRange)
            {
                 Debug.Log($"{gameObject.name}: Target {currentTargetTransform.name} moved out of range ({distanceToTarget} > {attackRange}). Stopping fight.");
                StopFighting();
                // Note: Movement will resume automatically because StopFighting calls StartMovement
            }
        }
        else // --- If NOT currently in combat ---
        {
            // Check if we have a valid potential target assigned
            if (currentTargetTransform != null)
            {
                // Check if the potential target died before we engaged
                 Health potentialTargetHealth = currentTargetTransform.GetComponent<Health>(); // Get health again in case it changed
                 if(potentialTargetHealth == null || potentialTargetHealth.GetCurrentHealth() <= 0) {
                      Debug.Log($"{gameObject.name}: Potential target {currentTargetTransform.name} died before engagement. Clearing target.");
                      ClearCurrentTarget(); // Clear target if it's dead
                      return; // Exit update
                 }

                // Check if the potential target is NOW in attack range
                float distanceToTarget = Vector2.Distance(transform.position, currentTargetTransform.position);
                 // Debug.Log($"{gameObject.name}: Distance to {currentTargetTransform.name} = {distanceToTarget}. Attack range = {attackRange}"); // Spammy log for debugging distance
                if (distanceToTarget <= attackRange)
                {
                    // Engage!
                    StartFighting(potentialTargetHealth); // Start fighting this target
                }
                // else: Keep moving towards it (handled by UnitMovement)
            }
            // else: No target, keep moving based on UnitMovement's default behavior (if any)
        }
    }

    void StartFighting(Health target)
    {
        // Basic checks
        if (isFighting || target == null || target.gameObject == this.gameObject) return;

        isFighting = true;
        targetHealth = target; // Store the health component of the engaged target
        currentTargetTransform = target.transform; // Ensure currentTargetTransform is set

        Debug.Log($"{gameObject.name} [Tag:{gameObject.tag}] ENGAGING {target.gameObject.name} [Tag:{target.gameObject.tag}] at range.");

        // Stop Movement
        if (unitMovement != null) unitMovement.StopMovement();
        else if (rb != null) rb.linearVelocity = Vector2.zero;

        // Start Attack Coroutine
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    void StopFighting()
    {
        if (!isFighting) return;
        isFighting = false;
        string previousTargetName = (currentTargetTransform != null) ? currentTargetTransform.name : ((targetHealth != null) ? targetHealth.gameObject.name : "Unknown");
        Debug.Log($"{gameObject.name} stopped fighting {previousTargetName}");

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        // Do NOT clear currentTargetTransform here IF the target is still alive but just out of range.
        // The unit should resume moving towards it.
        // Only clear the target if it's dead or gone (handled in Update or OnTriggerExit)

        // Resume Movement (UnitMovement will use currentTargetTransform if it's still set)
        if (unitMovement != null) unitMovement.StartMovement();

        targetHealth = null; // Clear the specific health component we were attacking
    }

    // --- Helper to fully clear target ---
    private void ClearCurrentTarget() {
        currentTargetTransform = null;
        targetHealth = null; // Ensure combat target is also cleared
        if(unitMovement != null) {
            unitMovement.ClearTarget(); // Tell movement script target is gone
        }
    }

    IEnumerator AttackRoutine()
    {
        // Renamed target parameter to avoid conflict with class member
        Health initialTarget = targetHealth; // Use the currently set targetHealth

        if (initialTarget == null || initialTarget.GetCurrentHealth() <= 0)
        {
             Debug.Log($"{gameObject.name}: Target invalid at AttackRoutine start.");
             // Update loop should handle calling StopFighting(), just exit coroutine
             yield break;
        }
        Debug.Log($"{gameObject.name}: AttackRoutine started against {initialTarget.gameObject.name}");

        while (isFighting) // Loop while the UNIT is in fighting state
        {
            // Check if the target we are locked onto (targetHealth) is still valid
             if (targetHealth == null || targetHealth.GetCurrentHealth() <= 0)
             {
                 Debug.Log($"{gameObject.name}: Target {initialTarget.gameObject.name} became invalid during AttackRoutine loop (before wait).");
                 break; // Exit loop, Update will call StopFighting
             }

            // We don't necessarily need to re-check range *within* the attack loop,
            // as the Update loop handles stopping combat if range is broken.
            // But we could add an optional check here for responsiveness.

            yield return new WaitForSeconds(attackRate);

            // Check target validity AGAIN AFTER waiting (and ensure still fighting state)
            if (isFighting && targetHealth != null && targetHealth.GetCurrentHealth() > 0)
            {
                // Ensure we are still in attack range (Optional check for robustness)
                // float distance = Vector2.Distance(transform.position, targetHealth.transform.position);
                // if (distance <= attackRange) {
                    Debug.Log($"{gameObject.name} attacks {targetHealth.gameObject.name} for {attackDamage} damage.");
                    targetHealth.TakeDamage(attackDamage);
                // } else {
                //    Debug.Log($"{gameObject.name} target {targetHealth.gameObject.name} moved out of range during attack windup.");
                //    // Don't break here, let Update handle StopFighting to resume movement
                // }
            }
            else
            {
                 // Check if we stopped fighting during the yield
                 if (!isFighting) {
                     Debug.Log($"{gameObject.name}: Stopped fighting during AttackRoutine yield.");
                 } else {
                     Debug.Log($"{gameObject.name}: Target {initialTarget.gameObject.name} became invalid during AttackRoutine loop (after wait).");
                 }
                 break; // Exit loop, Update will call StopFighting if needed
            }
        }
        Debug.Log($"{gameObject.name}: AttackRoutine finished for target {initialTarget.gameObject.name}.");
        attackCoroutine = null;
    }

}
