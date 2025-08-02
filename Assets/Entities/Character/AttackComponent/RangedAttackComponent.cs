using UnityEngine;

public class RangedAttackComponent : AttackComponent
{
    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform projectileSpawnPosition;
    public override void Attack(Quaternion direction)
    {
        Debug.Log("Attack direction: " + direction.eulerAngles);
        GameObject currentProjectile = Instantiate(projectile, projectileSpawnPosition.position, projectileSpawnPosition.rotation);
        Rigidbody2D rb = currentProjectile.GetComponent<Rigidbody2D>();
        rb.AddForce(currentProjectile.transform.up * 500f);
    }
}