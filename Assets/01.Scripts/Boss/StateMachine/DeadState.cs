using UnityEngine;

public class DeadState : IBossState
{
    private BossBase boss;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        // 사망 애니메이션, 이펙트 실행
        boss.Animator.SetTrigger(boss.AnimationData.DeadHash);

        if (boss.rb != null)
        {
            boss.rb.velocity = Vector2.zero;
            boss.rb.simulated = false; // 물리 계산 중지 (옵션)
        }

        Object.Destroy(boss.gameObject, 2f); // 2초 후 삭제
        GameManager.Instance.Stage.OpenDoor();
    }

    public void Update()
    {
        // 죽었으니 아무것도 안 함
    }

    public void FixedUpdate()
    {

    }
    public void Exit()
    {
        // 
    }
}