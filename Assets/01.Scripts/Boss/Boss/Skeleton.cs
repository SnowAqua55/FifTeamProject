using UnityEngine;

public class Skeleton : BossBase
{
    public override void InitStateMachine()
    {
        ChangeState(new IdleState()); // 시작 상태
    }

    public override void AttackPlayer()
    {
        // 공격 생략
    }
}
