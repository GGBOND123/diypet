using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour {

    // playIntro is set to PlayOnAwake
    public AudioSource playIntro;
    // These will both only be started through this script
    public AudioSource playOutro;
    public AudioSource loopMusic;

    private bool loopingMusic = false;
    private bool playOutroMusic = false;

    void Start() {
        loopMusic.loop = true;
    }

    void Update() {
        if (!playIntro.isPlaying && !loopingMusic) {
            loopingMusic = true;
            loopMusic.Play();
        }

        if (!loopMusic.isPlaying && playOutroMusic) {
            playOutroMusic = false;
            playOutro.Play();
        }

    }

    public void PlayOutro() {
        playOutroMusic = true;
        loopMusic.loop = false;
    }
}
