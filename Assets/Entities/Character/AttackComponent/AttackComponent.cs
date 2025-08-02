using UnityEngine;

public abstract class AttackComponent : MonoBehaviour
{
    [SerializeField]
    private int attackSpeed;
    public abstract void Attack(Quaternion direction);
}