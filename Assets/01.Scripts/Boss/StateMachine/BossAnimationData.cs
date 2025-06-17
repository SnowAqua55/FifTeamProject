using System;
using UnityEngine;

[Serializable]
public class GolemBossAnimationData
{
    [SerializeField] private string moveParam = "isMove";
    [SerializeField] private string hitParam = "isHit";
    [SerializeField] private string deadParam = "isDead";
    [SerializeField] private string attackParam = "DoAttack";

    public int MoveHash { get; private set; }
    public int HitHash { get; private set; }
    public int DeadHash { get; private set; }
    public int AttackHash { get; private set; }

    public void Initialize()
    {
        MoveHash = Animator.StringToHash(moveParam);
        HitHash = Animator.StringToHash(hitParam);
        DeadHash = Animator.StringToHash(deadParam);
        AttackHash = Animator.StringToHash(attackParam);
    }
}

