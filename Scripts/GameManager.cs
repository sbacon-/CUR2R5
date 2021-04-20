using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject applePrefab, gridLinePrefab;
    public int gridSize, score = 0;

    GameObject scoreUI, scoreAdd;
    TextMeshProUGUI scoreAddTMP, countDownTMP;
    Vector3 scoreAddPos;
    byte scoreAddAlpha = 0;
    byte countdown = 3;
    internal bool gameStart=false;
    

    // Start is called before the first frame update
    void Start()
    {
       
        gridSize = (int) GameObject.Find("Background").transform.localScale.x * 5;
        GenerateApple();
        SetupGridLine(gridSize);
        
        scoreUI = GameObject.Find("ScoreValue");
        scoreAdd = GameObject.Find("ScoreAdd");
        scoreAddPos = scoreAdd.transform.position;
        scoreAddTMP = scoreAdd.GetComponent<TextMeshProUGUI>();
        countDownTMP = GameObject.Find("CountDown").GetComponent<TextMeshProUGUI>();
        InvokeRepeating("AdvanceCountdown", 0f, 1f);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.E)||Input.GetKeyDown(KeyCode.Pause)){Pause();}
        string scoreText = "" + ((score-3) * 100);
        
        scoreUI.GetComponent<TextMeshProUGUI>().text = scoreText;

        if (scoreAddAlpha != 0) {
            scoreAddAlpha--;
            scoreAddTMP.color = new Color32(0, 0, 0, scoreAddAlpha);
            scoreAdd.transform.position += Vector3.down*Time.deltaTime*10f;
        }
    }
    void GenerateApple() {
        var position = new Vector3(UnityEngine.Random.Range(-gridSize+1, gridSize), 0, UnityEngine.Random.Range(-gridSize+1, gridSize));
        Instantiate(applePrefab, position, Quaternion.identity);
    }
    public void AddSegment() {
        GameObject lastSegment = GameObject.Find("Segment"+score);
        score++;
        GameObject newSegment = Instantiate(lastSegment);
        lastSegment.GetComponent<SegmentBehaviour>().tailSegment = newSegment;
        newSegment.name = "Segment" + score;
        newSegment.transform.parent = lastSegment.transform.parent;
        SegmentBehaviour sb = newSegment.GetComponent<SegmentBehaviour>();
        sb.id = score;
        sb.tailSegment = null;

        GenerateApple();

        scoreAdd.transform.position = scoreAddPos;
        scoreAddAlpha = 255;
    }

    void Pause() {
        GameObject canvas = GameObject.Find("Canvas");
        Transform[] menus = canvas.GetComponentsInChildren<Transform>(true);
        Transform pausemenu = Array.Find(menus, m => m.name == "PauseMenu");
        pausemenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    internal void Unpause() {
        GameObject.Find("PauseMenu").SetActive(false);
        if (GameObject.Find("SettingsMenu") != null) GameObject.Find("SettingsMenu").SetActive(false);
        Time.timeScale = 1;
    }

    void AdvanceCountdown() {
        countDownTMP.text = ""+countdown;
        if (countDownTMP.text == "0"){
            countDownTMP.text = "GO!";
            CancelInvoke("AdvanceCountdown");
            Invoke("FadeCountdown",1f);
        } else {
            countdown--;
        }
    }

    void FadeCountdown() {
        countDownTMP.gameObject.SetActive(false);
    }

    public void GameOver() {
        GameObject.Find("Canvas").GetComponent<UIBehaviour>().GameOver(scoreUI.GetComponent<TextMeshProUGUI>().text);
    }

    void SetupGridLine(int dimension) {
        GameObject gridManager = new GameObject("GridManager");
        for(int i = -dimension; i<dimension; i++) {
            Vector3 vertical = new Vector3((float)i+0.5f, 0.01f, 0);
            Vector3 horizontal = new Vector3(0, 0.01f, (float)i+0.5f);
            GameObject horizontalLine = Instantiate(gridLinePrefab, horizontal, Quaternion.identity);
            GameObject verticalLine = Instantiate(gridLinePrefab, vertical, Quaternion.Euler(0, 90, 0));
            horizontalLine.transform.parent = gridManager.transform;
            verticalLine.transform.parent = gridManager.transform;
        }   
    }


}
