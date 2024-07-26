using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory_JSW : MonoBehaviour
{
    // 주무기 구조체
    public struct Slot
    {
        public ItemTable_JSW.Items item;
        public int value1;
        public int value2;
        public Slot(ItemTable_JSW.Items item, int value1, int value2)
        {
            this.item = item;
            this.value1 = value1;
            this.value2 = value2;
        }
    }
    // 슬롯 4개
    Slot[] slots = new Slot[4];
    public Slot this[int idx] { get{ return slots[idx]; } }

    // Start is called before the first frame update
    void Start()
    {   // 슬롯 초기화
        slots[0] = new Slot(ItemTable_JSW.Items.none, 0, 0);
        slots[1] = new Slot(ItemTable_JSW.Items.pistol, 15, 0);
        slots[2] = new Slot(ItemTable_JSW.Items.none, 0, 0);
        slots[3] = new Slot(ItemTable_JSW.Items.none, 0, 0);
    }

    // 아이템 줍기
    public bool PickUp(ItemTable_JSW.Items itemEnum, int value1 = 1, int value2 = 0)
    {
        // 주무기 교체
        object item = ItemTable_JSW.instance.itemTable[itemEnum];
        if (item is ItemTable_JSW.MainWeapon)
        {
            slots[0] = new Slot(itemEnum, value1, value2);
            return true;
        }
        // 보조무기 교체
        else if (item is ItemTable_JSW.SubWeapon || item is ItemTable_JSW.MeleeWeapon)
        {
            slots[1] = new Slot(itemEnum, value1, value2);
            return true;
        }
        // 투척류 교체
        else if (item is ItemTable_JSW.Projectile)
        {   // 현재 소지템과 동일한지 확인
            if (slots[2].item != itemEnum)
            {
                slots[2] = new Slot(itemEnum, value1, value2);
                return true;
            }
            else return false;
        }
        // 회복템 교체
        else if (item is ItemTable_JSW.Recovery)
        {   // 현재 소지템과 동일한지 확인
            if (slots[3].item != itemEnum)
            {
                slots[3] = new Slot(itemEnum, value1, value2);
                return true;
            }
            else return false;
        }
        else return false;
    }
    // 발사 & 사용
    public bool Use(int slot)
    {
        if (slots[slot].value1 > 0)
        {
            slots[slot].value1--;
            // 무기 아닐 경우 다 사용하면 빈 손으로 변경
            if (slots[slot].value1 == 0 && slot >= 2) slots[slot] = new Slot(ItemTable_JSW.Items.none, 0, 0);
            return true;
        }
        else return false;
    }
    public bool Reload(int slot)
    {   // 주무기
        if (slot == 0)
        {
            if (ItemTable_JSW.instance.itemTable[slots[0].item] is ItemTable_JSW.MainWeapon item)
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
            if (ItemTable_JSW.instance.itemTable[slots[1].item] is ItemTable_JSW.SubWeapon item)
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
