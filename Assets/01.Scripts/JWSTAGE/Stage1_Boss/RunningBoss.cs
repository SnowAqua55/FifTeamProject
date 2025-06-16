using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunningBoss : BossBase
{
    public Transform[] teleportPosition;
    

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
        StartCoroutine(Teleport());

    }
    
    IEnumerator Teleport()
    {
        float randomRange = Random.Range(-3.0f, 3.0f);
        int ran = Random.Range(0, teleportPosition.Length);
        Vector2 telPosition =
            new Vector2(teleportPosition[ran].position.x + randomRange, teleportPosition[ran].position.y);
        Animator.SetTrigger("IsTeleport");
        yield return new WaitForSeconds(1.0f);
        transform.position = telPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerAttack")
        {
            TakeDamage(1);
            ChangeState(new DamagedState());
        }
    }
}
