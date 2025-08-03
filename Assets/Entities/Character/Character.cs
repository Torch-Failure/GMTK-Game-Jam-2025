using UnityEngine;
using System;

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
    public event Action<Character> OnDeath;

    public enum CharacterState {Alive, Dead};
    public CharacterState state {get; private set;} = CharacterState.Alive;

    public void Move(Vector2 direction)
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public Color GetCurrentColor()
    {
        switch(state)
        {
            case CharacterState.Alive:
                return Color.white;
            case CharacterState.Dead:
                return Color.chocolate;
        }

        throw new InvalidOperationException($"Switch statement did not handle character state {state}");
    }

    public void Die()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        var renderer = GetComponent<SpriteRenderer>();
        renderer.color = Color.chocolate;

        state = CharacterState.Dead;

        OnDeath?.Invoke(this);

    }

    public void UnDie()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;
        var renderer = GetComponent<SpriteRenderer>();
        renderer.color = Color.white;


        currentHealth = maxHealth;
        state = CharacterState.Alive;
    }

    public virtual void Attack()
    {
        if (attackComponent == null)
        {
            Debug.Log("Attack compoment is null");
        }
        if (transform.rotation == null)
        {
            Debug.Log("Transform rotation is null");
        }
        attackComponent.Attack(transform.rotation);
    }
}
