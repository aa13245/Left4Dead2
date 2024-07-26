using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BotMove : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;

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
    }

    // 플레이어와의 거리
    float playerDis;
    // 플레이어와 유지하는 최대 거리
    float followDis = 7;

    // Update is called once per frame
    void Update()
    {
        playerDis = Vector3.Distance(transform.position, player.transform.position);
        if (botState == BotState.Idle)
        {   // 플레이어 거리 멀어짐
            if (playerDis > followDis)
            {
                agent.isStopped = false;
                agent.avoidancePriority = Random.Range(0, 100);
                agent.SetDestination(player.transform.position);
                ChangeBotState(BotState.Follow);
            }
        }
        else if (botState == BotState.Follow)
        {
            if (playerDis > followDis - 1)
            {   // 멀음
                agent.SetDestination(player.transform.position);
            }
            else
            {   // 도착
                agent.isStopped = true;
                ChangeBotState(BotState.Idle);
            }
        }
    }
}
