public class IdleState : IBossState
{
    private BossBase boss;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        // 대기 애니메이션 실행, 초기화 등
    }

    public void Update()
    {
        // 플레이어가 일정 범위 안에 들어오면 추적 상태로 전환
        if (boss.IsPlayerInRange(boss.DetectRange))
        {
            boss.ChangeState(new ChaseState());
        }
    }

    public void Exit()
    {
        // Idle 상태 종료 처리
    }
}