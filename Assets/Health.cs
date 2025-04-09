using System;
using Unity.Jobs;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int maxHealth = 100;
    [SerializeField] private int currHealth;
    public LogicScript logic;
    private float timer;
    public event Action<int, int> OnHealthChanged;

    void Awake()
    {
        currHealth = maxHealth;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        OnHealthChanged?.Invoke(currHealth, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth <= 0) return;

        if (timer < 0.5) {
            timer += Time.deltaTime;
        }
        else 
        {
            TakeDamage(1);
            if (currHealth <= 0) {
                Die();
            }
            timer = 0;
        }
    }

    void TakeDamage(int attack)
    {
        currHealth -= attack;
        currHealth = Math.Max(0, currHealth);
        OnHealthChanged?.Invoke(currHealth, maxHealth);
    }

    void Die(){
        Debug.Log("Base destroyed");
        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currHealth;
    public int GetMaxHealth() => maxHealth;
}
