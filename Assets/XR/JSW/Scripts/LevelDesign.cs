using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

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
    public VisualEffect[] fireWorks;
    bool isStarted;

    public Helicopter_JSW helicopter;
    public Transform botDest;
    GameObject ping;
    public GameObject[] pingPos;

    // BGM
    public AudioSource concertAudio;
    public AudioClip[] music;
    AudioSource audioSource;
    public AudioClip bgm_horde;


    private void Awake()
    {
        transform.DetachChildren();
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        ping = GameObject.Find("PingCanvas").transform.GetChild(0).gameObject;
        //StartCoroutine(Message());
    }

    // Update is called once per frame
    void Update()
    {
        RaidUpdate();
        pingPosUpdate();
    }
    public bool LightOn()
    {
        if (!isLightOn)
        {
            isLightOn = true;
            pingLvl = 2;
            ping.transform.GetChild(0).GetComponent<Text>().text = "록 콘서트를 시작하여 헬기에 신호를 보내십시오.";
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
            pingLvl = 3;
            ping.SetActive(false);
            StartCoroutine(Message());
            // 음악, 폭죽
            FireWork(true);
            RaidStart();
            return true;
        }
        return false;
    }
    public GameObject msgObj;
    public Text icon;
    public Text mainText;
    IEnumerator Message()
    {
        concertAudio.clip = music[0];
        concertAudio.Play();
        msgObj.SetActive(true);
        while (msgObj.GetComponent<RectTransform>().anchoredPosition.y > -185)
        {   // 내려옴
            msgObj.GetComponent<RectTransform>().anchoredPosition += Vector2.down * Time.deltaTime * 1000;
            icon.color += new Color(0, 0, 0, Time.deltaTime * 3);
            mainText.color += new Color(0, 0, 0, Time.deltaTime * 3);
            yield return null;
        }
        yield return new WaitForSeconds(5);
        audioSource.PlayOneShot(bgm_horde, 0.5f);
        yield return new WaitForSeconds(2);
        // 사라짐
        while (icon.color.a > 0)
        {
            icon.color -= new Color(0, 0, 0, Time.deltaTime * 3);
            mainText.color -= new Color(0, 0, 0, Time.deltaTime * 3);
            yield return null;
        }
        msgObj.SetActive(false);
    }
    bool isFireWorkOn;
    void FireWork(bool on)
    {
        if (isFireWorkOn == on) return;
        isFireWorkOn = on;
        for (int i = 0; i < fireWorks.Length; i++)
        {
            fireWorks[i].enabled = on;
        }
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
        if (timer > 10) FireWork(false);
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
            spawnCooltime = 5;
        }
        if (specialZomCool <= 0)
        {   // 특수 좀비
            Spawn(ZomKind.Special);
            specialZomCool = 20;
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
    int pingLvl;
    void pingPosUpdate()
    {
        if (pingLvl == 1) ping.transform.position = Camera.main.WorldToScreenPoint(pingPos[0].transform.position);
        else if (pingLvl == 2) ping.transform.position = Camera.main.WorldToScreenPoint(pingPos[1].transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player_KJS")) return;
        if (pingLvl == 0)
        {
            pingLvl = 1;
            ping.SetActive(true);
        }
    }
}
