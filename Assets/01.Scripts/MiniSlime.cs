using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSlime : MonoBehaviour
{

    [SerializeField] public Transform playerPos;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] KingSlime kingSlime;

    public int maxHp = 3000;
    public int curHp;
    public float jumpPower = 5.0f; // ������ ��
    public float jumpInterval = 0.8f; // ���� ����
    private float jumpTimer = 0f; // ���� Ÿ�̸�

    private void Awake()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        playerPos = _player.GetComponent<Transform>();

        GameObject _kingSlime = GameObject.Find("KingSlime");
        kingSlime = _kingSlime.GetComponent<KingSlime>();
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        
        _rigidbody.freezeRotation = true; // ���� �� ȸ���� �ϱ⿡ ȸ���� ���ߴ� �ڵ�

        curHp = maxHp;

        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if (curHp <= 0)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        jumpTimer += Time.fixedDeltaTime;

        //������ 2�ʰ� ������
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
        curHp -= 10;// ���߿��� �÷��̾��� �������� ������ ����
        Vector2 knockbackDir = new Vector2(-(playerPos.position.x - transform.position.x), 0f).normalized;
        _rigidbody.AddForce(knockbackDir * 3f, ForceMode2D.Impulse);
    }


    public void Die()
    {
        kingSlime.curMiniSlime -= 1;
        Destroy(gameObject, 1f);
    }
}
