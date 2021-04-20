using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalControlScript : MonoBehaviour
{
    public static GlobalControlScript Instance;
    public Sounds[] sounds;

    private void Awake() {
        if (Instance == null) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    //[SETTINGS]
    public float musicVol, sfxVol;
    public bool mute;

    private void Start() {
        foreach (Sounds s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.isMusic;
        }
        musicVol = 0.2f;
        sfxVol = 0.75f;
        mute = false;
        UpdateAudio(musicVol,sfxVol,mute);
        PlaySound("Sanctuary");
    }
    public void UpdateAudio(float musicVol, float sfxVol, bool mute) {
        this.musicVol = musicVol;
        this.sfxVol = sfxVol;
        this.mute = mute;
        foreach (Sounds s in sounds) {
            if (s.isMusic) s.source.volume = musicVol;
            else s.source.volume = sfxVol;
            s.source.mute = mute;
        }
    }
    public void PlaySound(string name) {
        Sounds s = Array.Find(sounds, s => s.name == name);
        if (!s.source.isPlaying) { 
            if (s.isMusic) StopMusic();
            s.source.Play();
        }
    }

    void StopMusic() {
        foreach(Sounds s in sounds) {
            if (s.isMusic && s.source.isPlaying) s.source.Stop();
        }
        
    }
}
