using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBossAttack : MonoBehaviour
{
    private void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                other.GetComponent<PlayerHealth>().TakeDamage(1);
            }
            else return;
        }
    }
}
