using UnityEngine;
public class BodyLaserDamage : MonoBehaviour
{
    private Collider2D col;
    private int damage;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void EnableDamage()
    {
        col.enabled = true;
    }

    public void DisableDamage()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
