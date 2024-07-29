using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager_JSW : MonoBehaviour
{
    public BotMove botMove;
    public BotSight_JSW botSight;
    public Inventory_JSW inventory;
    // 우선순위 타겟
    GameObject priorityTarget;
    public GameObject PriorityTarget {  get { return priorityTarget; } }
    // 우선순위 타겟 가시여부
    bool targetVisible;
    public bool TargetVisible { get { return targetVisible; } }
    // 파밍할 아이템 타겟
    GameObject farmingTarget;
    // Start is called before the first frame update
    void Start()
    {
        botMove = GetComponent<BotMove>();
        botSight = GetComponent<BotSight_JSW>();
        inventory = GetComponent<Inventory_JSW>();
    }

    // Update is called once per frame
    void Update()
    {
        // 우선 타겟 검사 필요
        /*
            1. 나를 공격
            2. 아군을 공격하는 특수좀비
            3. 특수좀비
            4. 아군 공격 좀비
            5. 좀비
         */
        // 우선 타겟 있을 때
        if (priorityTarget != null)
        {   // 가시여부 체크
            targetVisible = botSight.SightCheck(priorityTarget);
        }
        // 우선 타겟 없을 때
        else
        {
            if (botSight.Target != null)
            {

            }
            // 타겟 없을 때
            else
            {
                /*
                 * 1. 기절 살리기
                 * 2. 회복 - 플레이어, 아군, 본인
                 * 3. 장전
                 * 4. 파밍
                 */
                // 파밍
                // 목표가 있을 때
                if (farmingTarget != null)
                {   // 시아체크
                    if (!botSight.SightCheck(farmingTarget)) farmingTarget = null;
                }
                // 없을 때 탐지
                if (farmingTarget == null)
                {
                    // 주무기가 없을 때
                    if (farmingTarget == null && inventory[0] == null)
                    {   // 주무기 탐지
                        farmingTarget = botSight.ItemDetect(0);
                    }
                    // 주무기가 있는데 잔여 탄약이 최대 소지량 절반 이하일 때
                    else if (farmingTarget == null && inventory[0] != null && ItemTable_JSW.instance.itemTable[inventory[0].kind] is ItemTable_JSW.MainWeapon item && inventory[0].value2 < item.maxAmmoCapacity / 2)
                    {   // 주무기 탐지
                        farmingTarget = botSight.ItemDetect(0);
                    }
                    // 투척류가 없을 때
                    else if (farmingTarget == null && inventory[2] == null)
                    {
                        farmingTarget = botSight.ItemDetect(2);
                    }
                    // 회복템이 없을 때
                    else if (farmingTarget == null && inventory[3] == null)
                    {
                        farmingTarget = botSight.ItemDetect(3);
                    }
                }
                // 목표 아이템으로 이동하기
                if (farmingTarget != null)
                {   // 가까이 왔을 때 아이템 줍기
                    if (Vector3.Distance(gameObject.transform.position, farmingTarget.transform.position) < 1.5f)
                    {

                    }
                    // 멀 때 이동하기
                    else
                    {

                    }
                }
            }
        }
        // 공격
        if (botSight.FireEnable)
        {

        }
    }
}
