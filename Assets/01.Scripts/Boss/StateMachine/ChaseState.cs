using UnityEngine;

public class ChaseState : IBossState
{
    private BossBase boss;
    private Vector2 moveDirection;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        // 추적 애니메이션 실행 등
        boss.PlayMoveAnimation(true);
    }

    public void Update()
    {
        // 추적 방향 계산
        if (boss.player != null)
        {
            moveDirection = (boss.player.position - boss.transform.position).normalized;
            boss.FlipSprite(moveDirection.x);
        }

        if (!boss.IsPlayerInRange(boss.DetectRange))
        {
            boss.ChangeState(new IdleState());  // 탐지 범위 밖이면 대기 상태
            return;
        }

        // 공격 사거리 진입 시 상태 전환
        if (boss.IsPlayerInRange(boss.AttackRange))
        {
            boss.ChangeState(new AttackState());
        }
    }

    public void FixedUpdate()
    {
        // Rigidbody2D 물리 기반 이동 처리
        Vector2 newPosition = boss.rb.position + moveDirection * boss.MoveSpeed * Time.fixedDeltaTime;
        boss.rb.MovePosition(newPosition);
    }

    public void Exit()
    {
        // Chase 상태 종료 처리
        boss.PlayMoveAnimation(false);
    }
}