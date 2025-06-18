using System.Collections;
using UnityEngine;

public class ReaperPhaseTransitionState : IBossState
{
    private Reaper boss;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
        boss.StartCoroutine(PhaseRoutine());
    }

    public IEnumerator PhaseRoutine()
    {
        // 무적(콜라이더 끄기)
        boss.rb.simulated = false;
        boss.coll.enabled = false;
        
        // Idle 애니 + 파티클
        boss.Animator.Play("Reaper_Idle");
        boss.swirlParticle.gameObject.SetActive(true);
        boss.swirlParticle.Clear();
        boss.swirlParticle.Play();
        yield return new WaitForSeconds(boss.swirlParticle.main.duration);
        
        // 백스탭 3회
        for (int i = 0; i < 3; i++)
        {
            boss.PlayBackstabAnimation();
            yield return new WaitForSeconds(boss.backstabDuration + boss.backstabEffectInterval);
        }

        // 공중으로 부드럽게 이동하며 잔상 남기기
        Vector3 start = boss.transform.position;
        Vector3 end = boss.topCenterPos.position;
        float moveDuration = 1f; // 이동에 걸릴 시간
        float timer = 0f;
        float afterImageTimer = 0f;
        float afterImageInterval = 0.1f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;
            boss.transform.position = Vector3.Lerp(start, end, t);

            afterImageTimer += Time.deltaTime;
            if (afterImageTimer >= afterImageInterval)
            {
                boss.SpawnAfterImage();
                afterImageTimer = 0f;
            }

            yield return null;
        }

        boss.transform.position = end;

        yield return new WaitForSeconds(0.5f); // 잠깐 멈춤

        // Heavy Slash 애니 재생
        boss.PlayHeavySlashAnimation();

        // 한 프레임 기다려서 Animator 에 트리거 반영
        yield return null;
        // 슬래시 상태 진입 대기
        AnimatorStateInfo info0;
        do
        {
            info0 = boss.Animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 매 프레임 대기
        } while (!info0.IsName("Reaper_HeavySlash"));
        // 진입했으면 duration 만큼 대기
        yield return new WaitForSeconds(info0.length);
        
        boss.PlayDiveAnimation();
        yield return null;
        AnimatorStateInfo info1;
        do {
            info1 = boss.Animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (!info1.IsName("Reaper_Dive"));
        yield return new WaitForSeconds(info1.length);

        // 후광 & 속도 버프
        boss.haloObject.SetActive(true);
        // boss.MoveSpeed *= boss.phaseSpeedMultiplier;
        boss.projectileInterval /= boss.phaseSpeedMultiplier;
        boss.teleportPatternInterval /= boss.phaseSpeedMultiplier;

        // 추격 모드로 복귀
        boss.ChangeState(new ReaperChaseState());
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}