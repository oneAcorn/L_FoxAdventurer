using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    //Unity编译器可访问(SerializeField),代码不可访问(private)
    [SerializeField]
    private AudioClip jump1, jump2, fall;

    private void Awake()
    {
        instance = this;
    }

    public void PlayJump1()
    {
        audioSource.clip = jump1;
        audioSource.Play();
    }

    public void PlayJump2()
    {
        audioSource.clip = jump2;
        audioSource.Play();
    }

    public void PlayFall()
    {
        audioSource.clip = fall;
        audioSource.Play();
    }
}
