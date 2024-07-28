using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BotMove : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    BotManager_JSW botManager;

    public enum BotState
    {
        Idle,
        Follow,
    }
    BotState botState;
    void ChangeBotState(BotState s)
    {
        if (botState == s) return;
        botState = s;
        if (s == BotState.Idle)
        {

        }
        else if (s == BotState.Follow)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        botManager = GetComponent<BotManager_JSW>();
    }

    // 플레이어와의 거리
    float targetDis;
    // 플레이어와 유지하는 최대 거리
    float followDis = 7;
    // Update is called once per frame
    void Update()
    {
        targetDis = Vector3.Distance(transform.position, botManager.PriorityTarget != null ? botManager.PriorityTarget.transform.position : player.transform.position);
        if (botState == BotState.Idle)
        {   // 타겟을 쫒아가야 함 || 플레이어 거리 멀어짐
            if ((botManager.PriorityTarget != null && (!botManager.TargetVisible || targetDis > botManager.botSight.FireRange)) || targetDis > followDis)
            {
                agent.isStopped = false;
                agent.avoidancePriority = Random.Range(0, 100);
                agent.SetDestination(player.transform.position);
                ChangeBotState(BotState.Follow);
            }
        }
        else if (botState == BotState.Follow)
        {   // 타겟을 사격 가능 || 플레이어가 거리 내로 옴
            if ((botManager.PriorityTarget != null && (botManager.TargetVisible && targetDis <= botManager.botSight.FireRange - 1)) || targetDis < followDis - 1)
            {   // 도착
                agent.isStopped = true;
                ChangeBotState(BotState.Idle);
            }
            else
            {   // 멀음
                agent.SetDestination(player.transform.position);
            }
        }
    }
}
