using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Boss/GolemBossData")]
public class GolemBossData : BossData
{
    [Header("팔 데이터")]
    public ArmData golemArmData; // 골렘 보스 전용 팔 데이터

    [Header("골렘 레이저 공격 설정")]
    public float bodyLaserChargeTime = 1.5f;
    public float bodyLaserDuration = 0.7f;
    public float rainLaserWarningDuration = 1f;
    public float rainLaserInterval = 0.5f;
    public float rainLaserFallDuration = 0.5f;
    public int numberOfRainLasers = 5;

    [Serializable]
    public struct ArmData
    {
        public float maxHP;
        public float attackDamage;
        public float attackCooldown;
        public GameObject projectilePrefab;
        public float armOrbitSpeed;
        public float armOrbitRadius;
    }
}