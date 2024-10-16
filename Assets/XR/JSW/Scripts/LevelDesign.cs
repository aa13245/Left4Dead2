using System.Collections;
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
    GameObject canvas;
    public GameObject[] pingPos;

    // BGM
    public AudioSource concertAudio;
    public AudioClip[] music;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip bgm_horde;
    public AudioClip bgm_arrived;
    public AudioClip bgm_ending;
    public GameObject listener;


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
        canvas = GameObject.Find("PingCanvas").transform.GetChild(0).gameObject;
        StartCoroutine(GameStart());
    }
    IEnumerator GameStart()
    {   // 게임 시작 - 라디오 및 시작 브금 재생, 잠시 후 페이드 아웃
        AudioSource radio = GameObject.Find("RadioSource").GetComponent<AudioSource>();
        Image scriptUI = GameObject.Find("PingCanvas").transform.Find("ScriptUI").GetComponent<Image>();
        Text script = scriptUI.transform.GetChild(0).GetComponent<Text>();
        yield return new WaitForSeconds(2);
        audioSource.Play();
        while (audioSource2.volume > 0)
        {
            audioSource2.volume -= Time.deltaTime * 0.1f;
            yield return null;
        }
        radio.Play();
        scriptUI.transform.gameObject.SetActive(true);
        audioSource2.Stop();
        yield return new WaitForSeconds(10);
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 0.02f;
            scriptUI.color = new Color(scriptUI.color.r, scriptUI.color.g, scriptUI.color.b, scriptUI.color.a - Time.deltaTime * 0.5f);
            script.color = new Color(script.color.r, script.color.g, script.color.b, script.color.a - Time.deltaTime * 1);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 0.3f;
        audioSource2.volume = 0.3f;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && !helicopter.isEnable) StartCoroutine(Helicopter());
        RaidUpdate();
        pingPosUpdate();
    }
    public bool LightOn()
    {
        if (!isLightOn)
        {
            isLightOn = true;
            pingLvl = 2;
            canvas.transform.GetChild(0).GetComponent<Text>().text = "록 콘서트를 시작하여 헬기에 신호를 보내십시오.";
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
            canvas.SetActive(false);
            StartCoroutine(Message());
            StartCoroutine(RaidCoroutine());
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
        yield return new WaitForSeconds(1);
        while (msgObj.GetComponent<RectTransform>().anchoredPosition.y > -185)
        {   // 내려옴
            msgObj.GetComponent<RectTransform>().anchoredPosition += Vector2.down * Time.deltaTime * 1000;
            icon.color += new Color(0, 0, 0, Time.deltaTime * 3);
            mainText.color += new Color(0, 0, 0, Time.deltaTime * 3);
            yield return null;
        }
        yield return new WaitForSeconds(4);
        audioSource.PlayOneShot(bgm_horde);
        yield return new WaitForSeconds(3);
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
    bool heliEnable;
    void RaidUpdate()
    {
        if (!isStarted) return;
        timer += Time.deltaTime;
        if (spawnCooltime > 0) spawnCooltime -= Time.deltaTime;
        if (specialZomCool > 0) specialZomCool -= Time.deltaTime;
        if (timer > 10 && !heliEnable) FireWork(false);
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
        if (timer > 80 && timer < 110)
        {
            SpawnInterval();
            if (timer > 100 && tankCounter == 1)
            {
                tankCounter++;
                Spawn(ZomKind.Tank);
            }
        }
        // 무한 레이드
        if (timer > 150)
        {
            SpawnInterval();
            if (timer > 160 && tankCounter == 2)
            {
                tankCounter++;
                Spawn(ZomKind.Tank);
            }
            if (timer > 170 && !heliEnable){
                heliEnable = true;
                StartCoroutine(Helicopter());
            }
        }
    }
    IEnumerator RaidCoroutine()
    {
        yield return new WaitForSeconds(80);
        audioSource.PlayOneShot(bgm_horde);
        yield return new WaitForSeconds(150);
        audioSource.PlayOneShot(bgm_horde);
    }
    IEnumerator Helicopter()
    {
        while(concertAudio.volume > 0)
        {
            concertAudio.volume -= Time.deltaTime * 0.1f;
            yield return null;
        }
        helicopter.HelicopterEnable();
        audioSource.clip = bgm_arrived;
        audioSource.Play();
    }
    public IEnumerator EndingSound()
    {
        FireWork(true);
        yield return new WaitForSeconds(7);
        audioSource2.PlayOneShot(bgm_ending, 2);
        while(audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 0.08f;
            listener.transform.Translate(Vector3.up * Time.deltaTime * 40);
            yield return null;
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
            spawnCooltime = 3;
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
    // 진행 안내 핑 UI 위치 업데이트
    int pingLvl;
    void pingPosUpdate()
    {
        if (pingLvl == 1) canvas.transform.position = Camera.main.WorldToScreenPoint(pingPos[0].transform.position);
        else if (pingLvl == 2) canvas.transform.position = Camera.main.WorldToScreenPoint(pingPos[1].transform.position);
    }
    // 공연장 입장 시 1번 핑 ON
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player_KJS")) return;
        if (pingLvl == 0)
        {
            pingLvl = 1;
            canvas.SetActive(true);
        }
    }
}
