using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //I need...
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    public void StartRandomMusic() {
        audioSource.clip = audioClips[(int) Random.Range(0f, audioClips.Length)];
        audioSource.Play();
    }
    public void EndMusic() {
        audioSource.Stop();
    }
}
