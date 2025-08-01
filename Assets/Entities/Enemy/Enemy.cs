using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    private float visionRange = 5f;

    [SerializeField]
    private float alertDuration = 2f; // Time to wait before attacking

    private GameObject[] players;
    private GameObject currentTarget;

    private EnemyState currentState = EnemyState.Idle;
    private float alertTimer = 0f;

    public enum EnemyState
    {
        Idle,
        Alert,
        Attacking
    }

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Enemy initialized with " + players.Length + " players in the scene.");
    }

    void Update()
    {
        StateMachineUpdate();
    }

    void StateMachineUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Alert:
                HandleAlertState();
                break;
            case EnemyState.Attacking:
                HandleAttackingState();
                break;
        }
    }

    void HandleIdleState()
    {
        Debug.Log("Enemy is idle, searching for players.");
        GameObject closest = GetClosestPlayer();
        if (closest != null)
        {
            currentTarget = closest;
            currentState = EnemyState.Alert;
            alertTimer = 0f;
        }
    }

    void HandleAlertState()
    {
        Debug.Log("Enemy is alert, waiting for " + alertDuration + " seconds before attacking.");
        if (currentTarget == null || !TargetWithinRange(currentTarget, visionRange))
        {
            currentState = EnemyState.Idle;
            currentTarget = null;
            return;
        }

        rotateTowardsTarget(currentTarget);
        alertTimer += Time.deltaTime;
        if (alertTimer >= alertDuration)
        {
            currentState = EnemyState.Attacking;
        }
    }

    void HandleAttackingState()
    {
        Debug.Log("Enemy is attacking the target.");
        if (currentTarget == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        if (!TargetWithinRange(currentTarget, visionRange))
        {
            currentState = EnemyState.Alert;
            alertTimer = 0f;
            return;
        }

        rotateTowardsTarget(currentTarget);
        // Add attacking logic here (e.g., shooting, chasing, etc.)
    }

    GameObject GetClosestPlayer()
    {
        GameObject closestPlayer = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (var player in players)
        {
            float dist = Vector3.Distance(currentPos, player.transform.position);
            if (dist < minDist && dist <= visionRange)
            {
                minDist = dist;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    bool TargetWithinRange(GameObject target, float range)
    {
        if (target == null) return false;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= range;
    }

    void rotateTowardsTarget(GameObject target)
    {
        if (target == null) return;

        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}