using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int[,] board;
    public int[] mines;
    public Sprite[] squares;
    public GameObject squarePrefab;
    public GlobalControlScript gcs;
    public WindowsUI wUI;

    int boardWidth, boardHeight, mineCountX;
    float scale;

    public float blastRadius, blastForce;

    bool started = false;
    bool gameOver = false;
    bool win = false;
    public bool autoClick = false, gravity = false;

    int revealCount = 0, flagCount = 0;



    // Start is called before the first frame update
    void Start()
    {
        gcs = GameObject.Find("Global Control").GetComponent<GlobalControlScript>();
        wUI = GameObject.Find("Canvas").GetComponent<WindowsUI>();
        GenerateBoard(16,16,40);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F10)) {
            ExplodeBoard(Vector3.zero,blastRadius, blastForce);
        }
        if (revealCount+mineCountX == boardHeight * boardWidth && !gameOver && !win) {
            wUI.StopTimer();
            wUI.SmileySprite(1);
            wUI.LogScore();
            win = true;
            gcs.PlaySound("Sweep");
        }

        wUI.MinesText(mineCountX-flagCount);
    }

    public void GenerateBoard(int width, int height, int mineCount) {

        ClearBoard();

        if (width <= 10 && height <= 10) scale = 5f;
        else  scale = 1.5f;

        boardWidth = width;
        boardHeight = height;
        mineCountX = mineCount;

        float xoffset = (-0.16f * width * scale)/2 + (0.16f*scale/2);
        float yoffset = (-0.16f * height * scale)/2 + (0.16f * scale / 2) + (0.16f*8);

        mines = new int[mineCount];
        for (int m = 0; m < mineCount; m++) {
            bool complete = false;
            while (!complete) {
                int mineCoord = UnityEngine.Random.Range(0, width * height);
                if (ArrayContains(mineCoord)) continue;
                else {
                    mines[m] = mineCoord;
                    complete = true;
                }
            }
        }
        int index = 0;
        for (int y = 0; y<height; y++) {
            for(int x =0; x<width; x++) {
                GameObject square = Instantiate(squarePrefab);
                square.transform.parent = transform;

                square.transform.position = new Vector3(
                    xoffset + (index % width) * 0.16f * scale,
                    yoffset + ((int)(index / width)) * 0.16f * scale, 0); ;

                square.transform.localScale = Vector3.one * scale;

                SquareBehaviour sb = square.GetComponent<SquareBehaviour>();

                sb.meta = DetermineSquareMeta(index);
                sb.name = "SQ" + index ;
                sb.index = index;
                sb.DetermineNeighbors(width,height);
                
                index++;
            }
        }
    }

    private void ClearBoard() {
        started = false;
        gameOver = false;
        win = false;
        
        Transform[] transfroms = GetComponentsInChildren<Transform>();
        foreach(Transform t in transfroms) {
            if (t == transform) continue;
            t.name = "NULL";
            Destroy(t.gameObject);
        }
        wUI.SmileySprite(0);
        wUI.ResetTimer();
        revealCount = 0;
        flagCount = 0;
        print(transfroms.Length);

    }

    bool ArrayContains(int x) {
        int[] array = mines;
        for (int index = 0; index < array.Length; index++) {
            if (array[index] == x) return true;
        }
        return false;
    }
    int DetermineSquareMeta(int index) {
        int adjacent = 0;
        bool left, right, top, bottom;
        left = index % boardWidth == 0;
        right = (index+1) % boardWidth == 0;
        bottom = (index >= 0) && (index < boardWidth);
        top = index >= boardWidth * (boardHeight - 1);
        if (ArrayContains(index)) return 9;
        if (!bottom) {
            if (ArrayContains(index - boardWidth + 1) && !right) adjacent++;
            if (ArrayContains(index - boardWidth)) adjacent++;
            if (ArrayContains(index - boardWidth - 1) && !left) adjacent++;
        }
        if (ArrayContains(index - 1)&&!left) adjacent++;
        if (ArrayContains(index + 1)&&!right) adjacent++;
        if (!top){ 
            if (ArrayContains(index + boardWidth - 1)&&!left) adjacent++;
            if (ArrayContains(index + boardWidth)) adjacent++;
            if (ArrayContains(index + boardWidth + 1)&&!right) adjacent++;
        }
        return adjacent;
    }
    internal void SwapTile(SquareBehaviour squareBehaviour,int index) {
        squareBehaviour.GetComponent<SpriteRenderer>().sprite = squares[index];
    }
    internal void RevealTile(SquareBehaviour squareBehaviour) {
        if (!started) {
            TextMeshProUGUI loadText = GameObject.Find("Loading").GetComponent<TextMeshProUGUI>();
            loadText.text = "Loading";
            squareBehaviour = HandleReshuffle(squareBehaviour.index);
            started = true;
            loadText.text = "";
        }
        if (squareBehaviour.revealed || squareBehaviour.flagged) return;
        squareBehaviour.revealed = true;
        squareBehaviour.GetComponent<SpriteRenderer>().sprite = squares[squareBehaviour.meta];
        if (squareBehaviour.meta == 0) RevealNeighbors(squareBehaviour.neighbors);
        if (squareBehaviour.meta == 9) {
            ExplodeBoard(squareBehaviour.transform.position, blastRadius, blastForce);
            squareBehaviour.GetComponent<ParticleSystem>().Play();
        }
        revealCount++;
    }


    internal void RevealStack(SquareBehaviour sb) {
        if (CountAdjacentFlags(sb.neighbors, sb.meta)) RevealNeighbors(sb.neighbors);
    }

    private bool CountAdjacentFlags(int[] neighbors,int meta) {
        int count = 0;
        foreach (int i in neighbors) {
            if (i == -1) continue;
            else {
                SquareBehaviour sb = GameObject.Find("SQ" + i).GetComponent<SquareBehaviour>();
                if (sb.flagged) count++;
            }
        }
        return count == meta;
    }
    internal void RevealNeighbors(int[] neighbors) {
        foreach (int i in neighbors) {
            if (i == -1) continue;
            else {
                SquareBehaviour sb = GameObject.Find("SQ" + i).GetComponent<SquareBehaviour>();
                RevealTile(sb);
            }
        }
    }
    internal void FlagTile(SquareBehaviour sb) {
        if (sb.flagged) {
            SwapTile(sb,(int) Squares.Square);
            flagCount--;
        } else {
            SwapTile(sb, (int)Squares.Flag);
            flagCount++;
        }
        sb.flagged = !sb.flagged;
    }

    enum Squares {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, MineBoom, Mine, Flag, MineNot, Square, Question
    }

    SquareBehaviour HandleReshuffle(int index) {

        int tries = 0;
        SquareBehaviour sb=null;
        while (tries < 15) {
            GenerateBoard(boardWidth, boardHeight, mineCountX);
            sb = GameObject.Find("SQ" + index).GetComponent<SquareBehaviour>();
            if (sb.meta == 0)return sb;
            tries++;
        }
        return sb;
    }


    void ExplodeBoard(Vector3 point,float radius, float force) {
        if(gameOver == false) {
            foreach(int m in mines) {
                SquareBehaviour sb = GameObject.Find("SQ" + m).GetComponent<SquareBehaviour>();
                if (!sb.revealed) SwapTile(sb,(int) Squares.Question);
                sb.flagged = false;
                gameOver = true;
                wUI.SmileySprite(2);
            }
        }
        Rigidbody2D[] rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D r in rigidbodies) {
            r.isKinematic = false;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, 0.5f);
        foreach(Collider2D c in colliders) {
            Vector3 direction = c.transform.position - point;
            Rigidbody2D r = c.GetComponent<Rigidbody2D>();
            if (r == null) continue;
            r.AddForce(direction * force);
        }
        UpdateGravity();
        gcs.PlaySound("Boom");

    }
    internal void UpdateGravity() {
        Transform[] transfroms = GetComponentsInChildren<Transform>();
        foreach (Transform t in transfroms) {
            if (t == transform) continue;
            float grav = gravity ? 1 : 0;
            t.GetComponent<Rigidbody2D>().gravityScale = grav;
        }
    }
}
