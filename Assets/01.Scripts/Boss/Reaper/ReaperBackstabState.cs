using System.Collections;
using UnityEngine;

public class ReaperBackstabState : IBossState
{
    private Reaper boss;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
        // 애니메이션 트리거
        boss.PlayBackstabAnimation();
        // 애니메이션 길이만큼 기다렸다가 복귀
        boss.StartCoroutine(BackstabRoutine());
    }

    private IEnumerator BackstabRoutine()
    {
        // 전체 애니메이션 길이 대기
        yield return new WaitForSeconds(boss.backstabDuration);

        // 쿨다운 대기
        yield return new WaitForSeconds(boss.AttackCooldown);
        
        boss.ChangeState(new ReaperChaseState());
    }

    public void Update()     { }
    public void FixedUpdate(){ }
    public void Exit()       { }
}