using UnityEngine;

public class DeadState : IBossState
{
    private BossBase boss;

    public void Enter(BossBase boss)
    {
        this.boss = boss;
        // 사망 애니메이션, 이펙트 실행
        Object.Destroy(boss.gameObject, 2f); // 2초 후 삭제
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