using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningBoss : BossBase
{
    public override void InitStateMachine()
    {
        ChangeState(new IdleState());
    }

    public override void AttackPlayer()
    {
       //도망만 다니는 보스구현
       /*if (AttackRange >= Vector2.Distance(player.transform.position, transform.position))
       {
           //도망만가기
       }
       */
    }
}
