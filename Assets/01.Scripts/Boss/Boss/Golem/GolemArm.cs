using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GolemArm : MonoBehaviour
{
    private Golem parentBoss;
    private Animator armAnimator;
    private float currentHP;
    private float attackDamage;
    private float attackCooldown;
    private GameObject armProjectilePrefab;

    [Header("Arm Movement (Configured by BossData)")]
    private float rotationSpeed;
    private float orbitRadius;
    private float currentOrbitAngle = 0f;

    private bool isMovingToPatternPosition = false;
    private Vector3 targetPatternPosition;

    private Coroutine armAttackRoutineInstance;

    public void Initialize(Golem boss, GolemBossData.ArmData armConfig, Vector3 initialWorldPosition)
    {
        parentBoss = boss;

        currentHP = armConfig.maxHP;
        attackDamage = armConfig.attackDamage;
        attackCooldown = armConfig.attackCooldown;
        armProjectilePrefab = armConfig.projectilePrefab;
        rotationSpeed = armConfig.armOrbitSpeed;
        orbitRadius = armConfig.armOrbitRadius;

        armAnimator = GetComponent<Animator>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0;

        transform.position = initialWorldPosition;

        Vector2 initialOffset = transform.position - parentBoss.transform.position;
        currentOrbitAngle = Mathf.Atan2(initialOffset.y, initialOffset.x) * Mathf.Rad2Deg;

        gameObject.SetActive(true);
        isMovingToPatternPosition = false;

        if (armAttackRoutineInstance != null) StopCoroutine(armAttackRoutineInstance);
        armAttackRoutineInstance = StartCoroutine(ArmAttackRoutine());
    }

    void Update()
    {
        if (parentBoss == null || parentBoss.IsDead)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!isMovingToPatternPosition)
        {
            currentOrbitAngle += rotationSpeed * Time.deltaTime;
            UpdateOrbitPosition();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPatternPosition, parentBoss.MoveSpeed * Time.deltaTime * 1.5f);
            if (Vector3.Distance(transform.position, targetPatternPosition) < 0.1f)
            {
                isMovingToPatternPosition = false;
            }
        }
    }

    private void UpdateOrbitPosition()
    {
        if (parentBoss == null) return;

        float angleRad = currentOrbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * orbitRadius;
        transform.position = parentBoss.transform.position + offset;

        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (parentBoss.player == null) return;

        Vector2 directionToPlayer = (parentBoss.player.position - transform.position).normalized;
        transform.right = directionToPlayer;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log($"팔 '{gameObject.name}' 데미지 입음. 현재 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator ArmAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (parentBoss == null || parentBoss.player == null || !gameObject.activeSelf)
            {
                yield break;
            }

            if (!isMovingToPatternPosition)
            {
                PerformAttack();
            }
        }
    }

    public void PerformAttack()
    {
        int attackType = Random.Range(0, 2);
        if (attackType == 0)
        {
            ShootPlayerTargeted();
        }
        else
        {
            ShootDirectional();
        }
    }

    private void ShootPlayerTargeted()
    {
        if (parentBoss.player == null || armProjectilePrefab == null) return;

        GameObject projectileObj = Instantiate(armProjectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            Vector2 direction = (parentBoss.player.position - transform.position).normalized;
            projectile.SetDirection(direction);
            projectile.SetDamage(attackDamage);
        }
    }

    private void ShootDirectional()
    {
        if (armProjectilePrefab == null) return;

        // armAnimator?.SetTrigger("ArmShoot"); 

        GameObject projectileObj = Instantiate(armProjectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right,
                                     new Vector2(1,1).normalized, new Vector2(1,-1).normalized,
                                     new Vector2(-1,1).normalized, new Vector2(-1,-1).normalized };
            Vector2 direction = directions[Random.Range(0, directions.Length)];
            projectile.SetDirection(direction);
            projectile.SetDamage(attackDamage);
        }
    }

    private void Die()
    {
        Debug.Log("팔 '" + gameObject.name + "' 비활성화됨!");
        if (parentBoss != null)
        {
            parentBoss.OnArmDestroyed(this);
        }
        gameObject.SetActive(false);
        if (armAttackRoutineInstance != null) StopCoroutine(armAttackRoutineInstance);
    }

    public void MoveArmToPatternPosition(Vector3 targetPos)
    {
        targetPatternPosition = targetPos;
        isMovingToPatternPosition = true;
    }
}