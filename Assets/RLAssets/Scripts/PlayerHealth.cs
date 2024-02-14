using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField]
    PlayerAgent pAgent;
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Bullet Hit with Player");
        // Check if the player's health has reached zero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle player death (e.g., play death animation, respawn, etc.)
        Debug.Log("Player has died!");
        pAgent.AddReward(-1);
        pAgent.EndEpisode();
        // For simplicity, you can reload the level or perform other actions here.
    }
}
