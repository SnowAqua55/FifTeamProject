using UnityEngine;

public class AttackEffect : MonoBehaviour
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
        if (col.CompareTag("Monster")) // 맞춰서 수정해야하는 부분
        {
            // col.GetComponent<스크립트명>()?.TakeDamage(damage); > 구현에 맞춰서 수정
        }
    }
    
    // 플레이어 데미지 주기용 테스트 코드
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}