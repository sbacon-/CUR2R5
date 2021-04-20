using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindowsUI : MonoBehaviour
{
    BoardManager bm;
    GlobalControlScript gcs;
    public Sprite[] digits,smiley;

    float cWidth = 8, cHeight = 8, cMines = 10;
    int gen = 1;

    int rB = 0, rI=0, rX=0;

    public int timer = 0;



    // Start is called before the first frame update
    void Start() {

        bm = GameObject.Find("BoardManager").GetComponent<BoardManager>();
        gcs = GameObject.Find("Global Control").GetComponent<GlobalControlScript>();

        Transform[] transforms = this.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in transforms) {
            if(t.name == "MusicSlider")t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(gcs.musicVol);
            if(t.name == "SFXSlider")t.gameObject.GetComponent<Slider>().SetValueWithoutNotify(gcs.sfxVol);
            if(t.name == "MuteToggle")t.gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(gcs.mute);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DateTime date = DateTime.Now;
        string dateText = string.Format("{0:t}", date);
        GameObject.Find("SysTime").GetComponent<TextMeshProUGUI>().text = dateText;
    }

    public void Beginner() {
        bm.GenerateBoard(8, 8, 10);
        StartMenu();
        gen = 0;
    }
    public void Intermediate() {
        bm.GenerateBoard(16, 16, 40); 
        StartMenu();
        gen = 1;
    }
    public void Expert() {
        bm.GenerateBoard(30, 16, 99); 
        StartMenu();
        gen = 2;
    }
    public void GenCustom() {
        bm.GenerateBoard((int)cWidth, (int)cHeight, (int)cMines);
        StartMenu();
        gen = 3;
    }

    public void Regen() {
        switch (gen) {
            case 0:
                Beginner();
                break;
            case 1:
                Intermediate();
                break;
            case 2:
                Expert();
                break;
            case 3:
                GenCustom();
                break;
        }
    }
    public void RegenCancelStart() {
        switch (gen) {
            case 0:
                Beginner();
                break;
            case 1:
                Intermediate();
                break;
            case 2:
                Expert();
                break;
            case 3:
                GenCustom();
                break;
        }
        StartMenu();
    }


    public void Custom() {
        SettingsHide();
        Transform[] transforms = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        Transform custom = Array.Find(transforms, t => t.CompareTag("UI_custommenu"));
        SideMenu(true);
        custom.gameObject.SetActive(true);
    }
    public void CustomHide() {
        Transform[] transforms = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        Transform custom = Array.Find(transforms, t => t.CompareTag("UI_custommenu"));
        SideMenu(false);
        custom.gameObject.SetActive(false);
    }
    public void StartMenu() {
        Transform[] transforms = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        Transform pause = Array.Find(transforms, t => t.CompareTag("UI_pause"));
        pause.gameObject.SetActive(!pause.gameObject.activeSelf);
        if (pause.gameObject.activeSelf) RefreshHighScore();
        CustomHide();
        SettingsHide();
        SideMenu(false);
    }


    public void Settings() {
        CustomHide();
        Transform[] transforms = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        Transform settings = Array.Find(transforms, t => t.CompareTag("UI_settings"));
        SideMenu(true);
        settings.gameObject.SetActive(true);
    }
    public void SettingsHide() {

        Transform[] transforms = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        Transform settings = Array.Find(transforms, t => t.CompareTag("UI_settings"));
        SideMenu(false);
        settings.gameObject.SetActive(false);
    }
    public void SideMenu(bool set) {
        Transform[] transforms = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        Transform sideMenu = Array.Find(transforms, t => t.CompareTag("UI_sidemenu"));
        sideMenu.gameObject.SetActive(set);
    }
    public void Quit() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void UpdateAudio() {
        float musicVol = GameObject.Find("MusicSlider").GetComponent<Slider>().value;
        float sfxVol = GameObject.Find("SFXSlider").GetComponent<Slider>().value;
        bool mute = GameObject.Find("MuteToggle").GetComponent<Toggle>().isOn;
        gcs.UpdateAudio(musicVol, sfxVol, mute);
    }

    public void UpdateCustomLevel() {
        cWidth = GameObject.Find("WidthSlider").GetComponent<Slider>().value;
        cHeight = GameObject.Find("HeightSlider").GetComponent<Slider>().value;

        GameObject.Find("MinesSlider").GetComponent<Slider>().maxValue = (cWidth * cHeight) / 2;

        cMines = GameObject.Find("MinesSlider").GetComponent<Slider>().value;

        GameObject.Find("WidthText").GetComponent<TextMeshProUGUI>().text = "width: " + cWidth;
        GameObject.Find("HeightText").GetComponent<TextMeshProUGUI>().text = "height: " + cHeight;
        GameObject.Find("MinesText").GetComponent<TextMeshProUGUI>().text = "mines: " + cMines;


    }

    public void ToggleAutoGrav() {
        bm.autoClick = GameObject.Find("AutoClick").GetComponent<Toggle>().isOn;
        bm.gravity = GameObject.Find("Gravity").GetComponent<Toggle>().isOn;

        bm.UpdateGravity();
    }

    internal void UpdateTimer() {
        timer++;
        string timerstring = timer.ToString();
        if (timerstring.Length > 3) {
            TimerText("---");
            return;
        }
        while (timerstring.Length < 3) timerstring = "0" + timerstring;
        TimerText(timerstring);
    }
    private void TimerText(string v) {
        Image t1 = GameObject.Find("T1").GetComponent<Image>();
        Image t2 = GameObject.Find("T2").GetComponent<Image>();
        Image t3 = GameObject.Find("T3").GetComponent<Image>();
        if (v == "---") {
            t1.sprite = digits[10];
            t2.sprite = digits[10];
            t3.sprite = digits[10];
        } else {
            t1.sprite = digits[Int32.Parse(v.Substring(0, 1))];
            t2.sprite = digits[Int32.Parse(v.Substring(1, 1))];
            t3.sprite = digits[Int32.Parse(v.Substring(2, 1))];
        }
    }

    public void StartTimer() {
        InvokeRepeating("UpdateTimer", 0, 1);
    }
    public void StopTimer() {
        CancelInvoke("UpdateTimer");
    }
    public void ResetTimer() {
        StopTimer();
        timer = 0;
        StartTimer();
    }
    public void MinesText(int x) {
        string v = x.ToString();
        while (v.Length < 3) v = "0" + v;
        Image t1 = GameObject.Find("MC1").GetComponent<Image>();
        Image t2 = GameObject.Find("MC2").GetComponent<Image>();
        Image t3 = GameObject.Find("MC3").GetComponent<Image>();
        if (v == "---" || x<0) {
            t1.sprite = digits[10];
            t2.sprite = digits[10];
            t3.sprite = digits[10];
        } else {
            t1.sprite = digits[Int32.Parse(v.Substring(0, 1))];
            t2.sprite = digits[Int32.Parse(v.Substring(1, 1))];
            t3.sprite = digits[Int32.Parse(v.Substring(2, 1))];
        }
    }

    public void SmileySprite(int x) {
        Image smile = GameObject.Find("Smiley").GetComponent<Image>();
        smile.sprite = smiley[x];
    }
    
    public void LogScore() {
        switch (gen) {
            case 0:
                if (timer < rB || rB ==0 ) rB = timer;
                break;
            case 1:
                if (timer < rI || rI ==0 ) rI = timer;
                break;
            case 2:
                if (timer < rX || rX ==0 ) rX = timer;
                break;
        }
    }
    private void RefreshHighScore() {
        string b = rB == 0 ? "---" : rB+"";
        string i = rI == 0 ? "---" : rI+"";
        string x = rX == 0 ? "---" : rX+"";


        GameObject.Find("rB").GetComponent<TextMeshProUGUI>().text = b;
        GameObject.Find("rI").GetComponent<TextMeshProUGUI>().text = i;
        GameObject.Find("rX").GetComponent<TextMeshProUGUI>().text = x;
    }

}
