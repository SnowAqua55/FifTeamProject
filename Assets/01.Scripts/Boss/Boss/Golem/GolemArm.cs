using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GolemArm : MonoBehaviour
{
    private Golem parentBoss;              // 팔이 속한 보스 오브젝트 참조
    private float currentHP;               // 팔 현재 체력
    private float maxHP;                   // 팔 최대 체력
    private int attackDamage;              // 공격력
    private float attackCooldown;          // 공격 쿨타임
    private GameObject armProjectilePrefab;// 발사체 프리팹

    [Header("팔 움직임")]
    private float rotationSpeed;           // 팔의 회전 속도
    private float orbitRadius;             // 팔 궤도 반지름
    private float currentOrbitAngle = 0f;  // 궤도 각도

    [Header("HP바 설정")]
    [SerializeField] private GameObject hpBarPrefab;   // hpBar 캔버스 프리팹
    [SerializeField] private float hpBarOffsetY = 1.0f;// hpBar 위치 조절
    [SerializeField] private float hpBarVisibleDuration = 1f; // hpBar 지속 시간

    private GameObject hpBarInstance;      // Instantiate 된 HP바 오브젝트
    private Canvas hpCanvas;             // HP바 Canvas
    private Image hpFillImage;          // HP바 Filled Image
    private Coroutine hideHpBarCoroutine;  // HP바 숨기기 코루틴    
    private Coroutine armAttackRoutine;    // 공격 루틴 코루틴 참조

    /// <summary>
    /// 팔 초기화
    /// </summary>
    public void Initialize(Golem boss, GolemBossData.ArmData armConfig, Vector3 initialWorldPosition)
    {
        parentBoss = boss;

        // 데이터 세팅
        maxHP = armConfig.maxHP;
        currentHP = maxHP;
        attackDamage = armConfig.attackDamage;
        attackCooldown = armConfig.attackCooldown;
        armProjectilePrefab = armConfig.projectilePrefab;
        rotationSpeed = armConfig.armOrbitSpeed;
        orbitRadius = armConfig.armOrbitRadius;

        // 초기 위치, 각도 초기화
        transform.position = initialWorldPosition;
        Vector2 offset = (Vector2)transform.position - (Vector2)parentBoss.transform.position;
        currentOrbitAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        // HP바 생성 (씬 루트에 독립 생성)
        if (hpBarPrefab != null)
        {
            hpBarInstance = Instantiate(hpBarPrefab);
            hpCanvas = hpBarInstance.GetComponent<Canvas>();
            hpFillImage = hpBarInstance.GetComponentInChildren<Image>();

            if (hpCanvas != null)
            {
                hpCanvas.enabled = false;
                // World‑Space Canvas일 경우 메인 카메라 할당
                if (hpCanvas.renderMode == RenderMode.WorldSpace)
                    hpCanvas.worldCamera = Camera.main;
            }
        }

        // 팔 활성화 및 공격 루틴 시작
        gameObject.SetActive(true);
        if (armAttackRoutine != null) StopCoroutine(armAttackRoutine);
        armAttackRoutine = StartCoroutine(ArmAttackRoutine());
    }

    void Update()
    {
        // 보스 없거나 죽으면 비활성화
        if (parentBoss == null || parentBoss.IsDead)
        {
            CleanupHPBar();
            gameObject.SetActive(false);
            return;
        }

        // 궤도 회전
        currentOrbitAngle += rotationSpeed * Time.deltaTime;
        UpdateOrbitPosition();
    }

    void LateUpdate()
    {
        // 씬 루트에 생성된 HP바 위치만 팔 위치 기준으로 갱신
        if (hpBarInstance != null)
        {
            Vector3 worldPos = transform.position + Vector3.up * hpBarOffsetY;
            hpBarInstance.transform.position = worldPos;
        }
    }

    private void UpdateOrbitPosition()
    {
        float rad = currentOrbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * orbitRadius;
        transform.position = parentBoss.transform.position + offset;
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (parentBoss.player == null) return;
        Vector2 dir = (parentBoss.player.position - transform.position).normalized;
        transform.right = dir;
    }

    /// <summary>
    /// 데미지 처리
    /// </summary>
    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        UpdateHPBar();

        if (currentHP <= 0f)
        {
            CleanupHPBar();
            Die();
        }
    }

    private IEnumerator ArmAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            if (parentBoss == null || parentBoss.player == null || !gameObject.activeSelf)
                yield break;
            ShootPlayerTargeted();
        }
    }

    private void ShootPlayerTargeted()
    {
        if (armProjectilePrefab == null || parentBoss.player == null) return;
        GameObject projObj = Instantiate(armProjectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            Vector2 dir = (parentBoss.player.position - transform.position).normalized;
            proj.SetDirection(dir);
            proj.SetDamage(attackDamage);
        }
    }

    private void UpdateHPBar()
    {
        if (hpFillImage != null)
            hpFillImage.fillAmount = Mathf.Clamp01(currentHP / maxHP);

        if (hpCanvas != null)
        {
            hpCanvas.enabled = true;
            if (hideHpBarCoroutine != null) StopCoroutine(hideHpBarCoroutine);
            hideHpBarCoroutine = StartCoroutine(HideHPBarAfterDelay());
        }
    }

    private IEnumerator HideHPBarAfterDelay()
    {
        yield return new WaitForSeconds(hpBarVisibleDuration);
        if (hpCanvas != null)
            hpCanvas.enabled = false;
    }

    private void Die()
    {
        parentBoss.OnArmDestroyed(this);
        gameObject.SetActive(false);
    }

    private void CleanupHPBar()
    {
        if (hpBarInstance != null)
            Destroy(hpBarInstance);
    }
}
