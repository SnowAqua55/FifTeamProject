using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("잘 부딛혔음 ㅇㅇ");
            GameManager.Instance.Stage.NextStage();
        }
        else
        {
            Debug.Log("태그 틀렸대요");
        }
    }
}
