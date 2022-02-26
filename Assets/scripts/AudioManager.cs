using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    //I need...
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private int currentClipIndex;
    [SerializeField] private AudioClip currentClip;

    private void Awake() {
        muteToggle = gameManager.muteButton;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.M)) { muteToggle.isOn = !muteToggle.isOn; }
        ChangeMute(muteToggle.isOn);

        if (Input.GetKeyUp(KeyCode.Greater) ||
            Input.GetKeyUp(KeyCode.Period)) {

            currentClipIndex = currentClipIndex < audioClips.Length -1
                ? currentClipIndex += 1
                : 0;

            audioSource.clip = currentClip = audioClips[currentClipIndex];
                

            audioSource.Play();
        }

        if (Input.GetKeyUp(KeyCode.Less) ||
            Input.GetKeyUp(KeyCode.Comma)) {

            currentClipIndex = currentClipIndex > 0
                ? currentClipIndex -= 1
                : audioClips.Length - 1;

            audioSource.clip = currentClip = audioClips[currentClipIndex];

            audioSource.Play();
        }

        if (CheckClipEnded()) StartRandomMusic();
    }
    public void StartRandomMusic() {
        audioSource.clip = currentClip = audioClips[currentClipIndex = (int) Random.Range(0f, audioClips.Length)];
        audioSource.Play();
    }

    public bool CheckClipEnded() {
        return audioSource.time >= currentClip.length;
    }

    public void EndMusic() {
        audioSource.Stop();
    }

    public void ChangeMute(bool toggleIsOn) {
        audioSource.mute = toggleIsOn ? true : false;
    }
}
