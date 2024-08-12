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
    Human_KJS[] humans = new Human_KJS[4];
    int myIdx;
    public string botName;

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
    // 도움줄 타겟
    public GameObject approchingTarget;
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
        humans[0] = GameObject.Find("Player").GetComponent<Human_KJS>();
        for (int i = 1; i < 4; i++)
        {
            string name = "Bot" + i;
            humans[i] = GameObject.Find(name).GetComponent<Human_KJS>();
            if (gameObject.name == name) myIdx = i;
        }
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
        {   
            // 장전
            if ((inventory.SlotNum == 0 || inventory.SlotNum == 1) && inventory[inventory.SlotNum].value1 == 0)
            {
                human.Reload(true);
                return;
            }
            // 가시여부 체크
            targetVisible = botSight.SightCheck(priorityTarget, -1, true);
            if (targetVisible)
            {
                botMove.ChangeBotMoveState(BotMove.BotMoveState.Idle);
            }
            else
            {
                botMove.ChangeBotMoveState(BotMove.BotMoveState.Follow);
            }
        }
        // 우선 타겟 없을 때
        else
        {   /*
                  1. 기절 살리기
                  2. 회복 - 플레이어, 아군, 본인
                  3. 장전
                  4. 파밍
            */

            // 소생
            foreach (Human_KJS h in humans)
            {
                if (h.humanState == Human_KJS.HumanState.KnockedDown && h.interactionState == Human_KJS.InteractionState.None)
                {   // 내가 기절한 팀원과 가장 가까운지 체크
                    bool isMe = true;
                    for (int i = 1; i < 4; i++)
                    {
                        Human_KJS other = humans[i].GetComponent<Human_KJS>();
                        if (i != myIdx && other.humanState == Human_KJS.HumanState.Normal && other.interactionState == Human_KJS.InteractionState.None &&
                            Vector3.Distance(gameObject.transform.position, h.transform.position) > Vector3.Distance(humans[i].transform.position, h.transform.position))
                        {
                            isMe = false;
                        }
                    }
                    if (isMe)
                    {   // 이동하기
                        approchingTarget = h.gameObject;
                        // 가까우면 상호작용
                        if (Vector3.Distance(transform.position, approchingTarget.transform.position) < 3f)
                        {
                            botMove.ChangeBotMoveState(BotMove.BotMoveState.Idle);
                            human.Interact(approchingTarget, approchingTarget.layer);
                            return;
                        }
                        // 멀 때 이동하기
                        else
                        {
                            botMove.ChangeBotMoveState(BotMove.BotMoveState.Approching);
                            return;
                        }
                    }
                    else approchingTarget = null;
                }
            }
            // 타겟 있을 때
            if (botSight.Target != null)
            {   // 장전
                if ((inventory.SlotNum == 0 || inventory.SlotNum == 1) && inventory[inventory.SlotNum].value1 == 0)
                {
                    human.Reload(true);
                    return;
                }
            }
            // 타겟 없을 때
            else
            {
                GameObject teamTarget;
                for (int i = 1; i < 4; i++)
                {
                    teamTarget = humans[i].GetComponent<BotSight_JSW>().Target;
                    if (teamTarget != null)
                    {
                        botSight.Rot(teamTarget.transform.position);
                        break;
                    }
                }
                // 회복
                if (inventory[3] != null) // 내 회복템 확인
                {
                    // 팀원 힐
                    foreach (Human_KJS h in humans)
                    {
                        if (h.humanState == Human_KJS.HumanState.Normal && h.interactionState == Human_KJS.InteractionState.None && h.HP < 40 && h.gameObject != gameObject)
                        {   // 내가 대사와 가장 가까운지 체크
                            bool isMe = true;
                            for (int i = 1; i < 4; i++)
                            {
                                Human_KJS other = humans[i].GetComponent<Human_KJS>();
                                if (i != myIdx && other.humanState == Human_KJS.HumanState.Normal && other.interactionState == Human_KJS.InteractionState.None &&
                                    other.GetComponent<Inventory_JSW>()[3] != null &&
                                    Vector3.Distance(gameObject.transform.position, h.transform.position) > Vector3.Distance(humans[i].transform.position, h.transform.position))
                                {
                                    isMe = false;
                                }
                            }
                            if (isMe)
                            {   // 이동하기
                                inventory.SetSlotNum(3);
                                approchingTarget = h.gameObject;
                                // 가까우면 상호작용
                                if (Vector3.Distance(transform.position, approchingTarget.transform.position) < 3f)
                                {
                                    botMove.ChangeBotMoveState(BotMove.BotMoveState.Idle);
                                    human.Medikit(h.gameObject);
                                    return;
                                }
                                // 멀 때 이동하기
                                else
                                {
                                    botMove.ChangeBotMoveState(BotMove.BotMoveState.Approching);
                                    return;
                                }
                            }
                            else approchingTarget = null;
                        }
                    }
                    // 자힐
                    if (human.HP < 40)
                    {
                        inventory.SetSlotNum(3);
                        human.Medikit();
                        return;
                    }
                }
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
                    if (farmingTarget == null && inventory[0] != null && ItemTable_JSW.instance.itemTable[inventory[0].kind] is ItemTable_JSW.MainWeapon item && inventory[0].value2 < item.maxAmmoCapacity / 2)
                    {   // 주무기 탐지
                        farmingTarget = botSight.ItemDetect(0);
                    }
                    //// 투척류가 없을 때
                    //if (farmingTarget == null && inventory[2] == null)
                    //{
                    //    farmingTarget = botSight.ItemDetect(2);
                    //}
                    // 회복템이 없을 때
                    if (farmingTarget == null && inventory[3] == null)
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
                        human.Interact(farmingTarget, farmingTarget.layer);
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
        {   // 총을 안들고 있을 때
            if (inventory.SlotNum != 0)
            {   // 주무기가 있으면 들기
                if (inventory[0] != null && (inventory[0].value1 != 0 || inventory[0].value2 != 0)) inventory.SetSlotNum(0);
                else inventory.SetSlotNum(1);
            }
            GameObject target = null;
            if (priorityTarget != null) target = priorityTarget;
            else if (botSight.Target != null) target = botSight.Target;
            if (target == null) return;
            Vector3 origin = transform.position + transform.forward + Vector3.up * (human.humanState == Human_KJS.HumanState.KnockedDown ? 0.8f : 1.4f);
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
            Color c = Color.HSVToRGB(Mathf.Lerp(0, 0.3392157f, hpSlider.value), 1, 1);
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
