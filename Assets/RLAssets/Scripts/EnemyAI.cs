using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 10f;
    public float chaseRange = 5f;
    public float shootingRange = 3f;
    public float shootingCooldown = 1f;
    public Transform player;
    public LayerMask playerLayer;
    public Transform gunTransform;
    public int damage = 10;

    private NavMeshAgent navMeshAgent;
    private float shootingTimer;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        LayerMask playerLayer = LayerMask.GetMask("Player");
        shootingTimer = shootingCooldown;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Player is in detection range, start chasing
            ChasePlayer();

            // Check if the player is in shooting range
            if (distanceToPlayer <= shootingRange)
            {
                // Player is in shooting range, start shooting
                ShootPlayer();
            }
            else
            {
                // Player is out of shooting range, stop shooting animation or actions
                // Add any other logic you may need
            }
        }
        else
        {
            // Player is not in detection range, stop chasing
            StopChasing();
        }
    }

    private void ChasePlayer()
    {
        // Set the destination for the NavMeshAgent
        navMeshAgent.SetDestination(player.position);

        // Face the player while chasing
        FacePlayer();

        // Check if the enemy is within stopping distance
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance+3)
        {
            // Stop the NavMeshAgent from moving
            navMeshAgent.ResetPath();
        }
    }

    private void StopChasing()
    {
        // Stop the NavMeshAgent from moving
        navMeshAgent.ResetPath();
    }

    private void ShootPlayer()
    {
        // Check if shooting cooldown is over
        if (shootingTimer <= 0f)
        {
            // Perform raycast to check if player is in line of sight
            RaycastHit hit;
            Debug.DrawRay(gunTransform.position, gunTransform.forward * shootingRange, Color.red);
            if (Physics.Raycast(gunTransform.position, gunTransform.forward, out hit, shootingRange, playerLayer))
            {
                // Player is in line of sight, reduce player's health
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                Debug.Log("Hit with Player");
                // Reset shooting cooldown
                shootingTimer = shootingCooldown;
            }
        }
        else
        {
            // Reduce shooting cooldown
            shootingTimer -= Time.deltaTime;
        }
    }

    private void FacePlayer()
    {
        // Face the player while shooting
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep the rotation only in the horizontal plane
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * navMeshAgent.angularSpeed);
    }
}
