using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private AttackComponent attackComponent;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float currentHealth = 100f;

    public float Speed { get { return speed; } }
    public float MaxHealth { get { return maxHealth; } }
    public float CurrentHealth { get { return currentHealth; } }

    public void Move(Vector2 direction)
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public virtual void Attack()
    {
        attackComponent.Attack(transform.rotation);
    }
}
