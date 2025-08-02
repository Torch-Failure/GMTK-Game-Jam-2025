using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    private float visionRange = 5f;

    [SerializeField]
    private float visionAngle = 30f;

    [SerializeField]
    private float alertDuration = 2f;

    private GameObject currentTarget;
    private List<GameObject> players = new List<GameObject>();

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
        // Debug.Log("Enemy is idle, searching for players.");
        GameObject closest = GetClosestPlayer();
        // Debug.Log("Closest Player: " + closest);
        if (closest != null && CanSeeTarget(closest))
        {
            currentTarget = closest;
            currentState = EnemyState.Alert;
            alertTimer = 0f;
        }
    }

    void HandleAlertState()
    {
        // Debug.Log("Enemy is alert, waiting for " + alertDuration + " seconds before attacking.");
        if (currentTarget == null || !CanSeeTarget(currentTarget))
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
        // Debug.Log("Enemy is attacking the target.");
        if (currentTarget == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        if (!CanSeeTarget(currentTarget))
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
            if (dist < minDist)
            {
                minDist = dist;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    bool TargetWithinVisionAngle(GameObject target)
    {
        if (target == null) return false;

        Vector3 directionToTarget = target.transform.position - transform.position;
        directionToTarget.Normalize();

        Vector3 forward = transform.up;

        float angle = Vector3.Angle(forward, directionToTarget);

        return angle <= visionAngle;
    }

    bool TargetWithinVisionRange(GameObject target)
    {
        if (target == null) return false;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= visionRange;
    }

    bool CanSeeTarget(GameObject target)
    {
        if (target == null) return false;

        return TargetWithinVisionRange(target) && TargetWithinVisionAngle(target);
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
    
    public void SetPlayers(List<GameObject> playersList)
    {
        players = playersList;
        Debug.Log($"Enemy received {players.Count} players");
    }
}