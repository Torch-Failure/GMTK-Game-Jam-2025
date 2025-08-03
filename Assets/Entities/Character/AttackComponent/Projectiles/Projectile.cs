using UnityEngine;

public class Projectile : MonoBehaviour
{

    public int lifeTimeTicks;
    public float speed;
    public float damage;
    public int currentLifeTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
    }

    // Update is called once per frame
    public void ThreadPlayingFixedUpdate()
    {
        currentLifeTime++;
        Debug.Log($"Current lifetime is {currentLifeTime}. Max lifetime is {lifeTimeTicks}");
        if (lifeTimeTicks <= currentLifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Character character =  collision.gameObject.GetComponent<Character>();

        if (character != null)
        {
            // If we are hitting the other 'side' or if we can hit friendlies
            if (this.tag != character.tag || GameManager.enableFriendlyFire)
            {
                Debug.Log("Hit something");
                character.TakeDamage(damage);
            }
        }

        if (collision.gameObject.GetComponent<Projectile>() == null)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        ProjectileManager.instance.RemoveProjectile(this);
    }
}
