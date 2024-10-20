﻿using System.Collections;
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
    public float fov = 150;
    public float sightRange = 30;
    public float fireRange = 25;
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
                if (!SightCheck(target, -1, true)) 
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
        Collider[] zombiesInRange = Physics.OverlapSphere(transform.position, sightRange, 1 << LayerMask.NameToLayer("Enemy"));
        // 가까운 순 정렬
        zombiesInRange = zombiesInRange.OrderBy(zombie => Vector3.Distance(transform.position, zombie.transform.position)).ToArray();
        // 각도, 시아 체크
        foreach (Collider zombie in zombiesInRange)
        {   // 최상위부모 오브젝트
            GameObject topObj = zombie.gameObject;
            while (topObj.transform.parent != null && topObj.transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                topObj = topObj.transform.parent.gameObject;
            }
            // 시아 체크
            if (SightCheck(topObj, -1, true))
            {
                return topObj;
            }
        }
        return null;
    }
    public GameObject ItemDetect(int slotNum)
    {
        // 범위 내 감지
        Collider[] itemsInRange = Physics.OverlapSphere(transform.position, sightRange, 1 << LayerMask.NameToLayer("Item_JSW"));
        // 가까운 순 정렬
        itemsInRange = itemsInRange.OrderBy(item => Vector3.Distance(transform.position, item.transform.position)).ToArray();
        // 시아 체크
        foreach (Collider item in itemsInRange)
        {   // 최상위부모 오브젝트
            GameObject topObj = item.gameObject;
            while (topObj.transform.parent != null && topObj.transform.parent.gameObject.layer == LayerMask.NameToLayer("Item_JSW"))
            {
                topObj = topObj.transform.parent.gameObject;
            }
            if (SightCheck(topObj, 360))
            {
                if (slotNum == 0 && ItemTable.instance.itemTable[topObj.GetComponent<Item>().kind] is ItemTable.MainWeapon)
                {
                    return topObj;
                }
                else if (slotNum == 1 && (ItemTable.instance.itemTable[topObj.GetComponent<Item>().kind] is ItemTable.SubWeapon || ItemTable.instance.itemTable[topObj.GetComponent<Item>().kind] is ItemTable.MeleeWeapon))
                {
                    return topObj;
                }
                else if (slotNum == 2 && ItemTable.instance.itemTable[topObj.GetComponent<Item>().kind] is ItemTable.Projectile)
                {
                    return topObj;
                }
                if (slotNum == 3 && ItemTable.instance.itemTable[topObj.GetComponent<Item>().kind] is ItemTable.Recovery)
                {
                    return topObj;
                }
            }
        }
        return null;
    }
    // 시아 체크
    public bool SightCheck(GameObject _object, float _fov = -1, bool isZombie = false)
    {
        if (_fov < 0) _fov = fov;
        Vector3 offset = transform.forward + Vector3.up * 1.4f;
        Vector3 dir = (_object.transform.position + (isZombie ? Vector3.up : Vector3.zero) - (transform.position + offset)).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        RaycastHit hitInfo;
        // 각도와 장애물이 없는지 체크 - 살아있는지 체크 추가해야됨
        if (angle <= _fov && Physics.Raycast(transform.position + offset, dir, out hitInfo, sightRange))
        {   // 최상위부모 오브젝트
            GameObject topObj = hitInfo.transform.gameObject;
            while (topObj.transform.parent != null && topObj.transform.parent.gameObject.layer == _object.layer)
            {
                topObj = topObj.transform.parent.gameObject;
            }
            if (topObj == _object)
            {
                if (isZombie && _object.GetComponent<JKYEnemyHPSystem>().isDead) return false;
                return true;
            }
            else return false;
        }
        else return false;
    }
    // 봇 캐릭터 방향 회전
    public void Rot(Vector3 pos)
    {
        float rotY = Quaternion.LookRotation(pos - transform.position).eulerAngles.y;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, rotY);
        if (Mathf.Abs(deltaAngle) < 5) fireEnable = true;
        transform.Rotate(Vector3.up * deltaAngle * Time.deltaTime * rotSpeed);
    }
    // 아이템 체크
}
