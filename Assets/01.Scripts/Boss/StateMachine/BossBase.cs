using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    protected IBossState currentState;

    [Header("보스 데이터")]
    [SerializeField] private BossData bossData; // SO 연결

    public Animator Animator { get; private set; }
    public BossAnimationData AnimationData { get; private set; }

    protected float currentHP;
    public Rigidbody2D rb { get; protected set; }

    public float CurrentHP => currentHP;
    public bool IsDead => currentHP <= 0;

    public float MaxHP => bossData.maxHP;
    public float MoveSpeed => bossData.moveSpeed;
    public float AttackRange => bossData.attackRange;
    public float DetectRange => bossData.detectRange;
    public float AttackCooldown => bossData.attackCooldown;
    public float StaggerTime => bossData.staggerTime;

    public Transform player { get; protected set; }

    protected virtual void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        AnimationData = new BossAnimationData();
        AnimationData.Initialize();

        rb = GetComponent<Rigidbody2D>();
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        else
            Debug.LogWarning("플레이어를 찾을 수 없습니다!");

        // HP 초기화
        currentHP = bossData.maxHP;
    }

    protected virtual void Start()
    {
        InitStateMachine();
    }

    protected virtual void Update()
    {
        currentState?.Update();
    }

    protected virtual void FixedUpdate()
    {
        currentState?.FixedUpdate(); // 물리 이동 처리
    }

    /// <summary>
    /// 상태 전환
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IBossState newState)
    {
        if (currentState?.GetType() == newState.GetType()) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }
    
    /// <summary>
    /// 데미지 받음
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            ChangeState(new DeadState());  
        else
            ChangeState(new DamagedState());
    }

    /// <summary>
    /// 플레이어가 범위안에 들어온지 확인
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    /// <summary>
    /// 추격 등 움직일때 사용할 애니메이션
    /// </summary>
    /// <param name="isMove"></param>
    public void PlayMoveAnimation(bool isMove)
    {
        Animator.SetBool(AnimationData.MoveHash, isMove);
    }

    /// <summary>
    /// 피격 애니메이션 트리거
    /// </summary>
    public void PlayHitAnimation()
    {
        Animator.SetTrigger(AnimationData.HitHash);
    }

    /// <summary>
    /// 데드 애니메이션 트리거
    /// </summary>
    public void PlayDeadAnimation()
    {
        Animator.SetTrigger(AnimationData.DeadHash);
    }

    /// <summary>
    /// 보스 바라보는 방향 바꾸기
    /// </summary>
    /// <param name="directionX"></param>
    public void FlipSprite(float directionX)
    {
        if (Mathf.Abs(directionX) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(directionX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    // 초기화 함수
    public abstract void InitStateMachine();

    // 자식이 오버라이드하는 공격 함수
    public abstract void AttackPlayer();

    private void OnTriggerEnter2D(Collider2D other) // 추후 지우기 테스트용
    {
        if (other.gameObject.tag == "PlayerAttack")
        {
            TakeDamage(1);
            ChangeState(new DamagedState());
        }
    }
}
