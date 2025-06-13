using UnityEngine;

[CreateAssetMenu(menuName = "Boss/BossData")]
public class BossData : ScriptableObject
{
    public float maxHP;
    public float moveSpeed;
    public float attackRange;
    public float attackDamage;

    public float detectRange = 5f;
    public float attackCooldown = 1.5f;
    public float staggerTime = 0.5f;

    // 스킬 쿨다운, 패턴 등 추가
}