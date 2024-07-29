using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerFire_KJS : MonoBehaviour
{
    public GameObject bulletFactory;
    // Start is called before the first frame update
    public GameObject firePosition;
    //미사일을 담을 수 있는 배열
    public float fireRate = 0.1f;

    public GameObject[] bulletArray;
    
    public GameObject bombFactory;

    public float throwPower = 15F;
    //무기를 담을 수 있는 배열
    public GameObject[] weapons;
    //각 무기들에 배당된 무기 번호
    int weaponIndex = 0;

    private float currTime;

    //힐링 아이템 회복량
    public int healAmount = 20;

    //PlayerMove_KJS player;
    private PlayerMove_KJS player;

    void Start()
    {
        player = GetComponent<PlayerMove_KJS>();
        //배열을 이용해서 공간을 확보해라
        bulletArray = new GameObject[40];
        //배열을 채우자
        for (int i = 0; i < 40; i++)
        {
            //총알을 배송받는다.
            GameObject go = Instantiate(bulletFactory);
            //배송받은 총알을 채워 넣는다
            bulletArray[i] = go;
            //스위치를 끈다!(움직이지 않는다.)
            go.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        Fire();
        Swap();
        UseHealingItem();
    }
    void Fire()
    {
        //게임 상태가 '게임 중' 상태일때만 조작할 수 있게한다.
        if (GameManager_KJS.gm.gState != GameManager_KJS.GameState.Run)
        {
            return;
        }
        //유저가 Fire(LMB)을 클릭하면
        if (Input.GetButton("Fire1")&& currTime >= fireRate)
        {
            currTime = 0f;
            // 주무기
            if (weaponIndex == 0)
            {
               
                //비활성화 되어잇는 총알을 찾아라(반복문)
                for (int i = 0; i < 40; i++)
                {
                    //bulletArray[i]번째 총알이 비활성화 되어 있다면
                    if (bulletArray[i].activeSelf == false)
                    {           
                        {
                            //파이어 포지션의 위치에 옮겨놔라
                            bulletArray[i].transform.position = firePosition.transform.position;
                            // bullet의 각도를 카메라 방향으로 바꾸자
                            bulletArray[i].transform.up = Camera.main.transform.forward;
                            //활성화 시켜라(스위치를 켜라)
                            bulletArray[i].SetActive(true);
                            //반복문을 종료해라
                            break;
                        }
                    }

                }
            }
            // 보조무기
            else if (weaponIndex == 1 && Input.GetButtonDown("Fire1"))
            {

                //수류탄 오브젝트를 생성한 후 수류탄의 생성위치를 발사 위치로 한다.
                GameObject bomb = Instantiate(bombFactory);
                bomb.transform.position = firePosition.transform.position;

                //수류탄 오브젝트의 Rigidbody component를 가져온다.
                Rigidbody rb = bomb.GetComponent<Rigidbody>();

                //카메라의 정면 방향으로 수류탄에 물리적인 힘을 가한다.
                rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
            }

        }
    }

    void Swap()
    {
        //각 무기들에 배당된 숫자키
        bool main = Input.GetKeyDown(KeyCode.Alpha1);
        bool sub = Input.GetKeyDown(KeyCode.Alpha2);
        bool bomb = Input.GetKeyDown(KeyCode.Alpha3);
        bool heal = Input.GetKeyDown(KeyCode.Alpha4);
        if (main) weaponIndex = 0;
        if (sub) weaponIndex = 1;
        if (bomb) weaponIndex = 2;
        if (heal) weaponIndex = 3;
    }
    void UseHealingItem()
    {
        if (weaponIndex == 3 && Input.GetButtonDown("Fire1"))
        {
            player.Heal(healAmount);
        }
    }
}