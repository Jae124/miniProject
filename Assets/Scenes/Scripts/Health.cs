using System;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float healthMultiplier = 0.2f;
    [SerializeField] private int baseMaxHealth = 100;

    private int maxHealth;
    [SerializeField] private int currHealth;
    [SerializeField] private AudioClip[] damageSoundClips;
    private float timer;
    public event Action<int, int> OnHealthChanged;
    public bool isPlayerBase = false; // Add this line

    public void InitializeForStage(int stageDifficulty)
    {
        // Example: Increase health by 20% for each difficulty level above 1
        float multiplier = 1.0f + (healthMultiplier * (stageDifficulty - 1));
        maxHealth = Mathf.RoundToInt(baseMaxHealth * multiplier);

        // Or simpler linear increase:
        // maxHealth = baseMaxHealth + 10 * (stageDifficulty - 1); // Add 10 HP per difficulty level

        currHealth = maxHealth;
        OnHealthChanged?.Invoke(currHealth, maxHealth);
        // Debug.Log($"{gameObject.name} initialized for Stage Difficulty {stageDifficulty} with Max HP: {maxHealth}");
    }


    void Awake()
    {
        maxHealth = baseMaxHealth;
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
            if (currHealth <= 0) {
                Die();
            }
            timer = 0;
        }
    }

    public void TakeDamage(int attack)
    {
        currHealth -= attack;
        currHealth = Math.Max(0, currHealth);
        OnHealthChanged?.Invoke(currHealth, maxHealth);

        // play sound FX
        if(gameObject.CompareTag("Enemy"))
            SoundFXmanager.Instance.PlayRandomSoundFXClip(damageSoundClips, transform, 0.5f);

        if (currHealth <= 0) Die();
    }

    void Die(){
        // Check if the GameObject has the tag "EnemyBase" or "PlayerBase"
        if (gameObject.CompareTag("EnemyBase"))
        {
        GameManager.Instance.GameOver(true); // Player wins if EnemyBase is destroyed
        }
        else if (gameObject.CompareTag("PlayerBase"))
        {
        GameManager.Instance.GameOver(false); // Player loses if PlayerBase is destroyed
        }
        
        Destroy(gameObject);
    }

    public int GetCurrentHealth() => currHealth;
    public int GetMaxHealth() => maxHealth;
}
