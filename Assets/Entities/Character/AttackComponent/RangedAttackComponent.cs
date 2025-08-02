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
    }
}