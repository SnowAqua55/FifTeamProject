using System;
using UnityEngine;

[Serializable]
public class BossAnimationData
{
    [SerializeField] private string moveParam = "isMove";
    [SerializeField] private string hitParam = "isHit";
    [SerializeField] private string deadParam = "isDead";

    [SerializeField] private string bodyLaserChargeParam = "laserCharge"; // 본체 레이저 충전
    [SerializeField] private string bodyLaserFireParam = "laserFire";     // 본체 레이저 발사
    [SerializeField] private string invincibilityIdleParam = "InvincibilityIdle"; // 무적 애니메이션

    public int MoveHash { get; private set; }
    public int HitHash { get; private set; }
    public int DeadHash { get; private set; }

    public int BodyLaserChargeHash { get; private set; }
    public int BodyLaserFireHash { get; private set; }
    public int InvincibilityIdleHash { get; private set; }

    public void Initialize()
    {
        MoveHash = Animator.StringToHash(moveParam);
        HitHash = Animator.StringToHash(hitParam);
        DeadHash = Animator.StringToHash(deadParam);

        BodyLaserChargeHash = Animator.StringToHash(bodyLaserChargeParam);
        BodyLaserFireHash = Animator.StringToHash(bodyLaserFireParam);
        InvincibilityIdleHash = Animator.StringToHash(invincibilityIdleParam);
    }
}

