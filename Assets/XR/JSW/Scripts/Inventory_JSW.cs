//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

public class Inventory_JSW : MonoBehaviour
{
    Human_KJS human;
    // 시작 기본권총
    public GameObject pistolPrefab;

    // 슬롯 4개
    Item_JSW[] slots = new Item_JSW[4];
    public Item_JSW this[int idx] { get{ return slots[idx]; } }
    // 현재 선택된 슬롯
    int slotNum = 1;
    public int SlotNum { get { return slotNum; } }
    public bool SetSlotNum(int value)
    {
        if (slots[value] != null && slotNum != value)
        {
            slotNum = value;
            var item = ItemTable_JSW.instance.itemTable[slots[slotNum].kind];
            if (item is ItemTable_JSW.MainWeapon mainWeapon)
            {
                human.MinRecoil = mainWeapon.minRecoil;
            }
            else if (item is ItemTable_JSW.SubWeapon subWeapon)
            {
                human.MinRecoil = subWeapon.minRecoil;
            }
            else human.MinRecoil = 0;
            ChangeHand();
            return true;
        }
        else return false;
    }
    public GameObject hand;
    GameObject handItem;
    GameObject medikitUI;

    // Start is called before the first frame update
    void Start()
    {
        human = GetComponent<Human_KJS>();
        // 슬롯 초기화
        slots[0] = null;
        if (pistolPrefab != null)
        {
            GameObject pistol = Instantiate(pistolPrefab);
            pistol.SetActive(false);
            slots[1] = pistol.GetComponent<Item_JSW>();
        }
        slots[2] = null;
        slots[3] = null;
        ChangeHand();
        if (!human.isPlayer) medikitUI = GameObject.Find("Canvas").transform.Find("Info" + gameObject.name.Substring(3)).Find("MedikitUI").gameObject;
    }
    // 손 아이템 오브젝트 생성
    void ChangeHand()
    {
        Destroy(handItem);
        if (ItemTable_JSW.instance.itemObjs[(int)slots[slotNum].kind] != null && hand != null)
        {
            handItem = Instantiate(ItemTable_JSW.instance.itemObjs[(int)slots[slotNum].kind]);
            handItem.transform.parent = hand.transform;
            handItem.transform.localPosition = Vector3.zero;
            handItem.transform.localEulerAngles = Vector3.zero;
        }
    }
    // 아이템 줍기
    public bool PickUp(GameObject itemObj)
    {   // 아이템 레이어 체크
        if (itemObj.layer != LayerMask.NameToLayer("Item_JSW")) return false;
        // 아이템 컴포넌트 접근
        Item_JSW itemComp = null;
        Transform currTf = itemObj.transform;
        while (currTf != null)
        {
            itemComp = currTf.GetComponent<Item_JSW>();
            if (itemComp != null) break;
            currTf = currTf.parent;
        }
        if (itemComp == null) return false;

        // 주무기 교체
        int slot = 0;
        object item = ItemTable_JSW.instance.itemTable[itemComp.kind];
        if (item is ItemTable_JSW.MainWeapon)
        {
            slot = 0;
        }
        // 보조무기 교체
        else if (item is ItemTable_JSW.SubWeapon || item is ItemTable_JSW.MeleeWeapon)
        {
            slot = 1;
        }
        // 투척류 교체
        else if (item is ItemTable_JSW.Projectile)
        {   // 현재 소지템과 동일한지 확인
            if (slots[2] == null || slots[2].kind != itemComp.kind)
            {
                slot = 2;
            }
            else return false;
        }
        // 회복템 교체
        else if (item is ItemTable_JSW.Recovery)
        {   // 현재 소지템과 동일한지 확인
            if (slots[3] == null || slots[3].kind != itemComp.kind)
            {
                slot = 3;
            }
            else return false;
        }
        else return false;
        // 교체
        if (slots[slot] != null)
        {   // 기존 아이템 뱉기
            slots[slot].transform.position = transform.position + transform.forward * 0.4f + Vector3.up * 1.6f;
            slots[slot].gameObject.SetActive(true);
        }
        slots[slot] = itemObj.GetComponent<Item_JSW>();
        itemObj.SetActive(false);
        if (slot == 3 && !human.isPlayer) medikitUI.SetActive(true);
        return true;
    }
    public bool Drop(int slot)
    {
        if (slots[slot] != null)
        {
            slots[slot].transform.position = transform.position + transform.forward * 0.4f + Vector3.up * 1.6f;
            slots[slot].gameObject.SetActive(true);
            slots[slot] = null;
            return true;
        }
        return false;
    }
    // 발사 & 사용
    public bool Use(int slot)
    {
        if (slots[slot].value1 > 0)
        {
            slots[slot].value1--;
            // 무기 아닐 경우 다 사용하면 빈 손으로 변경
            if (slots[slot].value1 == 0 && slot >= 2)
            {
                Destroy(slots[slot]);
                slots[slot] = null;
                SetSlotNum(1);
            }
            if (slot == 3 && !human.isPlayer) medikitUI.SetActive(false);
            return true;
        }
        else return false;
    }
    public bool CheckReloadEnable(int slot)
    {   // 주무기
        if (slot == 0)
        {
            if (ItemTable_JSW.instance.itemTable[slots[0].kind] is ItemTable_JSW.MainWeapon item)
            {   // 가득 찼거나 보유 총알이 없을 때
                if (slots[0].value1 == item.magazineCapacity || slots[0].value2 == 0) return false;
                else
                {   
                    return true;
                }
            }
            else return false;
        }
        // 보조무기
        else if (slot == 1)
        {
            if (ItemTable_JSW.instance.itemTable[slots[1].kind] is ItemTable_JSW.SubWeapon item)
            {
                if (slots[1].value1 == item.magazineCapacity) return false;
                else
                {
                    return true;
                }
            }
            else return false;
        }
        else return false;
    }
    public bool Reload(int slot)
    {   // 주무기
        if (slot == 0)
        {
            if (ItemTable_JSW.instance.itemTable[slots[0].kind] is ItemTable_JSW.MainWeapon item)
            {   // 가득 찼거나 보유 총알이 없을 때
                if (slots[0].value1 == item.magazineCapacity || slots[0].value2 == 0) return false;
                else
                {   // 필요한 양
                    int needValue = item.magazineCapacity - slots[0].value1;
                    // 가능한 양
                    int enableValue = Mathf.Min(needValue, slots[0].value2);
                    // 값 변경
                    slots[0].value1 += enableValue;
                    slots[0].value2 -= enableValue;
                    return true;
                }
            }
            else return false;
        }
        // 보조무기
        else if (slot == 1)
        {
            if (ItemTable_JSW.instance.itemTable[slots[1].kind] is ItemTable_JSW.SubWeapon item)
            {   
                if (slots[1].value1 == item.magazineCapacity) return false;
                else
                {
                    slots[1].value1 = item.magazineCapacity;
                    return true;
                }
            }
            else return false;
        }
        else return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
