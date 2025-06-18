using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SetVolume : MonoBehaviour
{
    public BGMPlayer player;
    public AudioMixer audioMixer;
    public Slider BgmSlider;
    private void Awake()
    {
        BgmSlider.onValueChanged.AddListener(SetBgmVolume);
    }

    private void Start()
    {
        audioMixer.SetFloat("bgm", player.volume);
    }

    public void SetBgmVolume(float volume)
    {
        float sound = BgmSlider.value;
        //if(sound)
        if(sound == -40f) audioMixer.SetFloat("bgm", -80);
        audioMixer.SetFloat("bgm", Mathf.Log10(volume)* 20 );
    }
}
