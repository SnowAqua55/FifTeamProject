using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("좌우 이동 속도")]
    public float moveSpeed = 5f;

    [Header("점프 설정")]
    [Tooltip("점프력")]
    public float jumpForce = 10f;
    [Tooltip("지면으로 인식할 레이어")]
    public LayerMask groundLayer;
    [Tooltip("지면 판정을 할 위치 (발밑)")]
    public Transform groundCheck;
    [Tooltip("지면 판정용 반지름")]
    public float groundCheckRadius = 0.1f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInputX;
    private bool isGrounded;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 지면 판정
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 좌우 입력 받기
        moveInputX = Input.GetAxisRaw("Horizontal");

        // 점프 입력 처리 (Up Arrow 키)
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 스프라이트 좌우 반전
        if (moveInputX > 0.1f) spriteRenderer.flipX = false;
        else if (moveInputX < -0.1f) spriteRenderer.flipX = true;
        
        bool isRunning = Mathf.Abs(moveInputX) > 0.1f;
        animator.SetBool("IsRun", isRunning);
    }

    void FixedUpdate()
    {
        // 물리 기반 좌우 이동
        rb.velocity = new Vector2(moveInputX * moveSpeed, rb.velocity.y);
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
}