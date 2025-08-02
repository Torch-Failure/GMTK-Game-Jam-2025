using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> players = new List<GameObject>();
    private List<EnemySpawnData> enemySpawnData = new List<EnemySpawnData>();
    
    [Serializable]
    private class EnemySpawnData
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
        
        public EnemySpawnData(GameObject prefab, Transform transform)
        {
            this.prefab = prefab;
            position = transform.position;
            rotation = transform.rotation;
        }
    }

    void Start()
    {
        RecordEnemyPositions();
    }
    
    private void RecordEnemyPositions()
    {
        enemySpawnData.Clear();

        GetEnemies();

        foreach (GameObject enemyObject in enemies)
        {
            // Store prefab reference by creating a clone of the object
            // This keeps a reference to the original object for instantiation later
            GameObject enemyPrefab = enemyObject;
            enemySpawnData.Add(new EnemySpawnData(enemyPrefab, enemyObject.transform));
        }

        Debug.Log($"Recorded positions for {enemySpawnData.Count} enemies");
    }
    
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
    
    private void UpdateEnemiesPlayerReference()
    {
        GetPlayers();    
        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetPlayers(players);
            }
        }
        Debug.Log($"Updated {enemies.Count} enemies with {players.Count} players");
    }
    
    public void ResetEnemies()
    {
        Debug.Log("Resetting Enemies");
        // Store a temporary list of enemies to destroy
        List<GameObject> enemiesToDestroy = new List<GameObject>(enemies);
        
        // Destroy all existing enemies
        foreach (GameObject enemy in enemiesToDestroy)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        
        // Clear the enemies list since we'll repopulate it
        enemies.Clear();
        
        // Respawn enemies at their initial positions
        foreach (EnemySpawnData data in enemySpawnData)
        {
            // Log the rotation values we're using
            Debug.Log($"Respawning enemy with rotation: {data.rotation.eulerAngles}");
            
            // Instantiate with explicit rotation
            GameObject newEnemy = Instantiate(data.prefab, data.position, data.rotation);
            
            // Double-check the rotation was applied correctly
            Debug.Log($"New enemy rotation: {newEnemy.transform.rotation.eulerAngles}");
            
            // Explicitly set rotation again to ensure it's applied
            newEnemy.transform.rotation = data.rotation;
            
            // Enable the Enemy component
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.enabled = true;
            }
            
            enemies.Add(newEnemy);
        }
        
        // Update the newly spawned enemies with player references
        UpdateEnemiesPlayerReference();
        
        Debug.Log("Deleted and respawned all enemies at initial positions");
    }
}