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

        PlayerInit();
    }

    public void PlayerInit()
    {
        bool hasPlayerObject = FindAnyObjectByType<PlayerHealth>();
        if (hasPlayerObject)
        {
            GameObject playerObj = FindAnyObjectByType<PlayerHealth>().gameObject;
            if (playerObj == null) { }
            else
            {
                if (player == null)
                {
                    player = playerObj.GetComponent<PlayerHealth>();
                }
                playerObj.SetActive(false);
            }
        }
    }

    public void ChangeScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainScene":
                SceneManager.LoadScene("MainScene");
                PlayerInit();
                break;
            case "IntroScene":
                SceneManager.LoadScene("IntroScene");
                break;
            default:
                break;
        }
    }
    
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }

    public void GameOver()
    {
        UIManager.Instance.GameOverUI();
    }
}
