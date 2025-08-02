using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> players = new List<GameObject>();

    void GetEnemies()
    {
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        Debug.Log($"Found {enemies.Count} enemies");
    }

    void GetPlayers()
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        Debug.Log($"Found {players.Count} players");
    }
}