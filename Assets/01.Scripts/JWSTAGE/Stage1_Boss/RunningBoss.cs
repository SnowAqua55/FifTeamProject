using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class RunningBoss : BossBase
{
    public Transform[] teleportPosition;
    public GameObject attackPrefab;
    private int teleportCount;
    private Coroutine telCoroutine;

    [Header("광폭화 패턴")]
    [SerializeField] private float rageTime;
    [SerializeField] private bool isRage;
    [SerializeField] private GameObject[] rageAttack;
    [SerializeField] private float stageStartTime;
    [SerializeField] private float attackApplyTime;
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < GameManager.Instance.Stage.bossTeleportPosition.Length; i++)
        {
            teleportPosition[i] = GameManager.Instance.Stage.bossTeleportPosition[i];
        }

        isRage = false;
        stageStartTime = 0;
    }

    protected override void Update()
    {
        base.Update();
        stageStartTime += Time.deltaTime;
        if (stageStartTime >= rageTime && isRage != true)
        {
            isRage = true;
        }
    }

    public override void InitStateMachine()
    {
        ChangeState(new IdleState());
    }

    public override void AttackPlayer()
    {
        if (telCoroutine != null) return;
        telCoroutine = StartCoroutine(Teleport());
    }
    
    IEnumerator Teleport()
    {
        float randomRange = Random.Range(-3.0f, 3.0f);
        int ran = Random.Range(0, teleportPosition.Length);
        Vector2 telPosition =
            new Vector2(teleportPosition[ran].position.x + randomRange, teleportPosition[ran].position.y);
        if(currentState.GetType() == new DeadState().GetType()) yield break; 
        Animator.SetTrigger("IsTeleport");
        yield return new WaitForSeconds(1.0f);
        if(currentState.GetType() == new DeadState().GetType()) yield break; 
        transform.position = telPosition;
        teleportCount++;
        if (teleportCount == 3) Attack();
        telCoroutine = null;
    }

    private void Attack()
    {
        if (!isRage)
        {
            teleportCount++;
            if (teleportCount == 3)
            {
                teleportCount = 0;
                Instantiate(attackPrefab, GameManager.Instance.Player.transform.position, Quaternion.Euler(0, 0, 25));
            }
            else return;
        }
        else
        {
            StartCoroutine(RageAttack());
        }
    }

    IEnumerator RageAttack()
    {
        Debug.Log("여긴잘됨");
        int ran = Random.Range(0, rageAttack.Length);
        GameObject attack = Instantiate(rageAttack[ran]);
        attack.gameObject.SetActive(true);
        Collider2D[] col = attack.GetComponentsInChildren<Collider2D>();
        SpriteRenderer[] renderers = attack.GetComponentsInChildren<SpriteRenderer>();
        float timer = 0;
        Color firstColor = new Color(248f / 255f, 92f / 255f, 92f / 255f, 60f / 255f);
        Color lastColor = new Color(248f / 255f, 92f / 255f, 92f / 255f, 150f / 255f);
        while (timer < attackApplyTime)
        {
            timer += Time.deltaTime;
            float t = timer / attackApplyTime;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color = Color.Lerp(firstColor, lastColor, t);   
            }
        }

        for (int i = 0; i < col.Length; i++)
        {
            col[i].enabled = true;
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < col.Length; i++)
        {
            col[i].enabled = false;
            renderers[i].color = firstColor;
        }
    }
}
