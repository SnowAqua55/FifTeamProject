using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    public AudioClip jumpSfx, landSfx, dashSfx, hurtSfx, attackSfx, deathSfx;
    
    [Header("SFX 볼륨 스케일")]
    [Tooltip("Dash SFX 볼륨 스케일")]
    [Range(0f,1f)] public float dashVolume = 0.3f;
    
    AudioSource audioSource;

    void Awake() => audioSource = GetComponent<AudioSource>();

    public void PlayJump()  => audioSource.PlayOneShot(jumpSfx);
    public void PlayLand()  => audioSource.PlayOneShot(landSfx);
    public void PlayDash()  => audioSource.PlayOneShot(dashSfx, dashVolume);
    public void PlayHurt()  => audioSource.PlayOneShot(hurtSfx);
    public void PlayAttack()  => audioSource.PlayOneShot(attackSfx);
    public void PlayDeath() => audioSource.PlayOneShot(deathSfx);
}
