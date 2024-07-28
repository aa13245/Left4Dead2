using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BotSight_JSW : MonoBehaviour
{
    NavMeshAgent agent;
    BotManager_JSW botManager;
    GameObject target;
    public GameObject Target {  get { return target; } }
    float fov = 150;
    float sightRange = 30;
    float fireRange = 25;
    public float FireRange { get { return fireRange; } }
    bool fireEnable;
    public bool FireEnable { get { return fireEnable; } }
    float rotSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        botManager = GetComponent<BotManager_JSW>();
    }

    // Update is called once per frame
    void Update()
    {   
        fireEnable = false;
        // 우선 순위 타겟이 있을 때
        if (botManager.PriorityTarget)
        {   // 보이면 쳐다보기
            if (botManager.TargetVisible)
            {
                agent.updateRotation = false;
                Rot(botManager.PriorityTarget.transform.position);
            }
            else
            {
                agent.updateRotation=true;
            }
        }
        else
        {
            // 타겟이 없을 때
            if (target == null)
            {   // 타겟 탐지
                target = DetectBySight();
            }
            // 타겟이 있을 때
            else
            {   // 있다면 시아 체크
                if (!SightCheck(target)) 
                { 
                    target = null;
                    agent.updateRotation = true;
                    return; 
                }
                // 타겟 방향으로 회전
                agent.updateRotation = false;
                Rot(target.transform.position);
            }

        }
    }
    // 좀비 시아 감지
    GameObject DetectBySight()
    {   // 범위 내 감지
        Collider[] zombiesInRange = Physics.OverlapSphere(transform.position, sightRange, 1 << LayerMask.NameToLayer("Zombie_JSW"));
        // 가까운 순 정렬
        zombiesInRange = zombiesInRange.OrderBy(zombie => Vector3.Distance(transform.position, zombie.transform.position)).ToArray();
        // 각도, 시아 체크
        foreach (Collider zombie in zombiesInRange)
        {   // 시아 체크
            if (SightCheck(zombie.transform.gameObject))
            {
                return zombie.gameObject;
            }
        }
        return null;
    }
    // 시아 체크
    public bool SightCheck(GameObject _object)
    {
        Vector3 offset = transform.forward + Vector3.up * 1.6f;
        Vector3 dir = (_object.transform.position - (transform.position + offset)).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        RaycastHit hitInfo;
        // 각도와 장애물이 없는지 체크 - 살아있는지 체크 추가해야됨
        if (angle <= fov && Physics.Raycast(transform.position + offset, dir, out hitInfo, sightRange) && hitInfo.transform.gameObject == _object)
        {
            return true;
        }
        else return false;
    }
    // 봇 캐릭터 방향 회전
    void Rot(Vector3 pos)
    {
        float rotY = Quaternion.LookRotation(pos - transform.position).eulerAngles.y;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, rotY);
        if (Mathf.Abs(deltaAngle) < 5) fireEnable = true;
        transform.Rotate(Vector3.up * deltaAngle * Time.deltaTime * rotSpeed);
    }
    // 아이템 체크
    public GameObject DetectItem()
    {
        return null;
    }
}
