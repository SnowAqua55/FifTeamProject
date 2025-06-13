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

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private float moveInputX;
    private bool isGrounded;
    
    private float afterImageTimer;
    
    private bool isDodging;
    private bool canDodge = true;

    private bool isInvincible;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();    
    }

    void Update()
    {
        // 지면 판정
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && !wasGrounded) jumpCount = 0;

        // 좌우 입력 받기
        moveInputX = Input.GetAxisRaw("Horizontal");

        // 점프 입력 처리 (Up Arrow 키)
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpCount < maxJumpCount)
        {
            // Y속도 초기화 후 임펄스
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
            
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
        }
        
        // 회피 입력 (z)
        if (Input.GetKeyDown(KeyCode.Z) && canDodge && !this.isDodging)
        {
            StartCoroutine(DoDodge());
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
        isInvincible = true; // 무적
        canDodge = false;
        animator.SetTrigger("DoDodge");

        // 대시 방향 벡터
        float direction = spriteRenderer.flipX ? -1f : 1f;
        rb.velocity = new Vector2(direction * dodgeSpeed, 0f);

        // 잔상 타이머 초기화
        afterImageTimer = afterImageInterval;

        // 일정 시간 대시 유지
        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
        
        // 무적 끝내기
        yield return new WaitForSeconds(invincibleDuration - dodgeDuration);
        isInvincible = false;


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
        // 1) 잔상 오브젝트 생성
        var go = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        // 2) 플레이어 현재 스프라이트와 동일하게 복사
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite     = spriteRenderer.sprite;
        sr.flipX      = spriteRenderer.flipX;
        sr.sortingLayerID   = spriteRenderer.sortingLayerID;
        sr.sortingOrder     = spriteRenderer.sortingOrder - 1; // 한 겹 뒤로

        // AfterImageFader 스크립트가 스스로 페이드 후 파괴
    }
}