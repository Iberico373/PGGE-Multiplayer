using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> clipList= new List<AudioClip>();
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound11()
    {
        audioSource.clip = clipList[0];
        audioSource.Play();
    }

    public void PlaySound15()
    {
        audioSource.clip = clipList[1];
        audioSource.Play();
    }
}
