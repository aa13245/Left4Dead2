using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_KJS : MonoBehaviour
{
    // 싱글턴 변수
    public static GameManager_KJS gm;
    // 게임 상태 UI 오브젝트 변수
    public GameObject gameLabel;
    //패널 배열
    public GameObject[] panels;
    private int currentPanelIndex = -1;

    PlayerControler_KJS human;
    // 게임 상태 UI 텍스트 컴포넌트 변수
    Text gameText;

    // Start is called before the first frame update

    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }
    // 게임 상태 상수
    public enum GameState
    {
        Ready,
        Run,
        GameOver
    }

    //현재의 게임 상태 변수
    public GameState gState;

    PlayerControler_KJS player;
    void Start()
    {
        //플레이어 오브젝트를 찾은 후 플레이어의 PlayerMove 컴포넌트 받아오기.
        player = GameObject.Find("Player").GetComponent<PlayerControler_KJS>();
        //초기 게임 상태는 준비 상태로 설정한다.
        gState = GameState.Ready;
        //게임 상태 UI오브젝트에서 Text 컴포넌트를 가져온다.
        gameText = gameLabel.GetComponent<Text>();
        //상태 텍스트의 내용을 'Ready...'로 한다.
        gameText.text = "Ready...";
        //상태 텍스트의 색상을 빨간색으로 한다.
        gameText.color = new Color32(255, 0, 0, 255);
        //게임 준비 > 게임 중 상태로 전환하기
        StartCoroutine(ReadyToStart());
        // 시작시 모든 패널을 비 활성화
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
    IEnumerator ReadyToStart()
    {
        //2초간 대기한다.
        yield return new WaitForSeconds(2f);

        //상태 텍스트의 내용을 'Go!'로 한다.
        gameText.text = "Go!";

        //0.5초간 대기한다.
        yield return new WaitForSeconds(0.5f);

        //상태 텍스트를 비활성화 한다.
        gameLabel.SetActive(false);

        //상태를 '게임 중' 상태로 변경한다.
        gState = GameState.Run;
    }

    // Update is called once per frame
    void Update()
    {
        //for(int i=0; i)
        //만일, 플레이어의 hp가 0이하라면...
        if (player != null && player.human.HP <= 0)
        {
            //상태 텍스트를 활성화 한다.
            gameLabel.SetActive(true);
            //상태 텍스트의 내용을 'Game Over'로 한다.
            gameText.text = "Game Over";
            //상태 텍스트의 색상을 붉은색으로 한다.
            gameText.color = new Color32(255, 0, 0, 255);
            //상태를 '게임 오버' 상태로 변경한다.
            gState = GameState.GameOver;
        }
    }
}
