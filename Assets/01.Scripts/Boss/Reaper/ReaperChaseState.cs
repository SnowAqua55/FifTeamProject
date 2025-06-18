using UnityEngine;

public class ReaperChaseState : IBossState
{
    private Reaper boss;
    private Vector2 moveDir;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;               // Reaper로 캐스팅
        boss.PlayMoveAnimation(true);
    }

    public void Update()
    {
        // 플레이어 바라보기
        moveDir = (boss.player.position - boss.transform.position).normalized;
        boss.FlipSprite(moveDir.x);

        // DetectRange 밖이면 투사체 패턴
        if (!boss.IsPlayerInRange(boss.DetectRange))
        {
            boss.ChangeState(new ReaperProjectileState());
            return;
        }

        // AttackRange 안이면 근접공격
        if (boss.IsPlayerInRange(boss.AttackRange))
        {
            boss.ChangeState(new ReaperAttackState());
            return;
        }
    }

    public void FixedUpdate()
    {
        Vector2 p = boss.rb.position + moveDir * boss.MoveSpeed * Time.fixedDeltaTime;
        boss.rb.MovePosition(p);
    }

    public void Exit()
    {
        boss.PlayMoveAnimation(false);
    }
}