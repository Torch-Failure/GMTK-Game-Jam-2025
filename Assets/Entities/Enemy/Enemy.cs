using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private float visionAngle = 30f;
    [SerializeField] private int visionRayCount = 5;
    [SerializeField] private float alertDuration = 2f;

    [SerializeField] private LayerMask obstacleMask;

    private List<GameObject> players = new List<GameObject>();

    private float alertTimer = 0f;

    public enum EnemyState { Idle, Alert, Attacking }

    private EnemyState _currentState = EnemyState.Idle;
    private EnemyState currentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                if (value == EnemyState.Alert) alertTimer = 0f;
            }
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
            case EnemyState.Alert:
                AlertUpdate();
                break;
            case EnemyState.Attacking:
                AttackUpdate();
                break;
        }
    }

    void IdleUpdate()
    {
        if (VisionConeRaycast())
        {
            currentState = EnemyState.Alert;
        }
    }

    void AlertUpdate()
    {
        if (!VisionConeRaycast())
        {
            currentState = EnemyState.Idle;
            return;
        }
        GameObject closestPlayer = GetClosestObject(players);
        rotateTowardsTarget(closestPlayer);
        alertTimer += Time.deltaTime;
        if (alertTimer >= alertDuration)
        {
            currentState = EnemyState.Attacking;
        }
    }

    void AttackUpdate()
    {
        if (!VisionConeRaycast())
        {
            currentState = EnemyState.Alert;
            return;
        }
        GameObject closestPlayer = GetClosestObject(players);
        rotateTowardsTarget(closestPlayer);
        // TODO: Add attack logic here
    }

    
    bool VisionConeRaycast()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.up;
        float halfAngle = visionAngle;

        players = new List<GameObject>();

        for (int i = 0; i < visionRayCount; i++)
        {
            float t = (visionRayCount == 1) ? 0.5f : (float)i / (visionRayCount - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, visionRange, obstacleMask);
            if (hit.collider != null)
            {
                Debug.Log($"Hit: {hit.collider.gameObject.name}");
                if (hit.collider.CompareTag("Player"))
                {
                    players.Add(hit.collider.gameObject);
                }
            }
        }
        return players.Count > 0;
    }

    GameObject GetClosestObject(List<GameObject> objects)
    {
        GameObject closestObject = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (var obj in objects)
        {
            float dist = Vector3.Distance(currentPos, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    void rotateTowardsTarget(GameObject target)
    {
        if (target == null) return;
        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.up;
        float halfAngle = visionAngle;
        int segments = 30;
        float step = (halfAngle * 2) / segments;

        Vector3 prevPoint = transform.position + Quaternion.Euler(0, 0, -halfAngle) * forward * visionRange;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + step * i;
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, 0, angle) * forward * visionRange;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
        Vector3 leftEdge = transform.position + Quaternion.Euler(0, 0, -halfAngle) * forward * visionRange;
        Vector3 rightEdge = transform.position + Quaternion.Euler(0, 0, halfAngle) * forward * visionRange;
        Gizmos.DrawLine(transform.position, leftEdge);
        Gizmos.DrawLine(transform.position, rightEdge);
    }
}