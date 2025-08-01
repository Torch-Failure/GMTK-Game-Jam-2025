using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float currentHealth = 100f;

    public float Speed { get { return speed; } }
    public float MaxHealth { get { return maxHealth; } }
    public float CurrentHealth { get { return currentHealth; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector2 direction)
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}
