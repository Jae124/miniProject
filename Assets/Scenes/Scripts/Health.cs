using System;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;
    [SerializeField] private AudioClip[] damageSoundClips;
    private float timer;
    public event Action<int, int> OnHealthChanged;
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
        Debug.Log("Unit destroyed");
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
