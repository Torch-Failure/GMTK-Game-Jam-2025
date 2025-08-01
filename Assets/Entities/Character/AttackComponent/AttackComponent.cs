using UnityEngine;

public abstract class AttackComponent : MonoBehaviour
{
    public abstract void Attack(Quaternion direction);
}