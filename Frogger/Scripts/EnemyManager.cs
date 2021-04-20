using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour {
    public Sprite levels;
    public GameObject[] enemies, homes, spawns, otterSpawns;
    GameObject enemyContainer;
    FroggerGM fGM;

    public int level, lands, spawnCount, otterSpawnCount;
    public float baseSpeed;

    void Start() {
        enemyContainer = new GameObject("Enemies");
        enemyContainer.transform.parent = this.transform;
        GenerateLevel(0);
        InvokeRepeating("RollHome", 0, 10);
        InvokeRepeating("RollSpawns", 5, 15);
        fGM = GameObject.Find("GameManager").GetComponent<FroggerGM>();
    }
    private void Update() {
        if (lands == 5 || Input.GetKeyDown(KeyCode.F12)) {
            Animator[] anims = Array.FindAll(enemyContainer.GetComponentsInChildren<Animator>(), a => a.transform.position.y == 12);
            foreach (Animator a in anims) a.SetTrigger("Total");
            GameObject.Find("Global Control").GetComponent<GlobalControlScript>().PlaySound("Win");
            GameObject.Find("Player").GetComponent<PlayerMovement>().hopLock = true;
            fGM.StopTimer();
            Invoke("Win", 3f) ;
            lands = 0;
        }
        if (Input.GetKeyDown(KeyCode.F10)) RollSpawns();
    }

    void GenerateLevel(int level) {
        homes = new GameObject[5];
        spawns = new GameObject[17];
        otterSpawns = new GameObject[8];
        spawnCount = 0;
        otterSpawnCount = 0;
        while(level>10)level -= 10;
            for(int y=0; y<=12; y++) {
                for(int x=0; x<16; x++) {
                    switch (y) {
                    case 1:
                        baseSpeed = 1.5f;
                        if (ReadLevel(level, x, y) == Color.red) Spawn((int)Enemies.Car, x, y);
                        break;
                    case 2:
                        baseSpeed = 0.75f;
                        if (ReadLevel(level, x, y) == Color.green) Spawn((int)Enemies.SlowCar, x, y);
                        break;
                    case 3:
                        baseSpeed = 1.5f;
                        if (ReadLevel(level, x, y) == Color.blue) Spawn((int)Enemies.Car2, x, y);
                        break;
                    case 4:
                        baseSpeed = 3.0f;
                        if (ReadLevel(level, x, y) != Color.white) Spawn((int)Enemies.FastCar,x,y);
                        break;
                    case 5:
                        baseSpeed = 1.5f;
                        if (ReadLevel(level, x, y) == Color.cyan) Spawn((int)Enemies.BigCar1, x, y);
                        if (ReadLevel(level, x, y) == Color.blue) Spawn((int)Enemies.BigCar2, x, y);
                        break;
                    //MEDIAN
                    case 7:
                        baseSpeed = 1.5f;
                        if (ReadLevel(level, x, y) == Color.red) Spawn((int)Enemies.Turtle, x, y);
                        if (ReadLevel(level, x, y) == Color.green) Spawn((int)Enemies.Turtle_Sink, x, y);
                        break;
                    case 8:
                        baseSpeed = 1.5f;
                        if (ReadLevel(level, x, y) == Color.red) otterSpawns[otterSpawnCount++] = Spawn((int)Enemies.Log1, x, y);
                        if (ReadLevel(level, x, y) == Color.blue)spawns[spawnCount++]=Spawn((int)Enemies.Log2, x, y);
                        if (ReadLevel(level, x, y) == Color.green) Spawn((int)Enemies.Log3, x, y);
                        break;
                    case 9:
                        baseSpeed = 4.0f;
                        if (ReadLevel(level, x, y) == Color.red) otterSpawns[otterSpawnCount++] = Spawn((int)Enemies.Log1, x, y);
                        if (ReadLevel(level, x, y) == Color.blue) spawns[spawnCount++] = Spawn((int)Enemies.Log2, x, y);
                        if (ReadLevel(level, x, y) == Color.green) Spawn((int)Enemies.Log3, x, y);
                        break;

                    case 10:
                        baseSpeed = 1.75f;
                        if (ReadLevel(level, x, y) == Color.red) Spawn((int)Enemies.Turtle, x, y);
                        if (ReadLevel(level, x, y) == Color.green) Spawn((int)Enemies.Turtle_Sink, x, y);
                        break;

                    case 11:
                        baseSpeed = 1.5f;
                        if (ReadLevel(level, x, y) == Color.red) otterSpawns[otterSpawnCount++] = Spawn((int)Enemies.Log1, x, y);
                        if (ReadLevel(level, x, y) == Color.blue) spawns[spawnCount++] = Spawn((int)Enemies.Log2, x, y);
                        if (ReadLevel(level, x, y) == Color.green) Spawn((int)Enemies.Log3, x, y);
                        if (ReadLevel(level, x, y) == Color.magenta) Spawn((int)Enemies.Alligator1, x, y);
                        if (ReadLevel(level, x, y) == Color.cyan) Spawn((int)Enemies.Alligator2, x, y);
                        if (ReadLevel(level, x, y) == Color.yellow) Spawn((int)Enemies.Alligator3, x, y);
                        break;
                    case 12:
                        baseSpeed = 1.5f;
                        if (x%3==0 && x!=15)homes[x/3]=Spawn((int)Enemies.Home, x+0.5f, y);
                        break;
                    }
                }
            }
    }
    Color ReadLevel(int level, int x, int y) {
        return levels.texture.GetPixel(x, (Mathf.Abs(level-9) * 12)+y);
    }

    GameObject Spawn(int index, float x, float y) {
        GameObject enemy = Instantiate(enemies[index], new Vector3(x, y, 0), Quaternion.identity, enemyContainer.transform);
        EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();
        if (((y <= 8) && y % 2 != 0) || y == 10) eb.direction = Vector3.left;
        else eb.direction = Vector3.right;


        //SET SPEEDS
        float multipier = (level + 1) * .1f;
        if ((level + 1) % 5 == 0) {
            multipier = 1;
        }
        eb.speed = baseSpeed * (1 + multipier);

        if (y == 12) eb.speed = 0;

        return enemy;
    }
    GameObject Spawn(int index, EnemyBehaviour ebA, bool moves, float offsetX) {
        GameObject enemy = Instantiate(enemies[index], new Vector3(ebA.transform.position.x+offsetX, ebA.transform.position.y, 0), Quaternion.identity, enemyContainer.transform);
        EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();

        eb.speed = ebA.speed;
        eb.direction = ebA.direction;
        if (moves) {
            eb.direction = ebA.direction * -1;
        }

        eb.SelfDestruct(5);

        return enemy;
    }

    public enum Enemies {
        Car, 
        SlowCar, 
        Car2, 
        FastCar, 
        BigCar1, BigCar2, 
        Turtle, Turtle_Sink, 
        Log1, Log2, Log3, 
        Alligator1, Alligator2, Alligator3, AlligatorX, 
        Otter, Snake, Bonus,
        Home
    }

    public void Win() {
        fGM.AddPoints(fGM.RemainingTime()*10);
        fGM.AddPoints(1000);
        fGM.ResetTimer();
        RemoveAllEnemies();
        level++;
        GenerateLevel(level);
        fGM.StartTimer();

    }

    void RemoveAllEnemies() {
        Destroy(enemyContainer);
        enemyContainer = new GameObject("Enemies");
        enemyContainer.transform.parent = this.transform;
    }
    void RollHome() {
        EnemyBehaviour eb = homes[Random.Range(0, 5)].GetComponent<EnemyBehaviour>();
        switch (Random.Range(0, 3)) {
            case 1:
                eb.HomeFly();
                break;
            case 2:
                eb.HomeAlligator();
                break;
        }
    }
    void RollSpawns() {
        EnemyBehaviour eb = spawns[Random.Range(0, spawnCount)].GetComponent<EnemyBehaviour>();
        switch (Random.Range(0, 4)) {
            case 1:
                Spawn((int)Enemies.Bonus,eb,false,0);
                break;
            case 2:
                eb = otterSpawns[Random.Range(0, otterSpawnCount)].GetComponent<EnemyBehaviour>();
                Spawn((int)Enemies.Otter,eb,true,-1);
                break;
            case 3:
                Spawn((int)Enemies.Snake, eb, true,0);
                break;
        }
    }

}
