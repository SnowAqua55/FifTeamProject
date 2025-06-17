using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public enum State
{
    Move,
    Damaged,
    Skill,
    Change_Phase,
    Die
}

public class KingSlime : MonoBehaviour
{
    public Rigidbody2D _rigidbody;
    [SerializeField] public Transform player;

    [Header("������ �̹���")]
    [SerializeField] Sprite phase2;
    [SerializeField] Sprite phase3;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("������ ��ȯ��")]
    [SerializeField] private GameObject slimeball;
    [SerializeField] private GameObject miniSlime;

    [Header("������ �ִϸ��̼�")]
    [SerializeField] Animator animator;
    [SerializeField] AnimatorController phase1Jump;
    [SerializeField] AnimatorController phase2Jump;
    [SerializeField] AnimatorController phase3Jump;
    [SerializeField] AnimatorController phase1hurt;
    [SerializeField] AnimatorController phase2hurt;
    [SerializeField] AnimatorController phase3hurt;
    [SerializeField] AnimatorController dead;

    

    [Header("������ ����")]
    public string nowPhase = "Phase1";
    public int maxHp = 10000;
    public int curHp;
    public int attPower;
    public float jumpPower = 5.0f; // ������ ��

    [Header("������ ����")]
    public State state;
    public float jumpInterval = 4f; // ���� ����
    private float jumpTimer = 0f; // ���� Ÿ�̸�
    private bool useSkill = false;
    private float skillTimer = 0f;
    private float skillInterval = 15f;
    private Vector3 oriScale;// ���� ũ��


    [Header("�̴� ������ ��")]
    public int maxMiniSlime = 3;
    public int curMiniSlime; // �̴� �������� ���� �� => HPȸ�� �޼��忡�� ���



    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        animator = GetComponentInChildren<Animator>();

        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.freezeRotation = true; // ���� �� ȸ���� �ϱ⿡ ȸ���� ���ߴ� �ڵ�

        curHp = maxHp;
        oriScale = transform.localScale;

        state = State.Move;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(nowPhase != "Phase1")
        {
            skillTimer += Time.deltaTime;
        }
        
        if (skillTimer >= skillInterval)
        {
            state = State.Skill;
        }

        if(curHp <= maxHp * 0.6 && nowPhase == "Phase1")
        {
            state = State.Change_Phase;
        }

        if (curHp <= maxHp * 0.2 && nowPhase == "Phase2")
        {
            state = State.Change_Phase;
        }

        if (curHp <= 0)
        {
            state = State.Die;
        }


        switch(state)
        {
            case State.Move:
                jumpTimer += Time.deltaTime;

                //������ 2�ʰ� ������
                if (jumpTimer >= jumpInterval)
                {
                    Chase();
                    jumpTimer = 0f;
                }
                break;

            case State.Skill:
                if(nowPhase == "Phase2" && useSkill == false)
                {
                    StartDownSlamAttack();
                    
                }

                if(nowPhase == "Phase3" && useSkill == false)
                {
                    int skill = Random.Range(1, 4);
                    if(skill == 1)
                    {
                        StartDownSlamAttack();
                    }
                    else if(skill == 2 || skill == 3)
                    {
                        SpownMiniSlimeSkill();
                    }
                }
                skillTimer = 0f;
                break;

            case State.Change_Phase:
                PhaseChange();
                break;

            case State.Die:
                Dead();
                break;
            case State.Damaged:
                Damaged();
                break;
        }
    }

   

    //�÷��̾� ���� �޼��� (������ó�� ������)
    public void Chase()
    {
        switch (nowPhase)
        {
            case "Phase1":
                animator.runtimeAnimatorController = phase1Jump;
                break;
            case "Phase2":
                animator.runtimeAnimatorController = phase2Jump;
                break;
            case "Phase3":
                animator.runtimeAnimatorController = phase3Jump;
                break;
        }

        StartCoroutine(NomalAttack());

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 jumpDirection = new Vector2(direction.x, 1f).normalized;

        if (player.position.y - transform.position.y > 3.0f) jumpPower *= 2;

        _rigidbody.AddForce(jumpDirection * jumpPower, ForceMode2D.Impulse);

        jumpPower = 5;

    }

    
    //�⺻ ����
    private IEnumerator NomalAttack()
    {
        while (!useSkill)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            Vector3 offset = (dir.x < 0f) ? new Vector3(-2f, 0f, 0f) : new Vector3(2f, 0f, 0f);


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
        //���� �� �ڷ�ƾ�� �ǽõǸ� �̲��������� ���߿��� ������
        //�׷��� �������� ���� ����

        useSkill = true;
        attPower *= 2;
        
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        transform.position = new Vector3(player.position.x, player.position.y + 10.0f, 0f);

        yield return new WaitForSeconds(2.0f);

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.AddForce(Vector2.down * 20f, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(1.0f);
        useSkill = false;
        state = State.Move;
        attPower = attPower / 2;
    }

    public void SpownMiniSlimeSkill()
    {
        useSkill = true;
        curMiniSlime = maxMiniSlime;

        Vector2 dir = (player.position - transform.position).normalized;
        Vector3 offset = (dir.x < 0f) ? new Vector3(-4f, -1f, 0f) : new Vector3(4f, -1f, 0f);

        for (int i = 0; i < maxMiniSlime; i++)
        {
            Instantiate(miniSlime, transform.position + offset, Quaternion.identity);
            offset.x += 1f;

        }

        useSkill = false;
        state = State.Move;
        
        Invoke("HpHeal", 10f);

        
    }
    public void HpHeal()
    {
        // �ʵ忡 ���� �̴� �������� ���̰� ���� �̴� �����ӿ� ���� ���� �ִ� hp 5%ȸ��
        int healAmount = (int)(maxHp * 0.05f) * curMiniSlime;
        curHp = Mathf.Min(curHp + healAmount, maxHp);
    }

    public void PhaseChange()
    {
        if (nowPhase == "Phase1")
        {
            nowPhase = "Phase2";
            // Ǫ�� ���������� ��ȯ
            spriteRenderer.sprite = phase2;
            animator.runtimeAnimatorController = phase2Jump;
            jumpInterval = 1.5f; //���� ������ �����Ͽ� �̼� ����

        }
        else if (nowPhase == "Phase2")
        {
            nowPhase = "Phase3";
            // ���� ���������� ��ȯ
            spriteRenderer.sprite = phase3;
            animator.runtimeAnimatorController = phase3Jump;
            attPower *= 2;
            jumpInterval = 1.0f;
        }

        state = State.Move;
    }

    public void Damaged()
    {
        switch (nowPhase)
        {
            case "Phase1":
                animator.runtimeAnimatorController = phase1hurt;
                break;
            case "Phase2":
                animator.runtimeAnimatorController = phase1hurt;
                break;
            case "Phase3":
                animator.runtimeAnimatorController = phase3hurt;
                break;
        }

        curHp -= 10;// ���߿��� �÷��̾��� �������� ������ ����
        Vector2 knockbackDir = new Vector2(-(player.position.x - transform.position.x), 0f).normalized;
        _rigidbody.AddForce(knockbackDir * 4f, ForceMode2D.Impulse);

        state =State.Move;
    }

    public void Dead()
    {
        StopAllCoroutines();
        animator.runtimeAnimatorController = dead;
        // ���� �ִϸ��̼� �� �������� �޼���
        Destroy(gameObject, 1.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �����Ӱ� ������ ������
    }
}
