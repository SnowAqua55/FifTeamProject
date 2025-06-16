using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KingSlime : MonoBehaviour
{
    // �÷��̾� ������Ʈ �޾ƿ���

    [SerializeField] private GameObject slimeball;
    public Rigidbody2D _rigidbody;
    [SerializeField] public Transform player;

    private Coroutine nomalAttCoroutine;
    private Coroutine downSlamCoroutine;


    [Header("������ ����")]
    public int maxHp = 10000;
    public int curHp;
    public int attPower;
    public float jumpPower= 5.0f; // ������ ��
    public float jumpInterval = 2f; // ���� ����
    private float jumpTimer = 0f; // ���� Ÿ�̸�
    private bool useSkill = false; // ��ų ����� ����

    [Header("���� �̴� ������ ��")]
    public int curMiniSlime; // �̴� �������� ���� �� => HPȸ�� �޼��忡�� ���

    

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        curHp = maxHp;
        _rigidbody.freezeRotation = true; // ���� �� ȸ���� �ϱ⿡ ȸ���� ���ߴ� �ڵ�

        if(nomalAttCoroutine != null)
        {
            StopCoroutine(nomalAttCoroutine);
        }

        nomalAttCoroutine = StartCoroutine(NomalAttack());
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate()
    {
        jumpTimer += Time.fixedDeltaTime;
        
        //������ 2�ʰ� ������
        if(jumpTimer >= jumpInterval)
        {
              
            Chase();
            jumpTimer = 0f; 
        }
    }

    public void Idle()
    {

    }

    public void KigSlimeState()
    {

    }

    //�÷��̾� ���� �޼��� (������ó�� ������)
    public void Chase()
    {
        if (useSkill) return; 
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 jumpDirection = new Vector2 (direction.x, 1f).normalized;

        if (player.position.y - transform.position.y > 3.0f) jumpPower *= 2;
        
        _rigidbody.AddForce(jumpDirection * jumpPower, ForceMode2D.Impulse);

        jumpPower = 5;
         
    }

    //�⺻ ����
    private IEnumerator NomalAttack()
    {       
        Vector2 dir = (player.position - transform.position).normalized;
        Vector3 offset = (dir.x < 0f) ? new Vector3(-1f, 1f, 0f) : new Vector3(1f, 1f, 0f);

        while (!useSkill)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(slimeball, transform.position + offset, Quaternion.identity);
                yield return new WaitForSeconds(0.8f); 
            }
        }
    }

    public void StartDownSlamAttack()
    {
        StartCoroutine(DownSlamAttack());
    }

    private IEnumerator DownSlamAttack()
    {
        useSkill = true;

        //���� �� �ڷ�ƾ�� �ǽõǸ� �̲��������� ���߿��� ������
        //�������� ���� ����
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        transform.position = new Vector3(player.position.x, player.position.y + 8.0f, 0f);
       
        yield return new WaitForSeconds(3.0f);

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.AddForce(Vector2.down * 20f, ForceMode2D.Impulse);
        
        useSkill = false;

    }

    public void SpownMiniSlimeSkill()
    {
        useSkill = true;
    }
    public void HpHeal()
    {

    }

    public void PhaseChange()
    {

    }

    public void Dead()
    {
        // ���� �ִϸ��̼� �� �������� �޼���
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �����Ӱ� ������ ������
    }
}
