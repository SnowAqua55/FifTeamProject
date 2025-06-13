using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    protected IBossState currentState;

    [Header("보스 데이터")]
    [SerializeField] private BossData bossData; // SO 연결
    

    protected float currentHP;
    protected Rigidbody2D rb;

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

    public void ChangeState(IBossState newState)
    {
        if (currentState?.GetType() == newState.GetType()) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            ChangeState(new DeadState());
        else
            ChangeState(new DamagedState());
    }

    public bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    // 초기화 함수
    public abstract void InitStateMachine();

    // 자식이 오버라이드하는 공격 함수
    public abstract void AttackPlayer();
}
