using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class MiniSlime : MonoBehaviour
{

    [SerializeField] public Transform playerPos;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] KingSlime kingSlime;

    [SerializeField] Animator animator;
    [SerializeField] AnimatorController p2Slime;
    [SerializeField] AnimatorController p3Slime;
    [SerializeField] AnimatorController dead;

    public int maxHp = 3000;
    public int curHp;
    public float jumpPower = 5.0f; // 점프의 힘
    public float jumpInterval = 2f; // 점프 간격
    private float jumpTimer = 0f; // 점프 타이머
    private bool isDead = false;

    private void Awake()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        playerPos = _player.GetComponent<Transform>();

        GameObject _kingSlime = GameObject.Find("KingSlime");
        kingSlime = _kingSlime.GetComponent<KingSlime>();

        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        
        _rigidbody.freezeRotation = true; // 점프 시 회전을 하기에 회전을 멈추는 코드

        curHp = maxHp;

        StartCoroutine(DieForKing());
    }

    private void Update()
    {
        if (curHp <= 0)
        {
            Die();
        }

        if(kingSlime.nowPhase == "Phase2" && !isDead)
        {
            animator.runtimeAnimatorController = p2Slime;
        }
        else if (kingSlime.nowPhase == "Phase3" && !isDead)
        {
            animator.runtimeAnimatorController = p3Slime;
        }
    }

    private void FixedUpdate()
    {
        jumpTimer += Time.fixedDeltaTime;

        //점프후 2초가 지나면
        if (jumpTimer >= jumpInterval)
        {

            Chase();
            jumpTimer = 0f;
        }
    }


    public void Chase()
    {

        Vector2 direction = (playerPos.position - transform.position).normalized;
        Vector2 jumpDirection = new Vector2(direction.x, 1f).normalized;

        if (playerPos.position.y - transform.position.y > 3.0f) jumpPower *= 2;

        _rigidbody.AddForce(jumpDirection * jumpPower, ForceMode2D.Impulse);

        jumpPower = 5;
    }

    public void Damaged()
    {
        curHp -= 10;// 나중에는 플레이어의 데미지를 가져와 적용
        Vector2 knockbackDir = new Vector2(-(playerPos.position.x - transform.position.x), 0f).normalized;
        _rigidbody.AddForce(knockbackDir * 3f, ForceMode2D.Impulse);
    }

    public IEnumerator DieForKing()
    {
        yield return new WaitForSeconds(9f);
        isDead = true;
        animator.runtimeAnimatorController = dead;
        Destroy(gameObject, 1f);
    }

    public void Die()
    {
        isDead = true;
        animator.runtimeAnimatorController = dead;
        kingSlime.curMiniSlime -= 1;
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 슬라임과 닿으면 데미지
    }
}
