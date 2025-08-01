using UnityEngine;

public class RangedAttackComponent : AttackComponent
{
    public override void Attack(Quaternion direction) {
        Debug.Log("Attack direction: " + direction.eulerAngles);
        // Spawn a projectile and then launch it in the direction.
    }
}