using System;
using UnityEngine;

[Serializable]
public class BossAnimationData
{
    [SerializeField] private string moveParam = "isMove";
    [SerializeField] private string hitParam = "isHit";
    [SerializeField] private string deadParam = "isDead";
    [SerializeField] private string attackParam = "DoAttack";

    [SerializeField] private string bodyLaserChargeParam = "laserCharge"; // 본체 레이저 충전
    [SerializeField] private string bodyLaserEndParam = "laserEnd";     // 본체 레이저 발사
    [SerializeField] private string invincibilityIdleParam = "Invincible"; // 무적 애니메이션

    public int MoveHash { get; private set; }
    public int HitHash { get; private set; }
    public int DeadHash { get; private set; }
    public int AttackHash { get; private set; }

    public int BodyLaserChargeHash { get; private set; }
    public int BodyLaserEndHash { get; private set; }
    public int InvincibilityIdleHash { get; private set; }

    public void Initialize()
    {
        MoveHash = Animator.StringToHash(moveParam);
        HitHash = Animator.StringToHash(hitParam);
        DeadHash = Animator.StringToHash(deadParam);
        AttackHash = Animator.StringToHash(attackParam);

        BodyLaserChargeHash = Animator.StringToHash(bodyLaserChargeParam);
        BodyLaserEndHash = Animator.StringToHash(bodyLaserEndParam);
        InvincibilityIdleHash = Animator.StringToHash(invincibilityIdleParam);
    }
}

