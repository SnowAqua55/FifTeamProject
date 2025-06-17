using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private Stage stage;

    public Stage Stage
    {
        get { return stage; }
        set { stage = value; }
    }

    private BossBase boss;

    public BossBase Boss
    {
        get { return boss; }
        set { boss = value; }
    }
    private PlayerHealth player;  //현 플레이어 hp
    public PlayerHealth Player
    {
        get { return player; }
        set { player = value; }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerHealth>();
            if (player != null)
            {
                player = player.GetComponent<PlayerHealth>();
            }
        }
        else
        {
            Debug.Log("Player is Nothing");
        }
    }

    public void GameOver()
    {
        stage.GameOverPanel.SetActive(true);
    }
}
