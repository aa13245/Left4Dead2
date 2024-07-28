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
                 * 3. 파밍
                 */
            }
        }
        // 공격
        if (botSight.FireEnable)
        {

        }
    }
}
