using System;
using UnityEngine;

public class RangedAttackComponent : AttackComponent
{
    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform projectileSpawnPosition;

    public override void Attack(Quaternion direction)
    {
        GameObject currentProjectile = Instantiate(projectile, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
        currentProjectile.tag = transform.parent.gameObject.tag;

        Projectile projectileScript = currentProjectile.GetComponent<Projectile>();
        if (projectileScript == null)
        {
            throw new InvalidOperationException("Projectile was not a projectile");
        }

        var projectileManager = ProjectileManager.instance;
        if (projectileManager == null)
        {
            throw new InvalidOperationException("No projectile manager set!");
        }

        projectileManager.RegisterProjectile(projectile, projectileScript);
    }
}