using UnityEngine;

public class DamagedState : IBossState
{
    private BossBase boss;
    private float elapsedTime = 0f;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        boss.PlayHitAnimation();
        elapsedTime = 0f;
        // 피격 애니메이션, 넉백 등 처리
    }

    public void Update()
    {
        if (boss.CurrentHP <= 0)
        {
            boss.ChangeState(new DeadState());
        }
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= boss.StaggerTime)
        {
            // 다시 추적하거나 대기로 전환
            if (boss.IsPlayerInRange(boss.AttackRange))
                boss.ChangeState(new AttackState());
            else
                boss.ChangeState(new ChaseState());
        }
    }

    public void FixedUpdate()
    {

    }

    public void Exit()
    {
        // 상태 종료 처리
    }
}