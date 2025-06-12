using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject player;
    public GameObject[] boss;
    public GameObject[] doors;
    public Transform playerStartPosition;
    public Transform bossStartPosition;
    public int stageIndex = 0;
    public CinemachineVirtualCamera virtualCamera;
    
    private void Awake()
    {
        GameManager.Instance.Stage = this;
    }

    private void Start()
    { 
        //InitStage(); //위치 변경 예정
    }

    public void InitStage()
    {
        //플레이어 초기 위치 잡기
        virtualCamera.Follow = this.gameObject.transform;
        player.transform.position= playerStartPosition.position;
        Instantiate(boss[stageIndex]); // 해당 스테이지에 맞는 보스 소환
        boss[stageIndex].transform.position = bossStartPosition.position;
        doors[0].SetActive(true); // 닫힌 문 보여주기
        doors[1].SetActive(false); // 열린문 끄기
        // 필요하면 플레이어 체력 맥스로 초기화
    }

    public void OpenDoor() // 보스가 죽었을 때 문열리기
    {
        doors[0].SetActive(false);
        doors[1].SetActive(true);
    }
    
    public void NextStage() //  보스 잡고 포털이나 문에 닿을 시 실행
    {
        stageIndex++;
        if (stageIndex >= boss.Length)//스테이지 갯수 현재는 보스 갯수로 조정
        {
            stageIndex = boss.Length;
            return; // 마지막 스테이지 클리어 UI 출력해도 될 듯
        } 
        // UI FadeOut
        InitStage();
    }
    
}
