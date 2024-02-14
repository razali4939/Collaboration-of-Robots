using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private GameObject collectable;
    private GameObject[] collectables;

    public float shootingRange = 10f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject enemy;

    private int damage = 10;
    private Spawner spawn;
    [SerializeField] private PlayerHealth playerHealth;

    public void Awake()
    {
        spawn = FindObjectOfType<Spawner>();
        initialPosition = this.transform.position;
    }

    public override void Initialize()
    {
        this.transform.position = initialPosition;
    }

    public override void OnEpisodeBegin()
    {
        this.transform.position = initialPosition;
        spawn.Spawn();
        playerHealth.maxHealth = 100;
        playerHealth.currentHealth = 100;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(GetComponent<Rigidbody>().velocity);
        sensor.AddObservation(shootingRange);
        sensor.AddObservation(collectable.transform.position);
        sensor.AddObservation(playerHealth.currentHealth);
        sensor.AddObservation(enemy.transform.position);
        sensor.AddObservation(enemy.transform.rotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var discreteActions = actions.DiscreteActions;

        float moveHorizontal = discreteActions[0];
        float moveVertical = discreteActions[1];
        float rotateAction = discreteActions[2];
        float shootAction = discreteActions[3];

        Vector3 moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Rotate the agent based on the action received
        transform.Rotate(Vector3.up * rotateAction * 5f);

        if (shootAction == 1)
        {
            Shoot();
        }

        AddReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
        discreteActionsOut[1] = Mathf.RoundToInt(Input.GetAxis("Vertical"));
        discreteActionsOut[2] = Mathf.RoundToInt(Input.GetAxis("Rotate"));//GetAxis("Rotate")); // Use a dedicated input axis for rotation
        discreteActionsOut[3] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, shootingRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy Hit!");

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }

                AddReward(1f);
            }
        }
    }

    private void Update()
    {
        collectables = GameObject.FindGameObjectsWithTag("collectable");
        if (collectables.Length <= 0)
        {
            AddReward(1f);
            Debug.Log("All Collected");
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("collectable"))
        {
            Destroy(collision.gameObject, 0.5f);
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            AddReward(-0.1f);
        }
    }
}
