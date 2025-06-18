using System.Collections;
using UnityEngine;

public class ReaperProjectileState : IBossState
{
    private Reaper boss;
    private float elapsed;
    private float waitTime;
    
    // 대기 후 재추격까지 추가 쿨다운
    private float postCooldown = 2f;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
        elapsed = 0f;

        // 애니메이션 트리거해서 Animation Event로 투사체 스폰 시작
        boss.PlayShotAnimation();

        // 후 대기 시간 계산
        float duration = boss.projectileInterval * (boss.projectileCount - 1);
        waitTime = duration + postCooldown;
    }

    public void Update()
    {
        elapsed += Time.deltaTime;

        // volley + cooldown 이 지나면
        if (elapsed >= waitTime)
        {
            // 여기에 분기 로직 추가
            boss.projectilePatternCount++;

            bool outOfRange = !boss.IsPlayerInRange(boss.DetectRange);
            if (boss.projectilePatternCount >= 3 && outOfRange)
            {
                // 패턴을 3회 시전 했지만 여전히 범위 밖이라면
                boss.projectilePatternCount = 0;
                boss.ChangeState(new ReaperBackstabState());
            }
            else
            {
                // 범위 안으로 들어왔으면 카운터 초기화
                if (!outOfRange)
                    boss.projectilePatternCount = 0;

                // 그 외엔 다시 추격
                boss.ChangeState(new ReaperChaseState());
            }
        }
    }

    public void FixedUpdate() { }

    public void Exit() { }
}