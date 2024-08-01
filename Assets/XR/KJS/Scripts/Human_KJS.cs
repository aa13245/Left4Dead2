using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Human_KJS : MonoBehaviour
{
    public bool isPlayer;
    CharacterController cc;
    public GameObject bulletFactory;
    //총알 효과 주소
    public GameObject bulletEffectFactory;
    // Start is called before the first frame update
    public GameObject firePosition;
    //미사일을 담을 수 있는 배열
    public GameObject[] bulletArray;

    public float fireRate = 0.1f;

    public GameObject bombFactory;

    public float throwPower = 15F;
    //무기를 담을 수 있는 배열
    public GameObject[] weapons;

    private float currTime;

    //플레이어 체력 변수
    float hp = 100;
    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }
    //최대 체력 변수
    public float maxHP = 100;

    //PlayerMove_KJS player;
    public PlayerControler_KJS player;
    // 인벤토리 컴포넌트
    Inventory_JSW inventory;
    // 공격받을 때 슬로우걸리는 함수
    public Action slow;



    public enum HumanState
    {
        Normal,
        KnockedDown,
        Dead,

    }

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (gameObject.name == "Player") isPlayer = true;
        player = GetComponent<PlayerControler_KJS>();
        inventory = GetComponent<Inventory_JSW>();
        hp = maxHP;
        ////배열을 이용해서 공간을 확보해라
        //bulletArray = new GameObject[40];
        ////배열을 채우자
        //for (int i = 0; i < 40; i++)
        //{
        //    //총알을 배송받는다.
        //    GameObject go = Instantiate(bulletFactory);
        //    //배송받은 총알을 채워 넣는다
        //    bulletArray[i] = go;
        //    //스위치를 끈다!(움직이지 않는다.)
        //    go.SetActive(false);
        //}
    }
    // Update is called once per frame
    void Update()
    {
        if (currTime < 100) currTime += Time.deltaTime;
        KnockBackUpdate();
    }
    public void GetDamage(float value, GameObject attacker)
    {
        hp -= value;
        if (isPlayer)
        {
            player.DamageAction();
        }
        if (slow != null) slow();
    }
    public void MouseClick(Vector3 origin = new Vector3(), Vector3 pos = new Vector3())
    {
        //유저가 Fire(LMB)을 클릭하면
        if (inventory[inventory.SlotNum] == null) return;
        // 주무기
        if (inventory.SlotNum == 0)
        {
            MainWeapon(origin, pos);
        }
        // 보조무기
        else if (inventory.SlotNum == 1)
        {
            SubWeapon(origin, pos);
        }
        // 투척
        else if (inventory.SlotNum == 2)
        {
            ThirdSlot();
        }
        // 힐템
        else if (inventory.SlotNum == 3)
        {
            ForthSlot();
        }
    }
    void MainWeapon(Vector3 origin, Vector3 dir)
    {
        #region
        /* 
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
        */
        #endregion
        // 아이템 정보
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.MainWeapon itemInfo)
        {
            if (currTime >= itemInfo.fireRate)
            {
                // 장탄 확인
                if (inventory[inventory.SlotNum].value1 == 0)
                {   // 총알이 없을 때

                }
                else
                {
                    // 발사
                    currTime = 0f;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(isPlayer ? Camera.main.transform.position : origin, 
                                        isPlayer ? Camera.main.transform.forward : dir, out hitInfo, itemInfo.maxRange))
                    {
                        GameObject bullettEffect = Instantiate(bulletEffectFactory);
                        bullettEffect.transform.position = hitInfo.point;
                        bullettEffect.transform.forward = hitInfo.normal;
                        // 데미지 입히기
                        GiveDamage(TopObj(hitInfo.transform.gameObject), itemInfo.baseDmg);
                    }
                    // 장탄 -
                    inventory.Use(inventory.SlotNum);
                }
            }
        }
    }
    void SubWeapon(Vector3 origin, Vector3 dir)
    {
        // 권총이냐
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.SubWeapon itemInfo)
        {
            if (currTime >= itemInfo.fireRate)
            {
                // 장탄 확인
                if (inventory[inventory.SlotNum].value1 == 0)
                {   // 총알이 없을 때

                }
                else
                {
                    currTime = 0f;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(isPlayer ? Camera.main.transform.position : origin,
                                        isPlayer ? Camera.main.transform.forward : dir, out hitInfo, itemInfo.maxRange))
                    {
                        GameObject bullettEffect = Instantiate(bulletEffectFactory);
                        bullettEffect.transform.position = hitInfo.point;
                        bullettEffect.transform.forward = hitInfo.normal;
                        // 데미지 입히기
                        GiveDamage(TopObj(hitInfo.transform.gameObject), itemInfo.baseDmg);
                    }
                    inventory.Use(inventory.SlotNum);
                }
            }
        }
        // 근접무기냐
        else if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.MeleeWeapon itemInfo2)
        {
            
        }
    }
    void ThirdSlot()
    {
        //수류탄 오브젝트를 생성한 후 수류탄의 생성위치를 발사 위치로 한다.
        GameObject bomb = Instantiate(bombFactory);
        bomb.transform.position = firePosition.transform.position;

        //수류탄 오브젝트의 Rigidbody component를 가져온다.
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        //카메라의 정면 방향으로 수류탄에 물리적인 힘을 가한다.
        rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
    }
    void ForthSlot()
    {
        // 회복템 사용
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.Recovery itemInfo)
        {   
            // 회복값보다 체력이 낮을 때
            if (HP < itemInfo.value)
            {
                // 아이템 사용시간 구현 필요
                hp = itemInfo.value;
                inventory.Use(3);

            }
        }
    }
    void GiveDamage(GameObject target, float dmg)
    {
        // 좀비 공격
        if (target.layer == LayerMask.NameToLayer("Enemy"))
        {
            target.GetComponent<JKYEnemyFSM>().HitEnemy(dmg);
        }
        // 아군 공격
        else
        {

        }
    }
    public void PickUp(GameObject item = null)
    {
        // 플레이어 일 때
        if (item == null)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 2))
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Item_JSW"))
                {
                    inventory.PickUp(hitInfo.transform.gameObject);
                }
            }
        }
        // 봇일 때
        else
        {
            inventory.PickUp(item);
        }
    }
    public void Drop()
    {
        inventory.Drop(inventory.SlotNum);
    }
    public bool Reload()
    {
        // 장전 시간 구현 필요
        return inventory.Reload(inventory.SlotNum);
    }
    GameObject TopObj(GameObject obj)
    {
        GameObject topObj = obj;
        while (topObj.transform.parent != null && topObj.transform.parent.gameObject.layer == obj.layer)
        {
            topObj = topObj.transform.parent.gameObject;
        }
        return topObj;
    }
    public Action stun;
    public void Stun(GameObject stone)
    {
        stun();
    }
    float knockBackPow = 30;
    public Vector3 knockBackVector = Vector3.zero;
    public void ApplyKnockBack(GameObject zombie)
    {
        Vector3 dir = transform.position - zombie.transform.position + Vector3.up * 0.7f;
        dir.Normalize();
        knockBackVector = dir * knockBackPow;
    }
    void KnockBackUpdate()
    {
        knockBackVector -= knockBackVector * 1 * Time.deltaTime;
    }

}