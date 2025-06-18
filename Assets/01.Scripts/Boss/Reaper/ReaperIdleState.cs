public class ReaperIdleState : IBossState
{
    private Reaper boss;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
    }

    public void Update()
    {
        // DetectRange 밖이면 투사체
        if (!boss.IsPlayerInRange(boss.DetectRange))
        {
            boss.ChangeState(new ReaperProjectileState());
            return;
        }
        // DetectRange 안이면 추격
        if (boss.IsPlayerInRange(boss.DetectRange))
        {
            boss.ChangeState(new ReaperChaseState());
            return;
        }
    }

    public void FixedUpdate() { }
    public void Exit() { }
}