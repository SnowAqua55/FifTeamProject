using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Golem : BossBase
{
    private GolemBossData golemBossData => bossData as GolemBossData; 

    [Header("Arms Setup")]
    [SerializeField] private GameObject GolemArmPrefab; 
    [SerializeField] private Transform[] armSpawnPoints; 
    private List<GolemArm> allArms = new List<GolemArm>(); 
    private int activeArmsCount = 0; 

    [Header("Laser Attack Prefabs")]
    [SerializeField] private GameObject bodyLaserWarningPrefab; 
    [SerializeField] private GameObject bodyLaserPrefab;        
    [SerializeField] private GameObject rainLaserWarningPrefab; 
    [SerializeField] private GameObject rainLaserPrefab;        

    [Header("Arm Pattern Positions")]
    [SerializeField] private Transform[] armLinePositions;   


    protected override void Awake()
    {
        base.Awake(); 

        if (golemBossData == null)
        {
            Debug.LogError("GolemBoss 스크립트에는 GolemBossData 타입의 BossData가 할당되어야 합니다! 유니티 인스펙터에서 확인하세요.", this);
            enabled = false;
            return;
        }

        PrewarmArms(); 
    }

    protected override void Start()
    {
        base.Start(); 
        InitStateMachine();
        ActivateArms(); 
    }

    public override void InitStateMachine()
    {
        ChangeState(new InvincibilityIdleState()); 
    }

    public override void TakeDamage(float amount)
    {
        if (isInvincible) 
        {
            Debug.Log("골렘 본체는 현재 무적 상태입니다!");
            return; 
        }

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
                Debug.LogError($"생성된 {armObj.name}에 GolemArm 스크립트가 없습니다!");
            }
        }
        Debug.Log($"총 {allArms.Count}개의 팔을 미리 생성하여 풀링했습니다.");
    }

    private void ActivateArms()
    {
        activeArmsCount = 0; 
        SetInvincible(true); 

        if (armSpawnPoints == null || armSpawnPoints.Length < 4)
        {
            Debug.LogError("팔 스폰 위치(armSpawnPoints)가 충분하지 않습니다! 최소 4개 필요합니다. 유니티 인스펙터에서 설정해주세요.");
            return; 
        }
        
        for (int i = 0; i < allArms.Count; i++)
        {
            GolemArm arm = allArms[i];
            if (arm != null)
            {
                arm.Initialize(this, golemBossData.golemArmData, armSpawnPoints[i].position); 
                activeArmsCount++;
            }
        }
        Debug.Log("팔 4개 활성화 완료. 골렘 본체 무적 활성화.");
    }

    public void OnArmDestroyed(GolemArm destroyedArm)
    {
        activeArmsCount--;
        Debug.Log($"팔 비활성화됨. 현재 활성화된 팔: {activeArmsCount}/4");

        if (activeArmsCount <= 0) 
        {
            SetInvincible(false); 
            Debug.Log("모든 팔 비활성화. 골렘 본체 무적 해제!");
            
            ChangeState(new AttackState()); 
        }
    }

    public override void AttackPlayer()
    {
        if (isInvincible || activeArmsCount > 0)
        {
            Debug.Log("골렘 본체는 현재 무적 상태이거나 팔이 활성화되어 있어 본체 공격은 대기합니다.");
            return;
        }
        
        StartCoroutine(PerformBodyLaserAttackSequence()); 
    }

    private IEnumerator PerformBodyLaserAttackSequence()
    {
        float rand = Random.value;
        if (rand < 0.5f) 
        {
            yield return StartCoroutine(PerformBodyLaserAttack());
        }
        else 
        {
            yield return StartCoroutine(StartRainLaserPattern());
        }
    }
    
    private IEnumerator PerformBodyLaserAttack()
    {
        Debug.Log("본체 레이저 준비!");
        if (Animator != null && AnimationData != null)
        {
            Animator.SetTrigger(AnimationData.BodyLaserChargeHash); 
        }

        Vector3 playerCurrentPos = player.position; 
        GameObject warning = Instantiate(bodyLaserWarningPrefab, playerCurrentPos, Quaternion.identity);
        Destroy(warning, golemBossData.bodyLaserChargeTime); 

        yield return new WaitForSeconds(golemBossData.bodyLaserChargeTime); 

        Debug.Log("본체 레이저 발사!");
        if (Animator != null && AnimationData != null)
        {
            Animator.SetTrigger(AnimationData.BodyLaserFireHash); 
        }

        GameObject laser = Instantiate(bodyLaserPrefab, transform.position, Quaternion.identity);
        Vector2 laserDirection = (playerCurrentPos - transform.position).normalized;
        laser.transform.right = laserDirection; 
        
        Destroy(laser, golemBossData.bodyLaserDuration); 
    }

    private IEnumerator StartRainLaserPattern()
    {
        Debug.Log("맵 상단 레이저 패턴 시작!");
        
        for (int i = 0; i < allArms.Count; i++)
        {
            if (allArms[i].gameObject.activeSelf && i < armLinePositions.Length)
            {
                allArms[i].MoveArmToPatternPosition(armLinePositions[i].position);
            }
        }
        yield return new WaitForSeconds(1.0f); 

        float cameraHeight = Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float topY = Camera.main.transform.position.y + cameraHeight; 
        float minX = Camera.main.transform.position.x - cameraWidth;
        float maxX = Camera.main.transform.position.x + cameraWidth;

        for (int i = 0; i < golemBossData.numberOfRainLasers; i++) 
        {
            Vector3 spawnPos = new Vector3(Random.Range(minX, maxX), topY, 0); 
            
            GameObject warning = Instantiate(rainLaserWarningPrefab, spawnPos, Quaternion.identity);
            Destroy(warning, golemBossData.rainLaserWarningDuration); 

            yield return new WaitForSeconds(golemBossData.rainLaserInterval); 
        }
    }

    public class InvincibilityIdleState : IBossState
    {
        private BossBase boss;

        public void Enter(BossBase boss)
        {
            this.boss = boss;
            boss.SetInvincible(true); 
            boss.rb.velocity = Vector2.zero; 
            boss.Animator.SetBool(boss.AnimationData.InvincibilityIdleHash, true); 
            Debug.Log("무적 대기 상태 진입. 팔이 모두 파괴될 때까지 무적.");
        }

        public void Update()
        {
            Golem golem = boss as Golem;
            if (golem != null && golem.activeArmsCount <= 0)
            {
                boss.ChangeState(new AttackState()); 
            }
        }

        public void FixedUpdate() {}

        public void Exit()
        {
            boss.Animator.SetBool(boss.AnimationData.InvincibilityIdleHash, false); 
            Debug.Log("무적 대기 상태 종료.");
        }
    }
}