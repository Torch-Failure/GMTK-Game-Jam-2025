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
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            // Replace with actual death
            Debug.Log("Entity died :(");
            currentHealth = 100f;
        }
    }

    public virtual void Attack()
    {
        attackComponent.Attack(transform.rotation);
    }
}
