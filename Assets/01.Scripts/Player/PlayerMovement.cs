using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("좌우 이동 속도")]
    public float moveSpeed = 5f;

    [Header("점프 설정")]
    [Tooltip("점프력")]
    public float jumpForce = 6f;
    [Tooltip("최대 점프 횟수")]
    public int maxJumpCount = 2;
    private int jumpCount = 0;
    
    [Tooltip("지면으로 인식할 레이어")]
    public LayerMask groundLayer;
    [Tooltip("지면 판정을 할 위치 (발밑)")]
    public Transform groundCheck;
    [Tooltip("지면 판정용 반지름")]
    public float groundCheckRadius = 0.1f;
    
    [Header("회피 설정")]
    [Tooltip("잔상 프리팹")]
    public GameObject afterImagePrefab;
    [Tooltip("회피 시작 후 잔상 생성 간격(초)")]
    public float afterImageInterval = 0.05f;
    [Tooltip("회피 시 대시 속도")]
    public float dodgeSpeed = 8f;
    [Tooltip("회피 지속 시간")]
    public float dodgeDuration = 0.3f;
    [Tooltip("회피 쿨타임")]
    public float dodgeCooldown = 1f;
    [Tooltip("무적 지속 시간")]
    public float invincibleDuration = 0.3f;
    
    [Header("점프 이펙트")]
    [Tooltip("점프 시 생성할 파티클")]
    public ParticleSystem jumpEffectPrefab;

    [Header("걸음 이펙트")]
    [Tooltip("걸을 때 생성할 파티클")]
    public ParticleSystem footstepDustPrefab;
    [Tooltip("착지 할 때 생성할 파티클")]
    public ParticleSystem landingDustPrefab;
    [Tooltip("발자국 간격")]
    public float footstepInterval = 0.4f;
    
    [Header("공격 설정")]
    [Tooltip("공격 시 생성할 이펙트 프리팹")]
    public GameObject attackEffectPrefab;
    [Tooltip("공격 이펙트 스폰 시 앞쪽으로 밀어낼 거리")]
    public float attackOffsetX = 0.5f;
    [Tooltip("공격 간격(초)")]
    public float attackCooldown = 0.42f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerHealth playerHealth;
    private PlayerAudio audioPlayer;
    
    private float moveInputX;
    private bool isGrounded;
    private bool wasGrounded;
    private float footstepTimer;
    
    private float afterImageTimer;
    
    private bool isDodging;
    private bool canDodge = true;
    
    private float nextAttackTime = 0f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();    
        playerHealth = GetComponent<PlayerHealth>();
        audioPlayer = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        // 지면 판정
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded && !wasGrounded)
        {
            // 이단 점프 카운트 초기화
            jumpCount = 0;
            
            // 착지 SFX 재생
            audioPlayer.PlayLand();
            
            // 착지 이펙트 생성
            if (landingDustPrefab != null)
                Instantiate(landingDustPrefab, groundCheck.position, Quaternion.identity).Play();
        }
        
        // 좌우 입력 받기
        moveInputX = Input.GetAxisRaw("Horizontal");

        // 점프 입력 처리 (Up Arrow 키)
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount < maxJumpCount)
        {
            // Y속도 초기화 후 임펄스
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            
            // 점프 이펙트 생성
            if (jumpEffectPrefab != null)
            {
                // groundCheck 위치에 한번 재생
                ParticleSystem effect = Instantiate(
                    jumpEffectPrefab, 
                    groundCheck.position, 
                    Quaternion.identity
                );
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
            }
            
            // 점프 SFX 재생
            audioPlayer.PlayJump();
        }
        
        // 걸을 때 먼지 이펙트 생성
        if (isGrounded && Mathf.Abs(moveInputX) > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                Instantiate(footstepDustPrefab, groundCheck.position, Quaternion.identity).Play();
                footstepTimer = 0f;
            }
        }
        else footstepTimer = 0f; // 재진입 시 바로 생성
        
        // 회피 입력 (z 키)  
        if (Input.GetKeyDown(KeyCode.Z) && canDodge && !this.isDodging)
        {
            StartCoroutine(DoDodge());
            
            // 대쉬 SFX 재생
            audioPlayer.PlayDash();
        }
        
        // 공격 입력 (X 키)
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= nextAttackTime && !this.isDodging)
        {
            nextAttackTime = Time.time + attackCooldown;
            // 공격 애니메이션 재생
            animator.SetTrigger("DoAttack");
            // 공격 이펙트 생성
            SpawnAttackEffect();
            // 공격 SFX 재생
            audioPlayer.PlayAttack();
        }

        // 스프라이트 좌우 반전
        if (moveInputX > 0.1f) spriteRenderer.flipX = false;
        else if (moveInputX < -0.1f) spriteRenderer.flipX = true;

        bool isRunning = Mathf.Abs(moveInputX) > 0.1f;
        animator.SetBool("IsRun", isRunning);
        
        // 회피 애니메이션 중일 때만 잔상 스폰
        bool isDodging = animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Dodge");
        if (isDodging)
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
            afterImageTimer = afterImageInterval; // 재진입 시 바로 생성
        }
    }

    void FixedUpdate()
    {
        if (isDodging) return;  // 회피 중엔 이동 로직 무시
        
        // 물리 기반 좌우 이동
        rb.velocity = new Vector2(moveInputX * moveSpeed, rb.velocity.y);
    }

    private IEnumerator DoDodge()
    {
        // 상태 설정
        isDodging = true;
        canDodge = false;
        animator.SetTrigger("DoDodge");
        
        playerHealth.ActivateInvincibility(invincibleDuration);

        // 대시 방향 벡터
        float direction = spriteRenderer.flipX ? -1f : 1f;
        rb.velocity = new Vector2(direction * dodgeSpeed, 0f);

        // 잔상 타이머 초기화
        afterImageTimer = afterImageInterval;

        // 일정 시간 대시 유지
        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
        
        // 쿨다운 끝날 때까지 대기
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
    
    // 시각화용: Scene 뷰에서 지면 판정 범위 확인
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    private void SpawnAfterImage()
    {
        // 잔상 오브젝트 생성
        var go = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        // 플레이어 현재 스프라이트와 동일하게 복사
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite     = spriteRenderer.sprite;
        sr.flipX      = spriteRenderer.flipX;
        sr.sortingLayerID   = spriteRenderer.sortingLayerID;
        sr.sortingOrder     = spriteRenderer.sortingOrder - 1; // 한 겹 뒤로

        // AfterImageFader 스크립트가 스스로 페이드 후 파괴
    }
    
    private void SpawnAttackEffect()
    {
        if (attackEffectPrefab == null) return;

        // 어느 방향으로 공격할지 계산
        float dir = spriteRenderer.flipX ? -1f : 1f;

        // 생성 위치: 플레이어 위치 + offset
        Vector3 spawnPos = transform.position + Vector3.right * (attackOffsetX * dir);

        // 이펙트 생성
        GameObject go = Instantiate(attackEffectPrefab, spawnPos, Quaternion.identity);

        // 방향(플립)이 필요한 스프라이트라면
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = spriteRenderer.flipX;

        // 이펙트가 플레이어보다 앞에 보이도록 정리
        sr.sortingLayerID = spriteRenderer.sortingLayerID;
        sr.sortingOrder   = spriteRenderer.sortingOrder + 1;
    }
}