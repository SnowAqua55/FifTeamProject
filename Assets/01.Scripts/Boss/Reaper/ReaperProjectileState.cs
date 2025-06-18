using System.Collections;
using UnityEngine;

public class ReaperProjectileState : IBossState
{
    private Reaper boss;
    private float elapsed;
    private float waitTime;

    // 투사체 패턴 설정 (Reaper에서 가져옴)
    // volleyDuration = (projectileCount - 1) * projectileInterval
    // 대기 후 재추격까지 추가 쿨다운
    private float postVolleyCooldown = 2f;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
        elapsed = 0f;

        // 1) 애니메이션 트리거해서 Animation Event로 투사체 스폰 시작
        boss.PlayShotAnimation();

        // 2) volley 후 대기 시간 계산
        float volleyDuration = boss.projectileInterval * (boss.projectileCount - 1);
        waitTime = volleyDuration + postVolleyCooldown;
    }

    public void Update()
    {
        elapsed += Time.deltaTime;

        // 3) 전체 대기 시간이 지나면 추격 상태로 복귀
        if (elapsed >= waitTime)
        {
            boss.ChangeState(new ReaperChaseState());
        }
    }

    public void FixedUpdate() { }

    public void Exit() { }
}