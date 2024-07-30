﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class JKYEnemyFSM : MonoBehaviour
{
    // Start is called before the first frame update
    enum EnemyState
    {
        Idle,
        Move, 
        Attack,
        Return,
        Climb,
        Damaged,
        Die
    }

    // 에너미 상태 변수
    EnemyState m_State;

    //플레이어 발견 범위
    public float findDistance = 8f;

    // 플레이어 트랜스폼
    Transform player;

    // 공격 가능범위
    public float attackDistance = 2f;

    // 이동속도
    public float moveSpeed = 5f;

    // 누적 시간
    float currentTime = 0;

    // 공격딜레이시간
    float attackDelay = 2f;

    CharacterController cc;

    //에너미 공격력
    public float attackPower = 3;
    //
    // 초기 위치 저장용 변수
    Vector3 originPos;

    // 발사 무기 공격력
    public float weaponPower = 5;

    // 이동가능범위
    public float moveDistance = 20f;

    // 에너미의 체력
    public float hp = 50;
    public float maxhp = 50;
    Animator anim;

    // 내비게이션 에이전트 변수
    NavMeshAgent smith;

    // 클라이밍
    public float climbSpeed = 1f;
    public float detectionDistance = 2f;
    public LayerMask climb;
    private bool isClimbing = false;
    private Rigidbody rb;
    public float climbHeight = 5;
    private Vector3 climbTarget;
    private bool isMoving = false;



    // 에너미 시야각
    public float lookRadius = 8f; // 시야반경
    public float fieldOfView = 120f; //시야각도
    private bool playerInSight = false; //플레이어가 시야에 있는지 여부


    // 더가까운 플레이어찾기
    private Transform target;
    //private Transform enemy;
    void Start()
    {
        // 최초상태 대기
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();

        //자신의 초기 위치 저장하기 
        originPos = transform.position;

        anim = transform.GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();

        //hp = maxhp;
        rb = GetComponent<Rigidbody>();
        //Vector3 enemyy = enemy.position.y;
        //
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget();
        // 에너미 상태상수
        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            //case EnemyState.Return:
            //Return();
            //break;
            case EnemyState.Climb:
                Climb();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }
    }
    void Idle()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer < findDistance)
        {
            if (distanceToPlayer <= lookRadius)
            {
                //플레이어가 시야각도 내에 있는지 확인
                Vector3 directionToPlayer = (target.position - transform.position).normalized;
                float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleBetweenEnemyAndPlayer <= fieldOfView / 2f)
                {
                    //raycast로 장애물
                    if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, lookRadius))
                    {
                        if (hit.transform == target)
                        {
                            playerInSight = true;
                            m_State = EnemyState.Move;
                            print("상태전환 : Idle -> Move");

                            // 이동 애니메이션으로 전환하기
                            anim.SetTrigger("IdleToMove");
                        }
                        else
                        {
                            playerInSight = false;
                        }
                    }
                }
                else { playerInSight = false; }
            }
            else { playerInSight = false; }
        }

    }

    public float extraRotationSpeed = 0.3f;
    void Move()
    {

        if (Vector3.Distance(transform.position, target.position) > attackDistance)
        {
            // 이동 방향 설정
            //Vector3 dir = (player.position - transform.position).normalized;
            ////캐릭터 컨트롤러를 이용해 이동하기
            //cc.Move(dir * moveSpeed * Time.deltaTime);

            ////플레이어를 향해 방향을 전환한다.
            //transform.forward = dir;

            // 내비게이션 에이전트의 이동을 멈추고 경로를 초기화한다.
            //smith.isStopped = true;
            //smith.ResetPath();
            // 내비게이션으로 접근하는 최소 거리를 공격 가능 거리로 설정한다.
            smith.stoppingDistance = attackDistance;

            //내비게이션의 목적지를 플레이어의 위치로 설정한다.


            NavMeshPath path = new NavMeshPath();
            
            if (isClimbing)
            {
                print("climb함수들어왔다 트루");
                m_State = EnemyState.Climb;
                print("상태전환 Move -> Climb");
            }
            else if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            {

                checkForClimbingShortcut();
                smith.SetDestination(target.position);

            }
            
            // 이거한이유가 속도가 너무 빨라 지나칠떄 딴데봐서 그러나?
            else
            {
                print("이건뭐지?");
                Vector3 dir = target.transform.position - transform.position;
                dir.y = 0;
                dir.Normalize();

                cc.Move(dir * moveSpeed * Time.deltaTime);
            }


            //자동으 회전하지만...너무 느려서 보정을 해준다
            //내가 바라볼 방향의 벡터를 구하고
            Vector3 lookRotation = target.position - transform.position;
            //내 smith의 벨로시티와 내가 바라보고자 하는 벡터를
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), extraRotationSpeed * Time.deltaTime);
            //러프를 이용해서 좀 더 빨리 회전하게 시킨다

        }
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            m_State = EnemyState.Attack;
            print("상태전환 Move -> attack");

            // 누적시간을 공격 딜레이 시간만큼 미리 진행시켜 놓는다.
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }

        // 만일 현재 위치가 초기 위치에서 이동 가능 범위를 넘어간다면...
        
    }

    void checkForClimbingShortcut()
    {
        
        // 벽을 타고 올라가는 경로를 계산
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, climbHeight, climb))
        {
            print("climb");
            //climbTarget = new Vector3(transform.position.x, hit.transform.localScale.y, transform.position.z);

            //// 현재 경로와 벽을 타고 올라가는 경로 비교
            //float navMeshDistance = smith.remainingDistance;
            //float climbDistance = Vector3.Distance(transform.position, climbTarget) + Vector3.Distance(climbTarget, player.position);

            //if (climbDistance < navMeshDistance)
            //{
            //    print("짧아");
            //    // 벽을 타고 올라가는 경로가 더 짧으면 벽 타기 시작
            smith.enabled = false;
            //    cc.Move(player.transform.position - transform.position);
            isClimbing = true;
            print("네비끝");
            isMoving = true;
            cc.Move((target.transform.position - gameObject.transform.position) * moveSpeed * Time.deltaTime);
            //}
            //smith.enabled = false;
            //cc.Move((player.transform.position - transform.position * moveSpeed * Time.deltaTime));
            //print(111);

        }
    }
    void Climb()
    {
        print("왜여기까지안오냐고");
        //isClimbing = true;
        isMoving = false;
        print("부딪혓다");
        cc.Move(Vector3.up * climbSpeed * Time.deltaTime);
        //if (gameObject.transform.position.y > cy + cly + ey)
        //{
        //    print("끝까지 올라왔다");
        //    movingTime += Time.deltaTime;
        //    isMovingUp = false;
        //    isMoving = true;
        //    //cc.Move((player.transform.position - gameObject.transform.position) * moveSpeed * Time.deltaTime);

        //    if (movingTime > 2f)
        //    {
        //        print("좀만 앞으로가");
        //        isMoving = false;
        //        movingTime = 0;
        //        smith.enabled = true;
        //        smith.Warp(climbTarget); // 새로운 위치로 NavMeshAgent 이동
        //        smith.destination = player.position;
        //    }
        //}
    }
    float movingTime = 0;
    bool isMovingUp = false;
    float ey;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("climb"))
        {
            isClimbing = true;
            float cy = collision.transform.position.y;
            float cly = collision.transform.localScale.y/2;
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.layer == LayerMask.NameToLayer("climb"))
    //    {
    //        ey = gameObject.transform.position.y;
    //        isMoving = false;
    //        print("부딪혓다");
    //        isMovingUp = true;
    //        cc.Move((climbTarget - transform.position) * climbSpeed * Time.deltaTime);
    //        if (gameObject.transform.position.y > collision.transform.position.y + collision.transform.localScale.y/2 + ey)
    //        {
    //            print("끝까지 올라왔다");
    //            movingTime += Time.deltaTime;
    //            isMovingUp = false;
    //            isMoving = true;
    //            //cc.Move((player.transform.position - gameObject.transform.position) * moveSpeed * Time.deltaTime);

    //            if (movingTime > 2f)
    //            {
    //                print("좀만 앞으로가");
    //                isMoving = false;
    //                movingTime = 0;
    //                smith.enabled = true;
    //                smith.Warp(climbTarget); // 새로운 위치로 NavMeshAgent 이동
    //                smith.destination = player.position;
    //            }
    //        }

    //    }
    //}
    //void DetectWall()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, climb))
    //    {
    //        if (hit.collider != null)
    //        {
    //            print(222);
    //            isClimbing = true;
    //            rb.useGravity = false; // 벽을 탈 때 중력을 제거합니다.
    //        }
    //    }
    //    else
    //    {
    //        isClimbing = false;
    //        rb.useGravity = true; // 벽을 타지 않을 때 중력을 다시 활성화합니다.
    //    }

    //}

    void ClimbWall()
    {
        print("올라간다");
        //cc.Move((climbTarget- transform.position) * climbSpeed * Time.deltaTime);
        cc.Move((player.transform.position - transform.position * moveSpeed * Time.deltaTime));
        
        //// 벽을 다 올라갔는지 체크
        //if (Vector3.Distance(transform.position, climbTarget) < 0.1f)
        //{
        //    isClimbing = false;
        //    smith.enabled = true;
        //    smith.Warp(climbTarget); // 새로운 위치로 NavMeshAgent 이동
        //    smith.destination = player.position;
        //}
    }
    //여기 바꿧다!!!!!!!!!!!!!!!!!!!!!!!!!1
    public float rotationSpeed = 15f;
    void Attack()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어 공격
        if(Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            // 일정시간마다 플레이어 공격
            currentTime += Time.deltaTime;
            if(currentTime > attackDelay)
            {
                //player.GetComponent<JKYPlayerMove>().DamageAction(attackPower);
                player.GetComponent<Human_KJS>().GetDamage(attackPower);
                print("공격");
                currentTime = 0;

                anim.SetTrigger("StartAttack");

            }
        }
        else
        {
            m_State = EnemyState.Move;
            print("상태전환 : attack ->move");
            currentTime = 0;

            anim.SetTrigger("AttackToMove");

        }
    }

    public void AttackAction()
    {
        //print("attackaction");
        player.GetComponent<JKYPlayerMove>().DamageAction(attackPower);
    }
    void Damaged()
    {
        // 피격 상태를 처리하기 위한 코루틴\
        StartCoroutine(DamageProcess());

    }
    IEnumerator DamageProcess()
    {
        //피격 모션시간만큼 기다린다
        yield return new WaitForSeconds(0.5f);

        m_State = EnemyState.Move;
        print("상태전환 Damaged -.move");
        
    }

    public void HitEnemy(float hitPower)
    {
        //만일, 이미 피격 상태이거나 사망 상태 또느 ㄴ복귀 상태라면 아무런 처리도 하지 않고 함수를 종ㅇ료
        if(m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        //플레이어 공격력만큼 에너미의 체력을 감소시킨다.
        hp -= hitPower;

        // 에너미의 체력이 0보다 크면 피격 상태로 전환
        if( hp >0)
        {
            m_State = EnemyState.Damaged;
            print("상태 전환 Any State -> Damaged");
            Damaged();
        }
        // 죽음
        else
        {
            m_State = EnemyState.Die;
            print("상태전환 Any state -> Die");

            anim.SetTrigger("Die");
            Die();
        }
    }

    void Die()
    {
        // 진행중인 피격 코루틴을 중지한다.
        StopAllCoroutines();
        // 죽음상태를 처리하기 위한 코루틴을 실행한다.
        print("코루틴 시작");
        StartCoroutine(DieProcess());

    }
    IEnumerator DieProcess()
    {
        print("소멸");
        //캐릭터 콘트롤러를 비활성
        cc.enabled = false;

        // 2초 동안 기다린 후에 자기 자신을 제거한다.
        yield return new WaitForSeconds(2f);
        print("소멸");
        Destroy(gameObject);
        
    }


    void FindClosestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //GameObject[] players = LayerMask.NameToLayer("Player_KJS");
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        //GameObject[] allies = GameObject.FindGameObjectsWithTag("Bot_JSW");
        List<GameObject> allTargets = new List<GameObject>();
        allTargets.AddRange(players);
        allTargets.AddRange(allies);

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject target in allTargets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = target.transform;
            }
        }

        target = closestTarget;
        //print(target);
    }

}

