using System;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;
    private float timer;
    public event Action<int, int> OnHealthChanged;

    [Tooltip("Check this box if this Health component is attached to the Player's Base.")]
    public bool isPlayerBase = false; // Add this line

    void Awake()
    {
        currHealth = maxHealth;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        if (GameManager.Instance != null)
        {
            //Determine if the player won based on which base died. 
            // if this is not the player base dying, the player wins. 
            bool playerVictory = !isPlayerBase;
            GameManager.Instance.GameOver(playerVictory);
        }
        else{
            Debug.LogError("Cannot call GameOver: GameManager instance not found!");
        }

        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currHealth;
    public int GetMaxHealth() => maxHealth;
}
