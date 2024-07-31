using JetBrains.Annotations;
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

    PlayerControler_KJS human;
    // 게임 상태 UI 텍스트 컴포넌트 변수
    Text gameText;
    // 총알 정보를 표시할 텍스트 UI 요소
    public Text currentAmmoText;
    public Text totalAmmoText;

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

        // 게임 시작 시 기본 패널을 활성화
        if (panels.Length > 0)
        {
            panels[1].SetActive(true); // 예를 들어, 첫 번째 패널을 기본으로 활성화
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
        if (gState != GameState.Run)
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                if (IsSlotNotEmpty(i)) // 슬롯이 비어 있지 않은 경우에만 변경
                {
                    ChangePlayerSlot(i); // 숫자키에 따라 플레이어의 무기 슬롯 변경
                    SwitchPanel(i); // UI 패널 변경
                }
            }
        }

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
        // 총알 정보 업데이트
        //UpdateAmmoUI();
    }
    public void SwitchPanel(int index)
    {
        if (index < 0 || index >= panels.Length)
        {
            Debug.LogError("Invalid panel index.");
            return;
        }

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i]?.SetActive(i == index);
        }
    }

    private void ChangePlayerSlot(int slotIndex)
    {
        if (player == null) return;

        player.inventory.SlotNum = slotIndex;
        // 직접적으로 UI를 갱신할 메서드를 호출하는 것으로 변경
        // player.SendMessage("SlotUIChange");
        player.SlotUIChange(); // 직접 호출하는 방법을 사용
    }
    private bool IsSlotNotEmpty(int slotIndex)
    {
        return player.inventory[slotIndex] != null; // 슬롯이 비어있지 않으면 true 반환
    }
    private void UpdateAmmoUI()
    {
        if (player == null || player.inventory == null) return;

        // 주무기 슬롯의 총알 정보 업데이트
        if (player.inventory[0] != null && ItemTable_JSW.instance.itemTable[player.inventory[0].kind] is ItemTable_JSW.MainWeapon mainWeapon)
        {
            currentAmmoText.text = $"Ammo: {player.inventory[0].value1}/{mainWeapon.magazineCapacity}";
            totalAmmoText.text = $"Total Ammo: {player.inventory[0].value2}";
        }
        else
        {
            currentAmmoText.text = "Ammo: N/A";
            totalAmmoText.text = "Total Ammo: N/A";
        }
    }
}