using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    public int maxHP = 5;
    private int currentHP;

    [Header("피격 무적 설정")]
    [Tooltip("피격 후 무적 유지 시간(초)")]
    public float invincibleDuration = 1f;
    [Tooltip("피격 깜빡임 주기(초)")]
    public float flashInterval = 0.1f;

    private bool isInvincible;
    private bool isDead;
    private SpriteRenderer sr;
    private Animator animator;

    void Awake()
    {
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    // 외부에서 데미지 요청 시 호출
    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;
        
        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("Hurt");
        UIManager.Instance.UpdateHeart();
        StartCoroutine(HurtCoroutine());
    }
    
    IEnumerator HurtCoroutine()
    {
        if (isDead) yield break;
        
        isInvincible = true;
        float timer = 0f;

        // 깜빡임 + 무적 타이머
        while (timer < invincibleDuration)
        {
            sr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(flashInterval);
            sr.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval * 2f;
        }

        isInvincible = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        if (currentHP <= 0)
        {
            StartCoroutine(DieCoroutine());
        }
    }

    IEnumerator DieCoroutine()
    {
        var pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false; // 플레이어 이동 잠금
        
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1f); // 애니메이션 유예 시간

        Time.timeScale = 0;
        // 이후 사망 UI 처리 등
    }
    
    public void ActivateInvincibility(float duration)
    {
        if (isInvincible) return;
        StartCoroutine(InvincibilityCoroutine(duration));
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        isInvincible = false;
    }

    // 체력 반환 (UI 등 연결용)
    public int GetMaxHP() => maxHP;
    public int GetCurrentHP() => currentHP;
}