using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSlime : MonoBehaviour
{

    [SerializeField] public Transform playerPos;
    [SerializeField] Rigidbody2D _rigidbody;

    public int maxHp = 3000;
    public int curHp;
    public float jumpPower = 5.0f; // 점프의 힘
    public float jumpInterval = 0.8f; // 점프 간격
    private float jumpTimer = 0f; // 점프 타이머

    private void Awake()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        playerPos = _player.GetComponent<Transform>();
    }
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        
        _rigidbody.freezeRotation = true; // 점프 시 회전을 하기에 회전을 멈추는 코드

        curHp = maxHp;

        Destroy(gameObject, 10f);
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

    public void Die()
    {

    }
}
