using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
      public static BGMPlayer instance;
      
      [Header("BGM")]
      public AudioClip[] bgms;
      public AudioSource audioSource;
      public float volume;
      public int curBgmIndex;

      private void Awake()
      {
            instance = this;
            PlayBgm(0);
      }

      private void Start()
      {
            PlayBgm(0);
      }

      public void PlayBgm(int index) // 스테이지별로 bgm이 달라야할까?
      {
            if (audioSource.isPlaying && curBgmIndex != index)
            {
                  audioSource.Stop();
            }
            audioSource.clip = bgms[index];
            audioSource.volume = volume;
            audioSource.loop = true;
            curBgmIndex = index;
            audioSource.Play();
      }
}
