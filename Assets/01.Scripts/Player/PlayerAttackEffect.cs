using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    [Tooltip("이펙트가 유지되는 시간")]
    public float lifetime = 0.5f;
    [Tooltip("이펙트가 줄 수 있는 데미지")]
    public int damage = 1;

    void Start()
    {
        // 지정된 시간 후에 자동 파괴
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Monster"))
        {
            col.GetComponent<BossBase>()?.TakeDamage(damage);
            col.GetComponent<GolemArm>()?.TakeDamage(damage);
        }
    }
}