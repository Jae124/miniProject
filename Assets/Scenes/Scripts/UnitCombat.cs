using UnityEngine;

[RequireComponent(typeof(UnitHealth))] 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class UnitCombat : MonoBehaviour
{
    public int attackDamage = 10;
    public float attackRate = 1.5f;
    [SerializeField] private bool isFighting = false;

    private UnitHealth targetEnemyHealth;
    private UnitHealth myHealth;
    private Rigidbody2D rb; // Or Rigidbody rb; for 3D
    private UnitMovement unitMovement; // Assuming you have a movement script
    private Coroutine attackCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        myHealth = GetComponent<UnitHealth>();
        rb = GetComponent<Rigidbody2D>();
        unitMovement = GetComponent<UnitMovement>(); 

        if (unitMovement == null)
        {
            Debug.LogWarning("UnitMovement script not found on " + gameObject.name + ". Stopping behavior might not work correctly.");
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
