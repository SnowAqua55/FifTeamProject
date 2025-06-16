using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 5f;

    private float currentDamage; // 실제 적용될 데미지
    public float Speed => speed;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        // 옵션: 방향에 따라 스프라이트 회전 (2D 게임에서 유용)
        // Atan2는 라디안 값을 반환하므로 Rad2Deg로 변환
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void SetDamage(float dmg) // 데미지 설정 메서드
    {
        currentDamage = dmg;
    }

    void Awake()
    {
        // Rigidbody2D가 없다면 추가 (OnTriggerEnter2D 동작을 위해 필수)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true; // 물리적 힘에 영향받지 않음
        rb.gravityScale = 0;   // 중력 영향 없음

        // Collider2D가 없다면 추가 (OnTriggerEnter2D 동작을 위해 필수)
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning($"Projectile '{gameObject.name}'에 Collider2D가 없습니다. CircleCollider2D를 추가합니다.");
            col = gameObject.AddComponent<CircleCollider2D>(); // 기본적으로 CircleCollider2D 추가
        }
        col.isTrigger = true; // 트리거로 설정
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // 일정 시간 후 자동 파괴

        // SetDamage가 호출되지 않았을 경우를 대비한 기본값 설정 (선택 사항)
        if (currentDamage == 0)
        {
            currentDamage = 10f; // TODO: 기본 투사체 데미지 설정 (BossArm에서 설정하지 않을 경우)
        }
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
            // TODO: 실제 플레이어 체력 스크립트 (예: PlayerHealth)에 맞춰 호출 방식 변경
            // other.GetComponent<PlayerHealth>()?.TakeDamage(currentDamage);
            Debug.Log($"플레이어에게 {currentDamage} 데미지! (Projectile 충돌)");
            Destroy(gameObject); // 충돌 시 투사체 파괴
        }
        // TODO: 다른 오브젝트(예: 벽, 쉴드)와 충돌 시 파괴 로직 추가 (옵션)
        // if (other.CompareTag("Wall"))
        // {
        //     Destroy(gameObject);
        // }
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