using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBossAttack : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1); 
        }
    }
}
