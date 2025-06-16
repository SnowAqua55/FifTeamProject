using UnityEngine;

[CreateAssetMenu(menuName = "Boss/BossBaseData")]
public class BossData : ScriptableObject
{
    public float maxHP;
    public float moveSpeed;
    public float attackRange;
    public float attackDamage;

    public float detectRange = 5f;
    public float attackCooldown = 1.5f;
    public float staggerTime = 0.5f;

    // 모든 보스가 공통적으로 가질만한 스탯
    // 특정 보스에게만 해당하는 것은 상속
}