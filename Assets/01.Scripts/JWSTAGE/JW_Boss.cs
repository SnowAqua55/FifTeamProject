using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JW_Boss : MonoBehaviour
{
   public GameObject player;
   private Animator animator;
   private Rigidbody2D rigid;
   private SpriteRenderer sprite;
   public int hp;
   public int runDistance;
   private void Awake()
   {
      animator = GetComponentInChildren<Animator>();
      rigid = GetComponent<Rigidbody2D>();
      sprite = GetComponentInChildren<SpriteRenderer>();
   }

   private void Update()
   {
      RunAway();
   }

   private void RunAway()
   {
      int lookDir;
      if (sprite.flipX)
      {
         lookDir = -1;
      }
      else
      {
         lookDir = 1;
      }
      RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, lookDir* 10.0f, LayerMask.GetMask("Water"));
      if (hit.collider != null)
      {
         sprite.flipX =!sprite.flipX;
         runDistance = Random.Range(5, 8);
         rigid.velocity = new Vector2(lookDir * -runDistance, rigid.velocity.y);
      }
   }

   public void TakeDamae(int damage)
   {
      hp -= damage;// 혹은 hp가 1씩 깍이도록 설정
      if (hp <= 0)
      {
         animator.SetBool("IsDie" , true);
      }
   }
}
