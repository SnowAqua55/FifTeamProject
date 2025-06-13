using UnityEngine;

public class AttackState : IBossState
{
    private BossBase boss;
    private float elapsedTime = 0f;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        elapsedTime = 0f;
        // 공격 애니메이션 시작
        boss.AttackPlayer(); // 각 보스마다 다르게 / 나중에 공격 발동 조절 필요
    }

    public void Update()
    {
        elapsedTime += Time.deltaTime;

        // 공격 쿨타임 후 다시 Idle 또는 Chase 상태로
        if (elapsedTime >= boss.AttackCooldown)
        {
            if (boss.IsPlayerInRange(boss.AttackRange))
                boss.ChangeState(new AttackState()); // 반복 공격
            else
                boss.ChangeState(new ChaseState());
        }
    }

    public void Exit()
    {
        // 공격 상태 종료 처리
    }
}
