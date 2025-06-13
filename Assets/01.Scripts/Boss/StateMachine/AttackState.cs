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

        // 플레이어가 공격 범위를 벗어나면 쿨타임과 상관없이 상태 전환
        if (!boss.IsPlayerInRange(boss.AttackRange))
        {
            // 공격 범위 벗어나면 추적 상태로 전환
            boss.ChangeState(new ChaseState());
            return;
        }

        // 공격 쿨타임이 끝났으면 공격 실행
        if (elapsedTime >= boss.AttackCooldown)
        {
            elapsedTime = 0f;
            boss.AttackPlayer();
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
