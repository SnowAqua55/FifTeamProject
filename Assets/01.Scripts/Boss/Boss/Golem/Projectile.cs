using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 5f;

    private int currentDamage; // 실제 적용될 데미지
    public float Speed => speed;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetDamage(int dmg) // 데미지 설정 메서드
    {
        currentDamage = dmg;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // 일정 시간 후 자동 파괴
    }

    void Update()
    {
        // 월드 좌표계 기준으로 이동
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어에게 데미지 주기
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(currentDamage);
            Destroy(gameObject); // 충돌 시 투사체 파괴
        }
    }

    public void SetLifeTime(float time)
    {
        CancelInvoke(nameof(DestroySelf));
        lifeTime = time;
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}