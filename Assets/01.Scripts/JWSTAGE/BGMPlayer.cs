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

      private void Awake()
      {
            instance = this;
      }
}
