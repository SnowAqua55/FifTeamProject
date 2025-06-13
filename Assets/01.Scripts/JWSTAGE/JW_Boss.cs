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
   public float runDistance;
   public float checkWallDistance;
   public Transform[] teleportPosition;
   public LayerMask targetLayer;
   
   private void Awake()
   {
      animator = GetComponentInChildren<Animator>();
      rigid = GetComponent<Rigidbody2D>();
      sprite = GetComponentInChildren<SpriteRenderer>();
      player = GameObject.FindGameObjectWithTag("Player");
      LookPlayer();
      for (int i = 0; i < GameManager.Instance.Stage.bossTeleportPosition.Length; i++)
      {
         teleportPosition[i] = GameManager.Instance.Stage.bossTeleportPosition[i];
      }
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

      RaycastHit2D hit =
         Physics2D.Raycast(transform.position, Vector2.right, lookDir * 5.0f, targetLayer);
      if (hit.collider != null)
      {
         if (hit.collider.tag == "Player")
         {
            sprite.flipX = !sprite.flipX;
            runDistance = Random.Range(5, 8);
            rigid.velocity = new Vector2(lookDir * -runDistance, rigid.velocity.y);
            Invoke("LookPlayer", 1.0f);
         }
         else if (hit.collider.tag == "Respawn") // 추후 태그 변경
         {
            Teleport();
            rigid.velocity = Vector2.zero;
            Invoke("LookPlayer", 1.0f);
         }
      }
      else return;
   }

   public void TakeDamae(int damage)
   {
      hp -= damage; // 혹은 hp가 1씩 깍이도록 설정
      if (hp <= 0)
      {
         StartCoroutine(BossDie());
      }
      else
      {
         animator.SetTrigger("IsDamage");
      }
      LookPlayer();
   }

   IEnumerator BossDie()
   {
      animator.SetBool("IsDie", true);
      hp = 0;
      yield return new WaitForSeconds(2.0f);
      Destroy(gameObject);
      GameManager.Instance.Stage.OpenDoor();
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.tag == "Finish")
      {
         TakeDamae(1);
      }
   }

   private void LookPlayer()
   {
      if (player.transform.position.x > transform.position.x)
      {
         sprite.flipX = false;
      }
      else if (player.transform.position.x < transform.position.x)
      {
         sprite.flipX = true;
      }
   }

   private void Teleport()
   {
      float randomRange = Random.Range(-3.0f, 3.0f);
      int ran = Random.Range(0, teleportPosition.Length);
      animator.SetTrigger("IsTeleport");
      teleportPosition[ran].position += new Vector3(randomRange, 0,0);
      gameObject.transform.position = teleportPosition[ran].position;
   }
}
