using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float lifetimeSeconds = 2f;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetimeSeconds);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
