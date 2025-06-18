using UnityEngine;

public class ReaperAttackState : IBossState
{
    private Reaper boss;
    private float elapsedTime;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
        elapsedTime = 0f;
        
        // 애니메이션 트리거
        boss.PlayAttackAnimation();
    }

    public void Update()
    {
        elapsedTime += Time.deltaTime;

        // 사거리 이탈 시 다시 추격 상태로
        if (!boss.IsPlayerInRange(boss.AttackRange))
        {
            boss.ChangeState(new ReaperChaseState());
            return;
        }

        // 쿨다운 끝나면 재공격
        if (elapsedTime >= boss.AttackCooldown)
        {
            elapsedTime = 0f;
            boss.PlayAttackAnimation();
        }
    }

    public void FixedUpdate() { }

    public void Exit() { }
}