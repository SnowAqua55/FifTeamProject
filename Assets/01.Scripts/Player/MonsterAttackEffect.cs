using UnityEngine;

public class MonsterAttackEffect : MonoBehaviour
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

    // 플레이어 데미지
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var ph = col.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }
    }
}