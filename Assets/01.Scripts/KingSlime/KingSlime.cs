using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum State
{
    Move,
    Skill,
    Change_Phase,
    Die
}

public class KingSlime : MonoBehaviour
{
    [SerializeField] Sprite phase2;
    [SerializeField] Sprite phase3;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject slimeball;
    [SerializeField] private GameObject miniSlime;
    public Rigidbody2D _rigidbody;
    [SerializeField] public Transform player;


    private Coroutine nomalAttCoroutine;
    private Coroutine downSlamCoroutine;


    [Header("슬라임 스탯")]
    public string nowPhase = "Phase1";
    public int maxHp = 10000;
    public int curHp;
    public int attPower;
    public float jumpPower = 5.0f; // 점프의 힘

    [Header("슬라임 관련")]
    public State state;
    public float jumpInterval = 2f; // 점프 간격
    private float jumpTimer = 0f; // 점프 타이머
    private float skillTimer = 0f;
    private float skillInterval = 15f;
    private Vector3 oriScale;// 원래 크기


    [Header("미니 슬라임 수")]
    public int maxMiniSlime = 3;
    public int curMiniSlime; // 미니 슬라임이 남은 수 => HP회복 메서드에서 사용



    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.freezeRotation = true; // 점프 시 회전을 하기에 회전을 멈추는 코드

        curHp = maxHp;
        oriScale = transform.localScale;

        if (nomalAttCoroutine != null)
        {
            StopCoroutine(nomalAttCoroutine);
        }

        nomalAttCoroutine = StartCoroutine(NomalAttack());

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

        if(curHp <= maxHp * 0.6)
        {
            state = State.Change_Phase;
        }

        if(curHp <= 0)
        {
            state = State.Die;
        }


        switch(state)
        {
            case State.Move:
                jumpTimer += Time.deltaTime;

                //점프후 2초가 지나면
                if (jumpTimer >= jumpInterval)
                {
                    Chase();
                    jumpTimer = 0f;
                }
                break;

            case State.Skill:
                if(nowPhase == "Phase2")
                {
                    StartDownSlamAttack();
                }

                if(nowPhase == "Phase3")
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

                break;

            case State.Change_Phase:
                PhaseChange();
                break;

            case State.Die:
                Dead();
                break;
        }
    }

   

    //플레이어 추적 메서드 (슬라임처럼 점프로)
    public void Chase()
    {

        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 jumpDirection = new Vector2(direction.x, 1f).normalized;

        if (player.position.y - transform.position.y > 3.0f) jumpPower *= 2;

        _rigidbody.AddForce(jumpDirection * jumpPower, ForceMode2D.Impulse);

        jumpPower = 5;

    }

    //기본 공격
    private IEnumerator NomalAttack()
    {
        
        Vector2 dir = (player.position - transform.position).normalized;
        Vector3 offset = (dir.x < 0f) ? new Vector3(-2f, 0f, 0f) : new Vector3(2f, 0f, 0f);


        for (int i = 0; i < 3; i++)
        {
            Instantiate(slimeball, transform.position + offset, Quaternion.identity);
            yield return new WaitForSeconds(0.8f);
        }

    }

    public void StartDownSlamAttack()
    {
        StartCoroutine(DownSlamAttack());
    }

    private IEnumerator DownSlamAttack()
    {
        //추적 중 코루틴이 실시되면 미끄러지듯이 공중에서 움직임
        //그래서 움직임을 완전 멈춤
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        transform.position = new Vector3(player.position.x, player.position.y + 8.0f, 0f);

        yield return new WaitForSeconds(3.0f);

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.AddForce(Vector2.down * 20f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.0f);
        state = State.Move;
    }

    public void SpownMiniSlimeSkill()
    {
        curMiniSlime = maxMiniSlime;

        Vector2 dir = (player.position - transform.position).normalized;
        Vector3 offset = (dir.x < 0f) ? new Vector3(-4f, -1f, 0f) : new Vector3(4f, -1f, 0f);

        for (int i = 0; i < maxMiniSlime; i++)
        {
            Instantiate(miniSlime, transform.position + offset, Quaternion.identity);
            offset.x += 1f;

        }

        Invoke("HpHeal", 10f);

        state = State.Move;
    }
    public void HpHeal()
    {
        // 필드에 남은 미니 슬라임을 죽이고 남은 미니 슬라임에 수에 따라 최대 hp 5%회복
        int healAmount = (int)(maxHp * 0.05f) * curMiniSlime;
        curHp = Mathf.Min(curHp + healAmount, maxHp);
    }

    public void PhaseChange()
    {
        if (nowPhase == "Phase1")
        {
            nowPhase = "Phase2";
            // 푸른 슬라임으로 변환
            spriteRenderer.sprite = phase2;
            jumpInterval = 1.5f; //점프 간격을 감소하여 이속 증가

        }
        else if (nowPhase == "Phase2")
        {
            nowPhase = "Phase3";
            // 빨간 슬라임으로 변환
            spriteRenderer.sprite = phase3;
            attPower *= 2;
            jumpInterval = 1.0f;
        }

        state = State.Move;
    }

    public void Damaged()
    {
        curHp -= 10;// 나중에는 플레이어의 데미지를 가져와 적용
        Vector2 knockbackDir = new Vector2(-(player.position.x - transform.position.x), 0f).normalized;
        _rigidbody.AddForce(knockbackDir * 4f, ForceMode2D.Impulse);
    }

    public void Dead()
    {
        // 죽음 애니메이션 및 문열리는 메서드
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 슬라임과 닿으면 데미지
    }
}
