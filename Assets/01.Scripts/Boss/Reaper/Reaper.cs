using System.Collections;
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

    [Header("패턴 1 - 거리가 벌어지면 투사체 발사")]
    [Tooltip("투사체 프리팹")]
    public GameObject ReaperProjectilePrefab;
    [Tooltip("투사체 총 개수")]
    public int projectileCount = 3;
    [Tooltip("연속 발사 간격")]
    public float projectileInterval = 0.2f;

    [Header("패턴 2 - 순간이동 내려찍기")]
    [Tooltip("패턴 발동 주기")]
    public float teleportPatternInterval = 15f;
    [Tooltip("재등장 후 공중에서 떠 있을 시간")]
    public float floatDuration = 2.25f;
    [Tooltip("낙하 임펄스 세기")]
    public float diveImpulse = 20f;
    [Tooltip("패턴 쿨다운")]
    public float shardCooldown = 1.5f;
    [Tooltip("맵 중앙 상단 위치")]
    public Transform mapCenterTop;
    [Tooltip("레이어 마스크: 바닥 판정용")]
    public LayerMask groundLayer;
    [Tooltip("바닥까지 최대 레이캐스트 거리")]
    public float groundRayDistance = 10f;
    [Tooltip("내려찍기 공격으로 생성할 Prefab")]
    public GameObject diveAttackPrefab;
    [Tooltip("착지 후 X축 방향으로 퍼져나갈 거리")]
    public float diveAttackSpacing = 1f;
    [Tooltip("총 생성 횟수")]
    public int diveAttackCount = 3;
    [Tooltip("순차 생성 간격")]
    public float diveAttackInterval= 0.2f;
    
    [Header("패턴 3 - 암살")]
    [Tooltip("암살 순간이동 후 플레이어와의 X 간격")]
    public float backstabOffset = 1.5f;
    [Tooltip("암살 애니메이션 길이")]
    public float backstabDuration = 1.0f;
    [Tooltip("뒤통수 공격 시 생성할 이펙트 프리팹들")]
    public GameObject[] backstabEffectPrefabs;
    [Tooltip("이펙트 간 생성 간격")]
    public float backstabEffectInterval = 0.1f;
    [Tooltip("회전 랜덤 범위 (deg)")]
    public float backstabEffectMaxRotation = 360f;
    
    [Header("패턴 4 - 페이즈 전환 개막 패턴")]
    [Tooltip("phase 시작 시 나타날 파티클")]
    public ParticleSystem swirlParticle;
    [Tooltip("개막 파동 낫 공격용 위치")]
    public Transform topCenterPos;
    [Tooltip("후광 이펙트")]
    public GameObject haloObject;
    [Tooltip("속도 버프량")]
    public float phaseSpeedMultiplier = 1.25f;
    [Tooltip("한 번만 실행 플래그")]
    public bool phaseTriggered = false;
    [Tooltip("HeavySlash 이펙트")]
    public GameObject heavySlashEffectPrefab;
    
    // 연속 투사체 패턴 카운터
    [HideInInspector] public int projectilePatternCount = 0;
    
    [HideInInspector] public bool isDiving;
    [HideInInspector] public Collider2D coll;
    private float originalGravity;
    public float OriginalGravity => originalGravity;
    
    SpriteRenderer sr;
    float afterImageTimer;
    float teleportTimer;

    //애니메이터 파라미터 캐싱
    private static readonly int ShotHash = Animator.StringToHash("DoShot");
    private static readonly int DiveHash = Animator.StringToHash("DoDive");
    private static readonly int BackstabHash = Animator.StringToHash("DoBackstab");
    private static readonly int HeavySlashHash = Animator.StringToHash("DoHeavySlash");

    public void PlayShotAnimation()
    {
        Animator.SetTrigger(ShotHash);
    }

    public void PlayDiveAnimation()
    {
        Animator.SetTrigger(DiveHash);
    }
    
    public void PlayBackstabAnimation()
    {
        Animator.SetTrigger(BackstabHash);
    }
    
    public void PlayHeavySlashAnimation()
    {
        Animator.SetTrigger(HeavySlashHash);
    }

    protected override void Awake()
    {
        base.Awake();
        // 보스 스프라이트 렌더러 참조
        sr = GetComponent<SpriteRenderer>();
        // 타이머 초기화 → 즉시 한 번 찍히도록
        afterImageTimer = afterImageInterval;
        coll = gameObject.GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        GameManager.Instance.Stage.stageWalls.gameObject.SetActive(false);
        mapCenterTop = GameManager.Instance.Stage.reaperTelPosition.transform;
        topCenterPos = GameManager.Instance.Stage.reaperTelPosition.transform;
    }
    
    protected override void Start()
    {
        base.Start(); 
    }

    protected override void Update()
    {
        base.Update();

        teleportTimer += Time.deltaTime;
        if (teleportTimer >= teleportPatternInterval)
        {
            teleportTimer = 0f;
            ChangeState(new ReaperTeleportDiveState());
            return;
        }

        bool isMoving = Animator.GetBool(AnimationData.MoveHash);
        bool isFallingDive = isDiving && rb.velocity.y < -0.1f;

        // 이동 중이거나 패턴 3 낙하 시 잔상 생성
        if (isMoving || isFallingDive)
        {
            afterImageTimer += Time.deltaTime;
            if (afterImageTimer >= afterImageInterval)
            {
                SpawnAfterImage();
                afterImageTimer = 0f;
            }
        }
        else
        {
            // 재진입 시 즉시 찍히도록
            afterImageTimer = afterImageInterval;
        }
        
        if (!phaseTriggered && CurrentHP <= MaxHP * 0.5f)
        {
            phaseTriggered = true;
            ChangeState(new ReaperPhaseTransitionState());
        }
    }

    public override void InitStateMachine()
    {
        ChangeState(new ReaperIdleState());
    }

    public override void AttackPlayer()
    {
        // 공격 애니메이션 재생
        PlayAttackAnimation();
    }

    // 기본 공격 이펙트 생성
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

    // 잔상 생성
    public void SpawnAfterImage()
    {
        if (afterImagePrefab == null) return;

        var go = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        // 스케일 복사 (좌우 Flip 포함)
        go.transform.localScale = transform.localScale;

        // Sprite 복사
        var goSr = go.GetComponent<SpriteRenderer>();
        goSr.sprite = sr.sprite;
        goSr.flipX = sr.flipX;
        goSr.sortingLayerID = sr.sortingLayerID;
        goSr.sortingOrder = sr.sortingOrder - 1;
        // AfterImageFader 가 스스로 페이드 후 파괴
    }

    // Animation Event 에 연결할 함수 =====
    public void OnProjectileShotEvent()
    {
        // 첫 발 즉시
        SpawnSingleProjectile();
        // 나머지 발사 예약
        if (projectileCount > 1)
            StartCoroutine(SpawnRemaining(projectileCount - 1, projectileInterval));
    }

    private void SpawnSingleProjectile()
    {
        var go = Instantiate(ReaperProjectilePrefab, transform.position, Quaternion.identity);
        var proj = go.GetComponent<ReaperProjectile>();
        var dir = (player.position - transform.position).normalized;
        proj.Initialize(dir);
        
        float bossSign = Mathf.Sign(transform.localScale.x);
        Vector3 s = go.transform.localScale;
        s.x = Mathf.Abs(s.x) * bossSign;
        go.transform.localScale = s;
    }

    private IEnumerator SpawnRemaining(int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(interval);
            SpawnSingleProjectile();
        }
    }
    
    // 애니메이션에서 호출할 패턴 2 투명 이벤트
    public void OnDiveVanishEvent()
    {
        isDiving = true;
        // 콜라이더 끄기
        coll.enabled = false;
        // 중력 끄기
        rb.simulated = false;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
    }

    // 애니메이션에서 호출할 패턴 2 재등장 이벤트
    public void OnTeleportReappearEvent()
    {
        transform.position = mapCenterTop.position;
        
        StartCoroutine(DoDiveImpulse());
    }
    
    // 패턴 2 내려찍기 임펄스 강하 코루틴
    private IEnumerator DoDiveImpulse()
    {
        yield return new WaitForSeconds(floatDuration);
        // 강하 임펄스
        rb.simulated    = true;
        rb.gravityScale = originalGravity;      // 원래 중력 스케일로 돌린 뒤
        rb.AddForce(Vector2.down * diveImpulse, ForceMode2D.Impulse);
    }
    
    // 애니메이션에서 호출할 패턴 3 공격 이벤트
    public void OnDiveStrikeEvent()
    {
        // 아래로 레이캐스트 체크
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRayDistance, groundLayer);
        if (!hit.collider) return;

        // 중점 X는 보스 위치, Y는 닿은 지점
        float centerX = transform.position.x;
        float groundY = hit.point.y;

        // diveAttackPrefab 피벗 보정
        float yOffset = 0f;
        var sr = diveAttackPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
            yOffset = sr.bounds.extents.y;

        Vector3 centerPos = new Vector3(centerX, groundY + yOffset, transform.position.z);

        // 순차 생성
        StartCoroutine(SpawnDiveAttacks(centerPos) );

        // 복구
        coll.enabled = true;
        rb.simulated = true;
        rb.gravityScale = originalGravity;
        isDiving = false;
    }
    
    // 패턴 2 파편 생성 코루틴
    private IEnumerator SpawnDiveAttacks(Vector3 centerPos)
    {
        // 맨 처음 중앙에 한 번 스폰
        Instantiate(diveAttackPrefab, centerPos, Quaternion.identity);
        yield return new WaitForSeconds(diveAttackInterval);

        // i = 1, 2, … diveAttackCount 만큼 반복
        for (int i = 1; i <= diveAttackCount; i++)
        {
            // 좌/우 위치 계산
            Vector3 leftPos  = centerPos + Vector3.left * (diveAttackSpacing * i);
            Vector3 rightPos = centerPos + Vector3.right * (diveAttackSpacing * i);

            // 동시에 스폰
            Instantiate(diveAttackPrefab, leftPos,  Quaternion.identity);
            Instantiate(diveAttackPrefab, rightPos, Quaternion.identity);

            // 다음 웨이브까지 대기
            yield return new WaitForSeconds(diveAttackInterval);
        }
    }

    // 패턴 2 코루틴
    public IEnumerator TeleportDiveRoutine()
    {
        PlayDiveAnimation();
        yield return null;
        
        while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Reaper_Dive"))
            yield return null;
        
        // “Reaper_Dive” 클립 전체 길이 및 안전 매치용 여유
        float animLength = Animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animLength + 0.1f);
        
        yield return new WaitForSecondsRealtime(shardCooldown);

        if (isDiving)
        {
            coll.enabled = true;
            isDiving = false;
            rb.gravityScale = originalGravity;
        }

        ChangeState(new ReaperChaseState());
    }
    
    // 패턴 3 투명 이벤트
    public void OnBackstabVanishEvent()
    {
        coll.enabled = false;
    }
    
    // 패턴 3 재등장 이벤트
    public void OnBackstabReappearEvent()
    {
        // 플레이어 뒤로 이동
        float dir = Mathf.Sign(player.localScale.x);
        transform.position = player.position + Vector3.left * dir * backstabOffset;
        
        coll.enabled = true;
    }

    // 패턴 3 공격 이펙트 생성 이벤트
    public void OnBackstabStrikeEvent()
    {
        StartCoroutine(DoSpawnBackstabEffects());
    }

    // 패턴 3 이펙트 생성 코루틴
    private IEnumerator DoSpawnBackstabEffects()
    {
        for (int i = 0; i < backstabEffectPrefabs.Length; i++)
        {
            var prefab = backstabEffectPrefabs[i];
            Vector3 spawnPos = transform.position;
            float zRot = Random.Range(0f, backstabEffectMaxRotation);
            Instantiate(prefab, spawnPos, Quaternion.Euler(0,0,zRot));
            yield return new WaitForSeconds(backstabEffectInterval);
        }
    }
    
    // 패턴 4 공격 이펙트 생성 이벤트
    public void OnHeavySlashEvent()
    {
        if (heavySlashEffectPrefab == null) return;
        // 보스 위치나 필요한 오프셋
        Vector3 spawnPos = transform.position + Vector3.right * (transform.localScale.x > 0 ? 0.5f : -0.5f);
        Instantiate(heavySlashEffectPrefab, spawnPos, Quaternion.identity);
    }
}