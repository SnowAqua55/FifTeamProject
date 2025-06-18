using UnityEngine;

public class ReaperProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public int damage = 1;

    private Vector2 dir;

    public void Initialize(Vector2 direction)
    {
        dir = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var ph = col.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}