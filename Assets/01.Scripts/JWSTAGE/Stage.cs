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
    public CinemachineVirtualCamera bossVirtualCamera;
    public GameObject curBoss;
    
    public Transform[] bossTeleportPosition; // 도망가는 보스 전용
    public GameObject GameOverPanel;
    
    private void Awake()
    {
        GameManager.Instance.Stage = this;
        player = GameManager.Instance.Player.gameObject;
    }
    
    public IEnumerator InitStage()
    {
        //플레이어 초기 위치 잡기
        UIManager.Instance.FadeFlashStart();
        yield return new WaitForSeconds(3.0f);
        BGMPlayer.instance.PlayBgm(1);
        bossVirtualCamera.gameObject.SetActive(true);
        virtualCamera.Follow = this.gameObject.transform;
        player.transform.position= playerStartPosition.position;
        Instantiate(boss[stageIndex]); // 해당 스테이지에 맞는 보스 소환
        GameManager.Instance.Boss = boss[stageIndex].GetComponent<BossBase>();
        boss[stageIndex].transform.position = bossStartPosition.position;
        curBoss = boss[stageIndex];
        SpawnBossCamera();
        doors[0].SetActive(true); // 닫힌 문 보여주기
        doors[1].SetActive(false); // 열린문 끄기
        //BGMPlayer.instance.PlayBgm(stageIndex); 스테이지 별로 배경음이 다를떄
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
            GameManager.Instance.GameOver();
            return; // 마지막 스테이지 클리어 UI 출력해도 될 듯
        } 
        // UI FadeOut
        StartCoroutine(InitStage());
    }

    public void SpawnBossCamera()
    {
        if (curBoss == null)
        {
            return;
        }

        StartCoroutine("SetBossCamera");
    }

    IEnumerator SetBossCamera()
    {
        bossVirtualCamera.Follow = curBoss.transform;
        bossVirtualCamera.Priority = 12;
        yield return new WaitForSeconds(3.0f);
        bossVirtualCamera.Priority = 0;
    }
}
