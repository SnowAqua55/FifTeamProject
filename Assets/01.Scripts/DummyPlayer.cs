using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    public Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    [ContextMenu("test")]
    public void Jump()
    {
        rigid.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
    }
}

