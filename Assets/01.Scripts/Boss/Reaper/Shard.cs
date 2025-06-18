using UnityEngine;

public class Shard : MonoBehaviour
{
    private Vector2 direction;
    private float  speed;

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir;
        speed     = spd;
        Destroy(gameObject, 2f);  // Lifetime
    }

    void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
    }
}