using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotManager_JSW : MonoBehaviour
{
    public BotMove botMove;
    public BotSight_JSW botSight;
    public Inventory_JSW inventory;
    public Human_KJS human;

    Slider hpSlider;
    Image hpImage;
    Slider tempHpSlider;
    Image tempHpImage;
    // 우선순위 타겟
    GameObject priorityTarget;
    public GameObject PriorityTarget {  get { return priorityTarget; } }
    // 우선순위 타겟 가시여부
    bool targetVisible;
    public bool TargetVisible { get { return targetVisible; } }
    // 파밍할 아이템 타겟
    public GameObject farmingTarget;
    // Start is called before the first frame update
    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        hpSlider = canvas.transform.Find("Info"+ gameObject.name[3].ToString() +"/HPbar").GetComponent<Slider>();
        hpImage = canvas.transform.Find("Info" + gameObject.name[3].ToString() + "/HPbar/Fill Area/Fill").GetComponent<Image>();
        tempHpSlider = canvas.transform.Find("Info"+ gameObject.name[3].ToString() +"/TempHPbar").GetComponent<Slider>();
        tempHpImage = canvas.transform.Find("Info"+ gameObject.name[3].ToString() +"/TempHPbar/Fill Area/Fill").GetComponent<Image>();
        botMove = GetComponent<BotMove>();
        botSight = GetComponent<BotSight_JSW>();
        inventory = GetComponent<Inventory_JSW>();
        human = GetComponent<Human_KJS>();
    }

    // Update is called once per frame
    void Update()
    {
        HpUiUpdate();
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
            targetVisible = botSight.SightCheck(priorityTarget, -1, true);

            // 장전
            if ((inventory.SlotNum == 0 || inventory.SlotNum == 1) && inventory[inventory.SlotNum].value1 == 0)
            {
                human.Reload(true);
                return;
            }
        }
        // 우선 타겟 없을 때
        else
        {   // 타겟 있을 때
            if (botSight.Target != null)
            {
                // 장전
                if ((inventory.SlotNum == 0 || inventory.SlotNum == 1) && inventory[inventory.SlotNum].value1 == 0)
                {
                    human.Reload(true);
                    return;
                }
            }
            // 타겟 없을 때
            else
            {
                /*
                  1. 기절 살리기
                  2. 회복 - 플레이어, 아군, 본인
                  3. 장전
                  4. 파밍
                 */
                // 장전
                if ((inventory.SlotNum == 0 || inventory.SlotNum == 1) && inventory[inventory.SlotNum].value1 == 0)
                {
                    human.Reload(true);
                    return;
                }
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
                    if (Vector3.Distance(transform.position, farmingTarget.transform.position) < 1.5f)
                    {
                        botMove.ChangeBotMoveState(BotMove.BotMoveState.Idle);
                        human.PickUp(farmingTarget);
                    }
                    // 멀 때 이동하기
                    else
                    {
                        botMove.ChangeBotMoveState(BotMove.BotMoveState.Farming);
                    }
                }
            }
        }
        // 공격
        if (botSight.FireEnable)
        {
            GameObject target = null;
            if (priorityTarget != null) target = priorityTarget;
            else if (botSight.Target != null) target = botSight.Target;
            if (target == null) return;
            Vector3 origin = transform.position + transform.forward + Vector3.up * (human.humanState == Human_KJS.HumanState.KnockedDown ? 0.8f : 1.6f);
            Vector3 dir = target.transform.position + Vector3.up - origin;
            human.MouseClick(origin, dir);
        }
    }
    void HpUiUpdate()
    {
        hpSlider.value = (human.HP - human.TempHP) / (human.humanState == Human_KJS.HumanState.KnockedDown ? human.knockedDownMaxHP : human.maxHP);
        tempHpSlider.value = human.HP / (human.humanState == Human_KJS.HumanState.KnockedDown ? human.knockedDownMaxHP : human.maxHP);
        if (human.humanState == Human_KJS.HumanState.Normal)
        {
            Color c = Color.HSVToRGB(Mathf.Lerp(0, 0.3392157f, tempHpSlider.value), 1, 1);
            c.a = 1;
            hpImage.color = c;
            c.a = 0.5f;
            tempHpImage.color = c;
        }
        else if (human.humanState == Human_KJS.HumanState.KnockedDown)
        {
            Color c = Color.HSVToRGB(0, 1, 1);
            c.a = 1;
            hpImage.color = c;
            c.a = 0.5f;
            tempHpImage.color = c;
        }
    }
}
