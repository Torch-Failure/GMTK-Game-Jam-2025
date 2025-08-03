using UnityEngine;
using System.Collections.Generic;
using System;

public class ProjectileManager : MonoBehaviour
{
    public class SavedProjectileState
    {

        // Save:
        //  - Speed
        //  - Rotation
        //  - Position
        //  - Tags
        public GameObject projectilePrefab;

        Vector3 position;
        Quaternion rotation;
        string tag;

        Vector2 velocity;
        float angularVelocity;

        int maxLifetimeTicks;
        int currentLifeTicks;

        public SavedProjectileState(RegisteredProjectile registered)
        {
            projectilePrefab = registered.prefab;

            var projectile = registered.projectile;

            tag = projectile.tag;
            position = projectile.transform.position;
            rotation = projectile.transform.rotation;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            velocity = rb.linearVelocity;
            angularVelocity = rb.angularVelocity;
            maxLifetimeTicks = projectile.lifeTimeTicks;
            currentLifeTicks = projectile.currentLifeTime;
        }

        public Projectile LoadProjectileState()
        {
            GameObject newObj = Instantiate(projectilePrefab);
            Projectile projectile = newObj.GetComponent<Projectile>();
            if (projectile == null)
            {
                throw new InvalidOperationException("Tried to create a projectile and it wasn't a projectile");
            }

            projectile.transform.position = position;
            projectile.transform.rotation = rotation;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;  

            projectile.lifeTimeTicks = maxLifetimeTicks;
            projectile.currentLifeTime = currentLifeTicks;
            projectile.tag = tag;
            return projectile;
        }
    };

    public class RegisteredProjectile
    {
        public GameObject prefab;
        public Projectile projectile;

        public RegisteredProjectile(GameObject prefab, Projectile proj)
        {
            this.prefab = prefab;
            this.projectile = proj;
        }
    }

    public List<RegisteredProjectile> projectiles;
    public List<SavedProjectileState> savedProjectiles;

    public static ProjectileManager instance {get; private set;}

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate ProjectileManager found and destroyed.");
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void InitLoop()
    {
        projectiles = new();
        savedProjectiles = new();
    }

    public void SaveLoopStart()
    {
        savedProjectiles.Clear();
        foreach (var proj in projectiles)
        {
            savedProjectiles.Add(new SavedProjectileState(proj));
        }
    }

    public void LoadLoopStart()
    {
        // Destroy all active projectiles
        foreach(var proj in projectiles)
        {
            Debug.Log("killing all projectiles");
            Destroy(proj.projectile.gameObject);
        }

        projectiles.Clear();

        foreach (var savedProj in savedProjectiles)
        {
            Projectile newProj = savedProj.LoadProjectileState();
            RegisteredProjectile reg = new RegisteredProjectile(savedProj.projectilePrefab, newProj);
            projectiles.Add(reg);
        }
    }

    public void RegisterProjectile(GameObject prefab, Projectile projectile)
    {
        if (prefab == null)
        {
            Debug.Log("Prefab is null");
        }
        if (projectile == null)
        {
            Debug.Log("projectile is null");
        }
        if (projectiles == null)
        {
            Debug.Log("Projectile container is null");
        }
        RegisteredProjectile newReg = new RegisteredProjectile(prefab, projectile);
        if (newReg == null)
        {
            Debug.Log("newReg is null");
        }
        projectiles.Add(newReg);
    }

    public void RemoveProjectile(Projectile projectile)
    {
        // Glorious order N removal
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (projectiles[i].projectile == projectile)
            {
                    projectiles.RemoveAt(i);
            }
        }
    }

    public void ThreadPlayingFixedUpdate()
    {
        foreach (var proj in projectiles)
        {
            proj.projectile.ThreadPlayingFixedUpdate();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze()
    {
        foreach (var proj in projectiles)
        {
            var rb = proj.projectile.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                throw new InvalidOperationException("Projectile rigid body not there");
            }

            rb.simulated = false;
        }
    }

    public void Unfreeze()
    {
        foreach (var proj in projectiles)
        {
            var rb = proj.projectile.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                throw new InvalidOperationException("Projectile rigid body not there");
            }

            rb.simulated = true;
        }
    }
}
