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

        if (elapsedTime >= boss.AttackCooldown)
        {
            elapsedTime = 0f;  // 쿨다운 초기화

            if (boss.IsPlayerInRange(boss.AttackRange))
            {
                boss.AttackPlayer();  // 계속 공격 실행 (상태 전환 없이)
            }
            else
            {
                boss.ChangeState(new ChaseState());  // 범위 벗어나면 상태 전환
            }
        }
    }

    public void FixedUpdate()
    {

    }

    public void Exit()
    {
        // 공격 상태 종료 처리
    }
}
