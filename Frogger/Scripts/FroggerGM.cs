using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FroggerGM : MonoBehaviour
{

    public int points = 0, time = 60, lives = 7;

    PlayerMovement player;
    UIBehaviour uI;

    bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        uI = GameObject.Find("Canvas").GetComponent<UIBehaviour>();
        player = GameObject.Find("Player").GetComponentInChildren<PlayerMovement>();
        StartTimer();

    }

    // Update is called once per frame

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) uI.Pause();
        uI.InGameText(lives, points, time);

        if (RemainingTime() < 0 || Input.GetKeyDown(KeyCode.F9)) player.PlayerDeath();
        if (lives < 0 && !gameOver) {
            uI.GameOver(points.ToString());
            gameOver = true;
        }
    }

    public void AddPoints(int x) => points += x;

    internal int RemainingTime() {
        return time;

    }

    internal void StopTimer() {
        CancelInvoke("Tick");
        player.hopLock = true;
        ResetTimer();
    }
    internal void StartTimer() {
        InvokeRepeating("Tick", 0, 0.5f);
        player.hopLock = false;
    }

    internal void StartTimer(float x) {
        Invoke("StartTimer", x);
    }


    internal void ResetTimer() => time = 60;


    void Tick() {
        time--;
    }

    public bool isPaused() {
        return uI.pause;
    }

}
