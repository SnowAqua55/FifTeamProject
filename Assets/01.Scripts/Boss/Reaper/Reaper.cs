using UnityEngine;

public class Reaper : BossBase
{
    [Header("잔상 설정")]
    [Tooltip("잔상으로 사용할 프리팹")]
    public GameObject afterImagePrefab;
    [Tooltip("잔상 생성 간격")]
    public float afterImageInterval = 0.1f;
    
    [Header("공격 이펙트")]
    [Tooltip("공격할 때 스폰할 이펙트 프리팹")]
    public GameObject attackEffectPrefab;
    [Tooltip("이펙트가 스폰될 플레이어 기준 X 오프셋")]
    public float attackEffectOffsetX = 1.0f;
    
    SpriteRenderer sr;
    float afterImageTimer;

    protected override void Awake()
    {
        base.Awake();
        // 보스 스프라이트 렌더러 참조
        sr = GetComponent<SpriteRenderer>();
        // 타이머 초기화 → 즉시 한 번 찍히도록
        afterImageTimer = afterImageInterval;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        bool isMoving = Animator.GetBool(AnimationData.MoveHash);
        if (!isMoving) return;
        
        afterImageTimer += Time.deltaTime;
        if (afterImageTimer >= afterImageInterval)
        {
            SpawnAfterImage();
            afterImageTimer = 0f;
        }
    }

    // 필수 구현 메서드들
    public override void InitStateMachine()
    {
        ChangeState(new IdleState());
    }

    public override void AttackPlayer()
    {
        // 공격 애니메이션 재생
        PlayAttackAnimation();
    }
    
    public void SpawnAttackEffect()
    {
        if (attackEffectPrefab == null) return;
        
        // 보스가 향하고 있는 방향 구하기 (localScale.x 의 부호)
        float dir = Mathf.Sign(transform.localScale.x);
        
        // 현재 위치에서 X축으로 offset만큼 옆으로
        Vector3 spawnPos = transform.position + Vector3.right * (dir * attackEffectOffsetX);
        
        GameObject go = Instantiate(attackEffectPrefab, spawnPos, Quaternion.identity);
        
        // 원본 스케일을 유지하면서, X축 방향만 boss와 반대로 뒤집기
        Vector3 effectScale = go.transform.localScale;
        
        // 효과 프리팹이 좌측, 보스가 우측 기본이므로 dir을 -로 곱함
        effectScale.x = Mathf.Abs(effectScale.x) * -dir;
        go.transform.localScale = effectScale;
    }

    private void SpawnAfterImage()
    {
        if (afterImagePrefab == null) return;

        var go = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        // 스케일 복사 (좌우 Flip 포함)
        go.transform.localScale = transform.localScale;

        // Sprite 복사
        var goSr = go.GetComponent<SpriteRenderer>();
        goSr.sprite         = sr.sprite;
        goSr.flipX          = sr.flipX;
        goSr.sortingLayerID = sr.sortingLayerID;
        goSr.sortingOrder   = sr.sortingOrder - 1;
        // AfterImageFader 가 스스로 페이드 후 파괴
    }
}