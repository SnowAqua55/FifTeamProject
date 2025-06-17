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
    

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < GameManager.Instance.Stage.bossTeleportPosition.Length; i++)
        {
            teleportPosition[i] = GameManager.Instance.Stage.bossTeleportPosition[i];
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
        if(currentState.GetType() == new DeadState().GetType()) yield break; //deadstate상태일 때는 안하고싶은데 방법이 없을까 흠...
        Animator.SetTrigger("IsTeleport");
        if(currentState.GetType() == new DeadState().GetType()) yield break;
        yield return new WaitForSeconds(1.0f);
        if(currentState.GetType() == new DeadState().GetType()) yield break; 
        if(currentState.GetType() == new DeadState().GetType()) yield break; 
        transform.position = telPosition;
        teleportCount++;
        if (teleportCount == 3) Attack();
        telCoroutine = null;
    }

    private void Attack()
    {
        teleportCount = 0;
        Instantiate(attackPrefab, GameManager.Instance.Player.transform.position, Quaternion.Euler(0,0,25));
    }
    
}
