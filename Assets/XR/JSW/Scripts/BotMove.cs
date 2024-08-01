using System;
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
    CharacterController cc;

    public enum BotMoveState
    {
        Idle,
        Follow,
        Farming
    }
    BotMoveState botMoveState;
    public void ChangeBotMoveState(BotMoveState s)
    {
        if (botMoveState == s) return;
        botMoveState = s;
        if (s == BotMoveState.Idle)
        {

        }
        else if (s == BotMoveState.Follow)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        botManager = GetComponent<BotManager_JSW>();
        GetComponent<Human_KJS>().slow = Slow;
        GetComponent<Human_KJS>().stun = Stun;
        cc = GetComponent<CharacterController>();
    }
    bool stun = true;
    public void Stun()
    {
        StartCoroutine(StunWait());
    }
    IEnumerator StunWait()
    {
        stun = false;
        agent.enabled = false;
        yield return new WaitForSeconds(1);
        stun = true;
        agent.enabled = true;
    }

    // 플레이어와의 거리
    float targetDis;
    // 플레이어와 유지하는 최대 거리
    float followDis = 7;
    bool isKnockBacking;
    float yVelocity;
    // Update is called once per frame
    void Update()
    {
        if (cc.isGrounded) yVelocity = 0;
        if (botManager.human.knockBackVector.magnitude > 0.1f)
        {
            yVelocity += -9.81f * Time.deltaTime;
            cc.Move((botManager.human.knockBackVector + Vector3.up * yVelocity) * Time.deltaTime);
        }
        if (!stun) return;
        targetDis = Vector3.Distance(transform.position, botManager.PriorityTarget != null ? botManager.PriorityTarget.transform.position : player.transform.position);
        if (botMoveState == BotMoveState.Idle)
        {   // 타겟을 쫒아가야 함 || 플레이어 거리 멀어짐
            if ((botManager.PriorityTarget != null && (!botManager.TargetVisible || targetDis > botManager.botSight.FireRange)) || targetDis > followDis)
            {
                agent.isStopped = false;
                agent.avoidancePriority = UnityEngine.Random.Range(0, 100);
                agent.SetDestination(player.transform.position);
                ChangeBotMoveState(BotMoveState.Follow);
            }
        }
        else if (botMoveState == BotMoveState.Follow)
        {   // 타겟을 사격 가능 || 플레이어가 거리 내로 옴
            if ((botManager.PriorityTarget != null && (botManager.TargetVisible && targetDis <= botManager.botSight.FireRange - 1)) || targetDis < followDis - 1)
            {   // 도착
                agent.isStopped = true;
                ChangeBotMoveState(BotMoveState.Idle);
            }
            else
            {   // 멀음
                agent.SetDestination(player.transform.position);
            }
        }
        else if (botMoveState == BotMoveState.Farming)
        {
            if (botManager.botSight.Target != null || botManager.PriorityTarget != null || botManager.farmingTarget == null)
            {   // 파밍 중단
                botManager.farmingTarget = null;
                ChangeBotMoveState(BotMoveState.Idle);
            }
            else
            {
                agent.SetDestination(botManager.farmingTarget.transform.position);
                agent.isStopped = false;
            }
        }
    }

    // 맞았을 때 감속되는 함수
    public void Slow()
    {
        agent.velocity = Vector3.zero;
    }
}
