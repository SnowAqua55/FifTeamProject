using UnityEngine;

public class ChaseState : IBossState
{
    private BossBase boss;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        // 추적 애니메이션 실행 등
    }

    public void Update()
    {
        // 플레이어에게 이동
        Vector2 direction = (boss.player.position - boss.transform.position).normalized;
        boss.transform.position += (Vector3)(direction * boss.MoveSpeed * Time.deltaTime);

        // 일정 거리 안에 들어오면 공격 상태로 전환
        if (boss.IsPlayerInRange(boss.AttackRange))
        {
            boss.ChangeState(new AttackState());
        }
    }

    public void Exit()
    {
        // Chase 상태 종료 처리
    }
}