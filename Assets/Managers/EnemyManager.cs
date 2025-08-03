using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public class SavedEnemyState
    {
        Vector3 position;
        Quaternion rotation;
        float alertTimer;
        int patrolNodeId;
        Enemy.EnemyState state;

        public SavedEnemyState(Enemy enemy)
        {
            position = enemy.transform.position;
            rotation = enemy.transform.rotation;
            alertTimer = enemy.alertTimer;
            patrolNodeId = enemy.patrolNodeId;
            state = enemy.currentState;
        }

        public void Load(Enemy enemy)
        {
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.alertTimer = alertTimer;
            enemy.patrolNodeId = patrolNodeId;
            enemy.currentState = state        ;    
        }


    };

    // Should be all enemies, not just alive or dead
    Enemy[] activeEnemies;
    SavedEnemyState[] savedStates;

    public void InitLoop()
    {
        activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        savedStates = new SavedEnemyState[activeEnemies.Length];
        SaveLoopStart();
    }

    public void ThreadPlayingFixedUpdate()
    {
        foreach (var enemy in activeEnemies)
        {
            enemy.ThreadPlayingFixedUpdate();
        }
    }

    public void SaveLoopStart()
    {
        if (activeEnemies.Length != savedStates.Length)
        {
            throw new InvalidOperationException("Must have same number of saveds states as enemies!!!!!!!!!");
        }

        for (int i = 0; i < activeEnemies.Length; i++)
        {
            savedStates[i] = new SavedEnemyState(activeEnemies[i]);
        }
    }

    public void LoadLoopStart()
    {
        if (activeEnemies.Length != savedStates.Length)
        {
            throw new InvalidOperationException("Must have same number of saveds states as enemies!!!!!!!!!");
        }

        for (int i = 0; i < activeEnemies.Length; i++)
        {
            savedStates[i].Load(activeEnemies[i]);            
        }
    }

    public void ThreadPlayingFixedUpdate(int loopTick)
    {
        // Do nothing yet
    }



}