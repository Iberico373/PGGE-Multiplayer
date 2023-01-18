using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitObject : MonoBehaviour, IDamageable
{
  public enum Type
  {
    CONCRETE,
    METAL,
    WOOD,
    OTHERS,
  }
  [SerializeField] 
  AudioSource audioSource;
  [SerializeField]
  List<AudioClip> audioClips = new List<AudioClip>();

  [SerializeField]
  AudioClip defaultAudioClip;

  [SerializeField]
  Type type = Type.OTHERS;

  void IDamageable.TakeDamage()
  {
    AudioClip clip = (int)type < audioClips.Count ? audioClips[(int)type] : defaultAudioClip;
    audioSource.PlayOneShot(clip);
    Debug.Log("Take damage - " + gameObject.name);
  }
}
