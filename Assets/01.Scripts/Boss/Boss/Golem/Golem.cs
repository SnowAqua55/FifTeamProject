using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Golem : BossBase
{
    private GolemBossData golemBossData => bossData as GolemBossData;

    [Header("팔 설정")]
    [SerializeField] private GameObject GolemArmPrefab;
    [SerializeField] private Transform[] armSpawnPoints;
    private List<GolemArm> allArms = new List<GolemArm>();
    private int activeArmsCount = 0;

    [Header("레이저 프리팹")]
    [SerializeField] private GameObject bodyLaserPrefab;
    [SerializeField] private GameObject rainLaserWarningPrefab;
    [SerializeField] private GameObject rainLaserPrefab;

    [Header("고정 위치 레이저")]
    [SerializeField] private Transform[] rainLaserFixedSpawnPoints;

    protected override void Awake()
    {
        base.Awake();

        if (golemBossData == null)
        {
            enabled = false;
            return;
        }

        PrewarmArms(); // 팔 미리 생성 및 풀링
    }

    protected override void Start()
    {
        base.Start();
        InitStateMachine(); // 상태머신 초기화
        ActivateArms();     // 팔 4개 활성화
    }

    public override void InitStateMachine()
    {
        ChangeState(new InvincibilityIdleState());
    }

    public override void TakeDamage(float amount)
    {
        if (isInvincible) return;

        currentHP -= amount;

        if (currentHP <= 0)
        {
            ChangeState(new DeadState());
        }
        else
        {
            ChangeState(new DamagedState());
        }
    }

    private void PrewarmArms()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject armObj = Instantiate(GolemArmPrefab, Vector3.zero, Quaternion.identity);
            armObj.transform.parent = this.transform;
            GolemArm arm = armObj.GetComponent<GolemArm>();

            if (arm != null)
            {
                allArms.Add(arm);
                armObj.SetActive(false);
            }
            else
            {
                Debug.LogError("GolemArm 스크립트가 누락됨");
            }
        }
    }

    private void ActivateArms()
    {
        activeArmsCount = 0;
        SetInvincible(true); // 무적 상태 ON

        if (armSpawnPoints.Length < 4)
        {
            Debug.LogError("팔 스폰 포인트가 부족합니다.");
            return;
        }

        for (int i = 0; i < allArms.Count; i++)
        {
            GolemArm arm = allArms[i];
            arm.Initialize(this, golemBossData.golemArmData, armSpawnPoints[i].position);
            activeArmsCount++;
        }
    }

    public void OnArmDestroyed(GolemArm destroyedArm)
    {
        activeArmsCount--;
        if (activeArmsCount <= 0)
        {
            SetInvincible(false); // 무적 해제
            ChangeState(new AttackState());
        }
    }

    public override void AttackPlayer()
    {
        if (isInvincible || activeArmsCount > 0) return;

        StartCoroutine(PerformBodyLaserAttackSequence());
    }

    /// <summary>
    /// 본체 공격 종류를 랜덤으로 선택
    /// </summary>
    private IEnumerator PerformBodyLaserAttackSequence()
    {
        float rand = Random.value;
        if (rand < 0.5f)
        {
            yield return StartCoroutine(PerformBodyLaserAttack()); // 단일 조준 레이저
        }
        else
        {
            yield return StartCoroutine(StartRainLaserPattern());  // 고정 위치 다발 레이저
        }
    }

    /// <summary>
    /// 레이저 차지/발사 루틴
    /// </summary>
    private IEnumerator PerformBodyLaserAttack()
    {
        Debug.Log("레이저 차지 시작!");

        if (Animator != null && AnimationData != null)
        {
            Animator.SetTrigger(AnimationData.BodyLaserChargeHash); // 차지 애니메이션 시작
        }

        float elapsed = 0f;
        float chargeTime = golemBossData.bodyLaserChargeTime;
        Vector2 laserDirection = Vector2.right;

        // 차지 시간 동안 플레이어 위치 추적하며 방향 결정
        while (elapsed < chargeTime)
        {
            if (player != null)
            {
                laserDirection = player.position.x > transform.position.x ? Vector2.right : Vector2.left;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 차지 마지막 모습 유지 (발사 애니메이션 없이)
        Debug.Log("레이저 발사!");

        GameObject laser = Instantiate(bodyLaserPrefab, transform.position, Quaternion.identity);
        laser.transform.right = laserDirection;

        BodyLaserDamage laserDamage = laser.GetComponent<BodyLaserDamage>();
        if (laserDamage != null)
        {
            laserDamage.SetDamage(golemBossData.golemArmData.attackDamage);
        }

        Destroy(laser, golemBossData.bodyLaserDuration);

        // 레이저가 발사된 상태(차지 마지막 프레임 유지)에서 레이저 지속시간만큼 대기
        yield return new WaitForSeconds(golemBossData.bodyLaserDuration);

        // 레이저 발사 종료 후 마무리 애니메이션 실행
        if (Animator != null && AnimationData != null)
        {
            Animator.SetTrigger(AnimationData.BodyLaserEndHash);  // LaserEnd 트리거로 마무리 애니메이션
        }

        yield return new WaitForSeconds(golemBossData.bodyLaserDuration);

        // 다음 상태로 전환
        ChangeState(new AttackState());
    }

    /// <summary>
    /// 레인 레이저 패턴: 화면 상단 고정 위치에서 다발 레이저
    /// </summary>
    private IEnumerator StartRainLaserPattern()
    {
        float warningTime = golemBossData.rainLaserWarningDuration;
        float fallY = Camera.main.transform.position.y + Camera.main.orthographicSize + 1f;

        // 화면 너비 계산
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
        float leftEdge = Camera.main.transform.position.x - cameraWidth / 2f;

        float interval = golemBossData.rainLaserInterval;
        int laserCount = Mathf.FloorToInt(cameraWidth / interval);

        List<Vector3> round1 = new List<Vector3>();
        List<Vector3> round2 = new List<Vector3>();

        // 첫 번째
        for (int i = 0; i < laserCount; i++)
        {
            float x = leftEdge + (i * interval) + 2.5f;
            round1.Add(new Vector3(x, fallY, 0));
        }

        yield return new WaitForSeconds(golemBossData.rainLaserFallDuration);

        // 두 번째
        for (int i = 0; i < laserCount; i++)
        {
            float x = leftEdge + (i * interval) + 5f;
            round2.Add(new Vector3(x, fallY, 0));
        }

        // 실행
        yield return SpawnRainWave(round1, warningTime);
        yield return new WaitForSeconds(0.2f);
        yield return SpawnRainWave(round2, warningTime);

        yield return new WaitForSeconds(golemBossData.rainLaserFallDuration);

        ChangeState(new AttackState());
    }

    private IEnumerator SpawnRainWave(List<Vector3> spawnPositions, float warningDuration)
    {
        List<GameObject> warnings = new List<GameObject>();

        // 경고 이펙트 생성
        foreach (Vector3 pos in spawnPositions)
        {
            Vector3 warningPos = new Vector3(pos.x, pos.y, -1f); // Z를 살짝 낮춰서 경고를 맨 앞에 보이게
            GameObject warning = Instantiate(rainLaserWarningPrefab, warningPos, Quaternion.identity);
            warnings.Add(warning);
        }

        // 경고 시간만큼 대기
        yield return new WaitForSeconds(warningDuration);

        // 경고 삭제하고 레이저 생성
        foreach (GameObject warning in warnings)
        {
            Vector3 laserPos = new Vector3(warning.transform.position.x, warning.transform.position.y, 0f);
            GameObject laser = Instantiate(rainLaserPrefab, laserPos, Quaternion.identity);

            // 레이저 발사 방향 및 수명 설정 (Projectile 컴포넌트 존재 시)
            Projectile proj = laser.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.SetDamage(golemBossData.attackDamage);
                proj.SetDirection(Vector2.down); // 레이저 아래로 낙하

                // 수명은 카메라 높이 기반 자동 계산
                float fallDistance = Camera.main.orthographicSize * 2f + 1f;
                proj.SetLifeTime(fallDistance / proj.Speed);
            }

            Destroy(warning); // 경고 이펙트 제거
        }
    }

    /// <summary>
    /// 팔이 파괴될 때까지 무적 상태 유지하는 Idle 상태
    /// </summary>
    public class InvincibilityIdleState : IBossState
    {
        private BossBase boss;

        public void Enter(BossBase boss)
        {
            this.boss = boss;
            boss.SetInvincible(true);
            boss.rb.velocity = Vector2.zero;
            boss.Animator.SetBool(boss.AnimationData.InvincibilityIdleHash, true);
            Debug.Log("무적 대기 상태 진입");
        }

        public void Update()
        {
            Golem golem = boss as Golem;
            if (golem != null && golem.activeArmsCount <= 0)
            {
                boss.ChangeState(new AttackState());
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            boss.Animator.SetBool(boss.AnimationData.InvincibilityIdleHash, false);
            Debug.Log("무적 대기 상태 종료");
        }
    }
}
