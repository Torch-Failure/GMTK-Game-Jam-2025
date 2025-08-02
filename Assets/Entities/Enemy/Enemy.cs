using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum State
    {   
        Idle,
        Patrolling
    };  

    private State state = State.Patrolling;
    private NavMeshAgent navMeshAgent;
    private int patrolNodeIndex = 0;

    public PatrolRoute patrolRoute;
    public float speed = 0;


    // Handle behaviour 

    // State:
    //  - Patrolling
    //  - Chilling
    //  - 
    //
    //
    //
    //


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.updateUpAxis = false; // Important for 2D!
        navMeshAgent.updateRotation = false;
        GoToNextPatrolPoint();
        // navMeshAgent.autoBraking = false; // Try with and without
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Patrolling:
                Debug.Log("Patrollling...");
                Patrol();
                break;
        }
    }

    void Patrol()
    {
        Debug.Log($"Distance: {navMeshAgent.remainingDistance}");   
        Debug.Log($"Target: {navMeshAgent.destination}");   

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
        else
        {
            navMeshAgent.destination = patrolRoute.points[patrolNodeIndex].position;
        }
    }

    void GoToNextPatrolPoint()
    {
        patrolNodeIndex++;
        if (patrolNodeIndex > patrolRoute.points.Length)
        {
            patrolNodeIndex = 0;
        }
        navMeshAgent.destination = patrolRoute.points[patrolNodeIndex].position;
    }
}
