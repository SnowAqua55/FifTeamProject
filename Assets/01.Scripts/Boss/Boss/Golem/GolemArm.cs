using UnityEngine;
using System.Collections;

public class GolemArm : MonoBehaviour
{
    private Golem parentBoss;              // 이 팔이 속한 보스 오브젝트 참조
    private float currentHP;               // 팔 현재 체력
    private float attackDamage;            // 공격력
    private float attackCooldown;          // 공격 쿨타임
    private GameObject armProjectilePrefab;// 발사체 프리팹

    [Header("팔 움직임")]
    private float rotationSpeed;           // 팔의 회전 속도 (원형 궤도 속도)
    private float orbitRadius;             // 팔이 보스를 중심으로 도는 반지름
    private float currentOrbitAngle = 0f; // 현재 궤도 각도 (도 단위)

    private Coroutine armAttackRoutineInstance;      // 공격 루틴 코루틴 참조

    /// <summary>
    /// 팔 초기화 (보스, 팔 설정값, 초기 위치 받아 세팅)
    /// </summary>
    /// <param name="boss">부모 오브젝트</param>
    /// <param name="armConfig">초기값</param>
    /// <param name="initialWorldPosition">팔 월드 포지션</param>
    public void Initialize(Golem boss, GolemBossData.ArmData armConfig, Vector3 initialWorldPosition)
    {
        parentBoss = boss;

        // 체력, 공격력, 쿨타임, 발사체, 회전속도 등 세팅
        currentHP = armConfig.maxHP;
        attackDamage = armConfig.attackDamage;
        attackCooldown = armConfig.attackCooldown;
        armProjectilePrefab = armConfig.projectilePrefab;
        rotationSpeed = armConfig.armOrbitSpeed;
        orbitRadius = armConfig.armOrbitRadius;

        // Rigidbody2D가 없으면 추가, 키네마틱 키고, 그레비티 끄기
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0;

        // 초기 위치 설정
        transform.position = initialWorldPosition;

        // 보스 위치 대비 팔 위치의 각도 계산 (궤도 초기 각도)
        Vector2 initialOffset = transform.position - parentBoss.transform.position;
        currentOrbitAngle = Mathf.Atan2(initialOffset.y, initialOffset.x) * Mathf.Rad2Deg;

        gameObject.SetActive(true);

        // 공격 코루틴 시작 (중복 방지 위해 기존 있으면 멈춤)
        if (armAttackRoutineInstance != null) StopCoroutine(armAttackRoutineInstance);
        armAttackRoutineInstance = StartCoroutine(ArmAttackRoutine());
    }

    // 매 프레임 호출 - 회전
    void Update()
    {
        if (parentBoss == null || parentBoss.IsDead)
        {
            // 보스가 없거나 죽었으면 팔 비활성화
            gameObject.SetActive(false);
            return;
        }
        // 패턴 위치 이동 중이 아니면 궤도 회전 각도를 증가시키고 위치 업데이트
        currentOrbitAngle += rotationSpeed * Time.deltaTime;
        UpdateOrbitPosition();
    }

    /// <summary>
    /// 현재 각도에 따라 보스 주변 궤도 위치 계산 및 적용
    /// </summary>
    private void UpdateOrbitPosition()
    {
        if (parentBoss == null) return;

        float angleRad = currentOrbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * orbitRadius;

        // 보스 위치 + 궤도 오프셋이 팔 위치
        transform.position = parentBoss.transform.position + offset;

        LookAtPlayer(); // 플레이어 바라보게 회전
    }

    /// <summary>
    /// 플레이어 방향을 향하도록 팔의 방향 설정
    /// </summary>
    private void LookAtPlayer()
    {
        if (parentBoss.player == null) return;

        Vector2 directionToPlayer = (parentBoss.player.position - transform.position).normalized;
        transform.right = directionToPlayer; // 팔의 오른쪽 축이 플레이어 방향
    }

    /// <summary>
    /// 외부에서 호출, 팔이 데미지를 입었을 때
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log($"팔 '{gameObject.name}' 데미지 입음. 현재 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die(); // 체력 0 이하면 죽음 처리
        }
    }

    /// <summary>
    /// 공격 코루틴: 쿨타임마다 공격 시도
    /// </summary>
    /// <returns></returns>
    private IEnumerator ArmAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (parentBoss == null || parentBoss.player == null || !gameObject.activeSelf)
            {
                yield break; // 조건 안 맞으면 루틴 종료
            }
            ShootPlayerTargeted(); // 이동 중이 아니면 공격 실행
        }
    }

    /// <summary>
    /// 플레이어를 정확히 향하는 발사체 생성 및 초기화
    /// </summary>
    private void ShootPlayerTargeted()
    {
        if (parentBoss.player == null || armProjectilePrefab == null) return;

        GameObject projectileObj = Instantiate(armProjectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            Vector2 direction = (parentBoss.player.position - transform.position).normalized;
            projectile.SetDirection(direction);
            projectile.SetDamage(attackDamage);
        }
    }

    /// <summary>
    /// 팔 체력이 0이 되어 죽었을 때 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("팔 '" + gameObject.name + "' 비활성화됨!");
        if (parentBoss != null)
        {
            parentBoss.OnArmDestroyed(this); // 보스에게 팔 파괴 알림
        }
        gameObject.SetActive(false);          // 팔 비활성화
        if (armAttackRoutineInstance != null) StopCoroutine(armAttackRoutineInstance); // 공격 코루틴 중지
    }
}
