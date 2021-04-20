using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour {
    GameObject uIInGame, uIGameOver, uISettings, uIPause;
    GlobalControlScript gcs;

    TextMeshProUGUI goScoreText,pointsScore;

    public bool pause = false;

    private void Start() {
        gcs = GameObject.Find("Global Control").GetComponent<GlobalControlScript>();
        Transform[] menus = this.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in menus) {
            GameObject g = t.gameObject;
            if (g.CompareTag("UI_ingame")) uIInGame = g;
            if (g.CompareTag("UI_gameover")) uIGameOver = g;
            if (g.CompareTag("UI_settings")) uISettings = g;
            if (g.CompareTag("UI_pause")) uIPause = g;
        }

        uISettings.SetActive(true);
        GameObject.Find("MusicSlider").GetComponent<Slider>().SetValueWithoutNotify(gcs.musicVol);
        GameObject.Find("SFXSlider").GetComponent<Slider>().SetValueWithoutNotify(gcs.sfxVol);
        GameObject.Find("MuteToggle").GetComponent<Toggle>().SetIsOnWithoutNotify(gcs.mute);
        uISettings.SetActive(false);

        uIGameOver.SetActive(true);
        goScoreText = GameObject.Find("GOScoreText").GetComponent<TextMeshProUGUI>();
        uIGameOver.SetActive(false);


        if (uIGameOver != null) uIGameOver.SetActive(false);

    }

    internal void InGameText(int lives, int points, int time) {
        if (!uIInGame.activeSelf) return;

        pointsScore = GameObject.Find("Points").GetComponent<TextMeshProUGUI>(); 
        string pointsString = points.ToString();
        while (pointsString.Length < 10) pointsString = "0" + pointsString;
        pointsScore.text = "points: " + pointsString;

        GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>().text = "x" + lives;

        GameObject.Find("Timer").GetComponent<Image>().fillAmount = time / 60f;
    }

    public void Snake() {
        gcs.PlaySound("Candy");
        SceneManager.LoadScene("Snake", LoadSceneMode.Single);
        Time.timeScale = 1;

    }
    public void Frogger() {
        gcs.PlaySound("DeepBlue");
        SceneManager.LoadScene("Frogger", LoadSceneMode.Single); 
        Time.timeScale = 1;

    }
    public void Minesweeper() {
        gcs.PlaySound("Modern");
        gcs.PlaySound("WindowsStartup");
        SceneManager.LoadScene("Minesweeper", LoadSceneMode.Single);
        Time.timeScale = 1;
    }

    public void MainMenu() {
        gcs.PlaySound("Sanctuary");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Time.timeScale = 1;
    }


    public void GameOver(string scoreValue) {
        uIInGame.SetActive(false);
        uIGameOver.SetActive(true);
        goScoreText.text= scoreValue;

    }
    public void SettingsReel() {
        uISettings.SetActive(!uISettings.activeInHierarchy);
    }

    public void UpdateAudio() {
        float musicVol = GameObject.Find("MusicSlider").GetComponent<Slider>().value;
        float sfxVol = GameObject.Find("SFXSlider").GetComponent<Slider>().value;
        bool mute = GameObject.Find("MuteToggle").GetComponent<Toggle>().isOn;
        gcs.UpdateAudio(musicVol, sfxVol, mute);
    }

    public void Unpause (){
        GameObject.Find("Game Manager").GetComponent<GameManager>().Unpause(); 
        Time.timeScale = 1;
    }
    public void Pause() {
        uISettings.SetActive(false);
        pause = !pause;
        Time.timeScale = pause ? 0 : 1;
        uIPause.SetActive(pause);
    }
}
