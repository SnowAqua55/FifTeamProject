using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SlimeBall : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] Collider2D slimeBall;
    [SerializeField] Collider2D kingSlime;
    [SerializeField] KingSlime ks;

    [SerializeField] SpriteRenderer _sP;
    [SerializeField] Sprite p2ball;
    [SerializeField] Sprite p3ball;

    public float speed = 3.0f;
    private bool isStarted = false;
    private bool isAttacked = false;
    private Vector2 dir;


    private void Awake()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        player = _player.GetComponent<Transform>();

        slimeBall = GetComponent<Collider2D>();

        GameObject _kingSlime = GameObject.Find("KingSlime");
        kingSlime = _kingSlime.GetComponent<Collider2D>();
        ks = _kingSlime.GetComponent<KingSlime>();

        _sP = GetComponent<SpriteRenderer>();
        
        Physics2D.IgnoreCollision(slimeBall, kingSlime);

    }

    private void Update()
    {
        if(ks.nowPhase == "Phase2")
        {
            _sP.sprite = p2ball;
        }
        else if(ks.nowPhase == "Phase3")
        {
            _sP.sprite = p3ball;

        }

        AttackPlayer();
        
    }

    public void AttackPlayer()
    {
        if (!isStarted)
        {
            dir = (player.position - transform.position).normalized;
            isStarted = true;
        }
        transform.position +=  (Vector3) (dir * speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacked) return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
        isAttacked = true;

        //플레이어 데미지 메서드 호출
        
        Destroy(gameObject);
    }
}
