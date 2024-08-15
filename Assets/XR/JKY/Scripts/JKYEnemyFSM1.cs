using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class JKYEnemyFSM1 : MonoBehaviour
{
    // Start is called before the first frame update
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Climb_Ready,
        Climb,
        Damaged,
        Die
    }

    // 에너미 상태 변수
    public EnemyState m_State;

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
    public float detectionRange = 15;
    private Vector3 climbTarget;
    private bool isMoving = false;



    // 에너미 시야각
    public float lookRadius = 8f; // 시야반경
    public float fieldOfView = 120f; //시야각도


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
            case EnemyState.Climb_Ready:
                Climb_Ready();

                break;
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
        //float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer < findDistance)
        {
            if (distanceToPlayer <= lookRadius)
            {
                //플레이어가 시야각도 내에 있는지 확인

                Vector3 directionToPlayer = (target.position - transform.position).normalized;
                float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                //if (angleBetweenEnemyAndPlayer <= fieldOfView / 2f)
                {
                    //raycast로 장애물
                    //if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, lookRadius))
                    {
                        //print(hit.transform.gameObject.name + "/" + target.name);
                        //if (hit.transform == target)
                        {
                            m_State = EnemyState.Move;
                            print("상태전환 : Idle -> Move");

                            // 이동 애니메이션으로 전환하기
                            anim.SetTrigger("IdleToMove");
                        }

                    }
                }
            }
        }

    }

    float yRange;
    RaycastHit distance;
    Vector3 climbReadyPo;
    public Transform testTr;
    public float extraRotationSpeed = 0.3f;
    void Move()
    {

        if (Vector3.Distance(transform.position, target.position) > attackDistance)
        {

            smith.stoppingDistance = attackDistance;

            NavMeshPath path = new NavMeshPath();


            float angleBetween = Vector3.Angle(transform.forward, target.transform.position - gameObject.transform.position);
            if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            {
                smith.SetDestination(target.position);
                //y값
                if (angleBetween > 80f)
                {
                    smith.enabled = false;


                    if(Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit))
                    {
                        if(hit.transform != target)
                        {

                            Vector3 direction = target.position - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                            print("돈다");
                        }
                        else if (hit.transform == target)
                        {
                            smith.enabled = true;
                            smith.ResetPath();
                            smith.speed = 8;
                            print("갑자기 빨라짐");
                        }
                    }
                }

                //if (checkForClimbingShortcut())
                //{
                //    if (smith.enabled == false)
                //    {
                //        smith.enabled = true;
                //        // smith.SetDestination(target.position);
                //        print("durldhkTsl");
                //        smith.ResetPath();
                //        smith.SetDestination(target.position);
                //    }
                //}
                //else
                //{
                //    print("다시 돌아왔따");
                //    smith.SetDestination(testTr.position);
                //}

            }

            //// 이거한이유가 속도가 너무 빨라 지나칠떄 딴데봐서 그러나?
            //else
            //{
            //    print("이건뭐지?");
            //    Vector3 dir = target.transform.position - transform.position;
            //    dir.y = 0;
            //    dir.Normalize();

            //    cc.Move(dir * moveSpeed * Time.deltaTime);
            //}


            ////자동으 회전하지만...너무 느려서 보정을 해준다
            ////내가 바라볼 방향의 벡터를 구하고
            //Vector3 lookRotation = target.position - transform.position;
            ////내 smith의 벨로시티와 내가 바라보고자 하는 벡터를
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), extraRotationSpeed * Time.deltaTime);
            ////러프를 이용해서 좀 더 빨리 회전하게 시킨다

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

    private void OnDrawGizmos()
    {
        if (transform != null)
        {
            float halfAngle = detectionAngle / 2.0f;
            for (float currentAngle = -halfAngle; currentAngle <= halfAngle; currentAngle += angleStep)
            {

            }
        }
    }

    public float detectionAngle = 70.0f;
    public float angleStep = 1.0f;
    Vector3 climbReadyPos;

    bool checkForClimbingShortcut()
    {



        float halfAngle = detectionAngle / 2.0f;
        List<RaycastHit> allHits = new List<RaycastHit>();
        //print("여기 들ㄷ어옴?");
        for (float currentAngle = -halfAngle; currentAngle <= halfAngle; currentAngle += angleStep)
        {
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(transform.position, direction, Color.red);
            // 벽을 타고 올라가는 경로를 계산
            RaycastHit hit;
            //print("이제쏠꺼야)");
            if (Physics.Raycast(ray, out hit, detectionRange, climb))
            {
                //print(allHits.Count);
                allHits.Add(hit);
                //print(allHits.Count);
                print("climb벽 찾음");

                //climbTarget = new Vector3(transform.position.x, hit.transform.localScale.y, transform.position.z);

                //// 현재 경로와 벽을 타고 올라가는 경로 비교
                /// 요때만 오르자.

                //float navMeshDistance = smith.remainingDistance;
                //float climbDistance = Vector3.Distance(transform.position, climbTarget) + Vector3.Distance(climbTarget, player.position);

                //if (climbDistance < navMeshDistance)
                //{
                //    print("짧아");
                //    // 벽을 타고 올라가는 경로가 더 짧으면 벽 타기 시작
                //smith.enabled = false;
                //smith.enabled = false;
                //    cc.Move(player.transform.position - transform.position);
                //print("네비끝");
                //isMoving = true;
                //Vector3 directions = target.transform.position - gameObject.transform.position;
                //cc.Move((directions) * moveSpeed * Time.deltaTime);
                //directions.y = 0;
                //directions.Normalize();
                ////isClimbing = true;


                //}
                //smith.enabled = false;
                //cc.Move((player.transform.position - transform.position * moveSpeed * Time.deltaTime));
                //print(111);

            }
            else
            {
                print(111);

            }
        }

        if (allHits.Count > 0)
        {

            float dist = float.MaxValue;
            int shortIdx = -1;

            for (int i = 0; i < allHits.Count; i++)
            {
                if (dist > allHits[i].distance)
                {
                    dist = allHits[i].distance;
                    shortIdx = i;
                    //print(dist);
                }
            }
            //Vector3 topPoint = allHits[shortIdx].point + Vector3.up * distance.transform.GetComponent<Collider>().bounds.size.y;
            float climbDistance = Vector3.Distance(transform.position, allHits[shortIdx].point) + distance.transform.GetComponent<Collider>().bounds.size.y + Vector3.Distance(allHits[shortIdx].point, target.transform.position);
            float navMeshDistance = smith.remainingDistance;
            yRange = Mathf.Abs(transform.position.y - target.transform.position.y);
            if (dist < 1 && yRange > 1)
            {
                smith.enabled = false;
                m_State = EnemyState.Climb;
                print("바로올라가니?");
            }
            else if (dist >= 1 && yRange > 1)
            {
                //if (climbDistance > navMeshDistance)
                //{
                //    print("돌아가");
                //    return true;

                //}
                //else
                {
                    climbReadyPo = climbReadyPos;
                    climbReadyPos = allHits[shortIdx].point + allHits[shortIdx].normal;
                    print(climbReadyPos);
                    print(allHits[shortIdx].transform.position.y + allHits[shortIdx].transform.localScale.y / 2);
                    smith.SetDestination(climbReadyPos);
                    smith.stoppingDistance = 0;
                    //m_State = EnemyState.Climb_Ready;

                    print("ey");
                    distance = allHits[shortIdx];
                }

            }
            return false;
        }
        return true;
    }
    // 이동 시간(초)
    public float moveDuration = 5.0f;

    // 타이머 변수
    private float timer = 0.0f;
    void Climb_Ready()
    {
        smith.enabled = false;
        print("지금상태 climb_ready");
        //cc.Move(Vector3.forward * moveSpeed * Time.deltaTime);
        if (timer < moveDuration)
        {
            Vector3 lookRotation1 = Vector3.forward;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation1), extraRotationSpeed * Time.deltaTime);
            // 오브젝트를 전방으로 이동시킵니다.
            //transform.Translate(Vector3.forward * speed * Time.deltaTime);
            cc.Move(Vector3.forward * speed * Time.deltaTime);
            // 타이머 갱신
            timer += Time.deltaTime;
        }

        if (timer > moveDuration)
        {
            //Vector3 lookRotation = target.position - transform.position;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), extraRotationSpeed * Time.deltaTime);
            m_State = EnemyState.Climb;
            timer = 0;
        }
        print("여기끝");
    }

    public float speed = 2f;



    //float cy;
    //float cly;
    //float movingTime = 0;
    //bool isMovingUp = false;
    //float ey;
    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("climb"))
    //    {
    //        print("벽에부딪힘");
    //        m_State = EnemyState.Climb;
    //        isClimbing = true;
    //        cy = collision.transform.position.y;
    //        cly = collision.transform.localScale.y/2;
    //    }
    //}
    void Climb()
    {
        print("Climb상태");
        print(distance.transform.GetComponent<Collider>().bounds.size.y); // 콜라이더 바운더리 사이즈로 불러올수도있다.
        if (transform.position.y < distance.transform.GetComponent<Collider>().bounds.size.y + 0.1f)
        {
            //Vector3 lookRotati = transform.position - climbReadyPo;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotati), extraRotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.up * speed * Time.deltaTime);

        }

        if (transform.position.y > distance.transform.GetComponent<Collider>().bounds.size.y + 0.1f)
        {
            // 타이머 갱신
            timer += Time.deltaTime;
            if (timer < moveDuration)
            {
                //Vector3 lookRotation = target.position - transform.position;
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotation), extraRotationSpeed * Time.deltaTime);
                // 오브젝트를 전방으로 이동시킵니다.
                transform.Translate(Vector3.forward * speed * Time.deltaTime);



            }
            if (timer > moveDuration)
            {
                m_State = EnemyState.Move;
                timer = 0;
                print("상태변환 --> MOVE");
            }

        }
    }



    //여기 바꿧다!!!!!!!!!!!!!!!!!!!!!!!!!1
    public float rotationSpeed = 39f;
    void Attack()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어 공격
        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            // 일정시간마다 플레이어 공격
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                //player.GetComponent<JKYPlayerMove>().DamageAction(attackPower);
                target.GetComponent<Human_KJS>().GetDamage(attackPower, gameObject);

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

    //public void AttackAction()
    //{
    //    //print("attackaction");
    //    player.GetComponent<PlayerControler_KJS>().DamageAction();
    //}
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
        if (m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        //플레이어 공격력만큼 에너미의 체력을 감소시킨다.
        hp -= hitPower;

        // 에너미의 체력이 0보다 크면 피격 상태로 전환
        if (hp > 0)
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
            JKYEnemyHPSystem dead = GetComponent<JKYEnemyHPSystem>();
            dead.isDead = true;
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
        yield return new WaitForSeconds(0.7f);
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
            if (distanceToTarget < closestDistance && target.GetComponent<Human_KJS>().humanState != Human_KJS.HumanState.Dead)
            {
                closestDistance = distanceToTarget;
                closestTarget = target.transform;
            }
        }

        target = closestTarget;
        //print(target);
    }



}

