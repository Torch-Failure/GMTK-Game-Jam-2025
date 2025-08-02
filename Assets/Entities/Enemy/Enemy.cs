using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private float visionAngle = 30f;
    [SerializeField] private int visionRayCount = 5;
    [SerializeField] private float alertDuration = 2f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] public float patrolNodeDistanceTolerance = 0.1f; // Distance to patrol node which counts as reaching
    [SerializeField] private EnemyPatrolRoute patrolRoute; 
    [SerializeField] private EnemyState defaultState = EnemyState.Idle;

    [SerializeField] private LayerMask obstacleMask;

    private float alertTimer = 0f;
    private int patrolNodeId  = 0;


    public enum EnemyState { Idle, Patrolling, Alert, Attacking }

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

    void Start()
    {
        currentState = defaultState;
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
            case EnemyState.Patrolling:
                PatrolUpdate();
                break;
        }
    }

    void CheckIfAlerted()
    {
        if (VisionConeRaycast().Count > 0)
        {
            currentState = EnemyState.Alert;
        }
    }

    void IdleUpdate()
    {
        CheckIfAlerted();
    }

    void PatrolUpdate()
    {
        if (patrolRoute == null)
        {
            defaultState = EnemyState.Idle;
            currentState = EnemyState.Idle;
            return;
        }   

        var currentNode = patrolRoute.nodes[patrolNodeId];
        Vector3 toNode = currentNode.position - transform.position;
        Move(toNode);
        rotateTowardsDirection(toNode);
        
        if( toNode.magnitude < patrolNodeDistanceTolerance )
        {
            patrolNodeId++;
            patrolNodeId %= patrolRoute.nodes.Length;
        } 

        CheckIfAlerted();
    }

    void AlertUpdate()
    {
        var players = VisionConeRaycast();
        if (players.Count == 0)
        {
            currentState = defaultState;
            return;
        }
        GameObject closestPlayer = Helpers.GetClosestObject(players, transform.position);
        rotateTowardsTarget(closestPlayer);
        alertTimer += Time.deltaTime;
        if (alertTimer >= alertDuration)
        {
            currentState = EnemyState.Attacking;
        }
    }

    void AttackUpdate()
    {
        var players = VisionConeRaycast();
        if (players.Count == 0)
        {
            currentState = EnemyState.Alert;
            return;
        }
        GameObject closestPlayer = Helpers.GetClosestObject(players, transform.position);
        rotateTowardsTarget(closestPlayer);
        // TODO: Add attack logic here
        Attack();
    }

    
    List<GameObject> VisionConeRaycast()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.up;
        float halfAngle = visionAngle;

        var players = new List<GameObject>();

        for (int i = 0; i < visionRayCount; i++)
        {
            float t = (visionRayCount == 1) ? 0.5f : (float)i / (visionRayCount - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, obstacleMask);
            if (hit.collider != null)
            {
                Debug.Log($"S: {hit.collider.gameObject.name}");
                if (hit.collider.CompareTag("Player"))
                {
                    players.Add(hit.collider.gameObject);
                }
            }
        }
        return players;
    }

    void rotateTowardsTarget(GameObject target)  // FIXME: This can go to a helper function if we have time.
    {
        if (target == null) return;
        Vector3 dir = target.transform.position - transform.position;
        rotateTowardsDirection(dir);
    }

    void rotateTowardsDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
