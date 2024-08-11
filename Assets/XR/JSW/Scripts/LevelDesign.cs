using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesign : MonoBehaviour
{
    GameObject zomStartSpawnPoints;
    GameObject specialZomStartSpawnPoints;
    GameObject raidSpawnPoints;

    public GameObject[] startSpawnZombie;
    public GameObject[] raidZombie;
    public GameObject[] specialZombies;
    public GameObject tank;

    bool isLightOn;
    public GameObject lights;
    bool isMusicOn;
    public GameObject stage;
    bool isStarted;

    public Helicopter_JSW helicopter;
    public Transform botDest;

    private void Awake()
    {
        transform.DetachChildren();
    }
    // Start is called before the first frame update
    void Start()
    {
        helicopter = GameObject.Find("Helicopter").GetComponent<Helicopter_JSW>();
        botDest = GameObject.Find("BotDest").transform;
        // 시작 좀비 스폰
        zomStartSpawnPoints = GameObject.Find("ZombieStartSpawnPoints");
        for (int i = 0; i < zomStartSpawnPoints.transform.childCount; i++)
        {
            Instantiate(startSpawnZombie[Random.Range(0, startSpawnZombie.Length)], zomStartSpawnPoints.transform.GetChild(i).transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), null);
        }
        // 시작 특수좀비 스폰
        specialZomStartSpawnPoints = GameObject.Find("SpecialZomStartSpawnPoints");
        //for (int i = 0;i < specialZomStartSpawnPoints.transform.childCount; i++)
        //{
        //    Instantiate(specialZombies[Random.Range(0, specialZombies.Length)], zomStartSpawnPoints.transform.GetChild(i).transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), null);
        //}
        // 레이드 스폰 위치
        raidSpawnPoints = GameObject.Find("RaidSpawnPoints");

        if (lights != null) lights.SetActive(false);
        if (stage != null) stage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaidUpdate();
    }
    public bool LightOn()
    {
        if (!isLightOn)
        {
            isLightOn = true;
            if (lights != null) lights.SetActive (true);
            return true;
        }
        return false;
    }
    public bool MusicOn()
    {
        if (!isMusicOn && isLightOn)
        {
            isMusicOn = true;
            if (stage != null)
            {
                stage.SetActive (true);
                // 음악, 폭죽
            }
            RaidStart();
            return true;
        }
        return false;
    }
    // 레이드 시작
    void RaidStart()
    {
        isStarted = true;
    }
    float timer;
    float spawnCooltime;
    float specialZomCool = 10;
    int tankCounter;
    void RaidUpdate()
    {
        if (!isStarted) return;
        timer += Time.deltaTime;
        if (spawnCooltime > 0) spawnCooltime -= Time.deltaTime;
        if (specialZomCool > 0) specialZomCool -= Time.deltaTime;

        // 레이드 1
        if (timer > 10 && timer < 40)
        {
            SpawnInterval();
            if (timer > 30 && tankCounter == 0)
            {
                tankCounter++;
                Spawn(ZomKind.Tank);
            }
        }

        // 레이드 2
        if (timer > 100 && timer < 130)
        {
            SpawnInterval();
            if (timer > 120 && tankCounter == 1)
            {
                tankCounter++;
                Spawn(ZomKind.Tank);
            }
        }

        // 무한 레이드
        if (timer > 190)
        {
            SpawnInterval();
            if (timer > 200 && tankCounter == 2)
            {
                tankCounter++;
                Spawn(ZomKind.Tank);
            }
            if (timer > 210 && !helicopter.isEnable){
                helicopter.HelicopterEnable();
            }
        }

    }

    enum ZomKind
    {
        Normal,
        Special,
        Tank
    }
    void SpawnInterval()
    {
        if (spawnCooltime <= 0)
        {   // 일반 좀비
            Spawn(ZomKind.Normal);
            spawnCooltime = 4;
        }
        if (specialZomCool <= 0)
        {   // 특수 좀비
            Spawn(ZomKind.Special);
            specialZomCool = 10;
        }
    }
    void Spawn(ZomKind kind)
    {
        if (kind == ZomKind.Normal)
        {
            for (int i = 0; i < raidSpawnPoints.transform.childCount; i++)
            {
                Instantiate(raidZombie[Random.Range(0, raidZombie.Length)], raidSpawnPoints.transform.GetChild(i).transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), null);
            }
        }
        else if (kind == ZomKind.Special)
        {
            Instantiate(specialZombies[Random.Range(0, specialZombies.Length)], raidSpawnPoints.transform.GetChild(Random.Range(0, raidSpawnPoints.transform.childCount)).transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), null);
        }
        else
        {
            Instantiate(tank, raidSpawnPoints.transform.GetChild(Random.Range(0, raidSpawnPoints.transform.childCount)).transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), null);
        }
        
    }
}
