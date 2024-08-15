using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //enum, 열거형
    public enum EsoundType
    {
        //EFT_BULLET = 109
        EFT_BULLET,
        EFT_DESTROY,
        EFT_DAMAGE
    }
    public enum EBgmType

    {
        //EFT_BULLET = 109
        BGM_TITLE,
        BGM_INGAME,
        BGM_RESULT
    }
    // SoundManager Prefab
    public GameObject soundManagerFactory;
    // 나를 담을 static 변수s
    public static SoundManager instance;
    public static SoundManager Get()
    {
        //만약에 instance가 null 이라면
        if (instance == null)
        {
            // soundmanager Prefab을 읽어오자
            GameObject soundManagerFactory = Resources.Load<GameObject>("SoundManager");
            //SoundManager 공장에서 SoundManager을 만들자
            GameObject soundManager = Instantiate(soundManagerFactory);
            //// Instance에 SoundManager 컴포넌트를 셋팅하자.
            //instance = soundManager.GetComponent<SoundManager>();
        }
        return instance;
    }


    // audiosource
    public AudioSource eftAudio;
    public AudioSource bgmAudio;

    // effect audio clip을 여러개 담아 놓을 변수
    public AudioClip[] eftAudios;
    public AudioClip[] bgmAudios;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 씬 전환이 돼도 게임오브젝트를 파괴하고 싶지않다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //audiosource 가져오자
        eftAudio = GetComponent<AudioSource>();
        bgmAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // effect Sound Play 하는 함수
    public void PlayEftSound(EsoundType idx)
    {
        int audioIdx = (int)idx;
        eftAudio.PlayOneShot(eftAudios[audioIdx]);
    }

    public void PlayBGMSound(EBgmType idx)
    {
        int bgmIdx = (int)idx;
        //플레이할 AudioClip을 설정
        bgmAudio.clip = bgmAudios[bgmIdx];
        //플레이!
        bgmAudio.Play();
    }
    public void AudioSourceEtc()
    {
        //일시정지
        bgmAudio.Pause();
        //완전멈춤
        bgmAudio.Stop();
        // 현재 실행되고있는 시간
        float currTime = bgmAudio.time;

        bgmAudio.time += 10;
    }
}
