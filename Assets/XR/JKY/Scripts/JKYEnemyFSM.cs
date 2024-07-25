using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class JKYEnemyFSM : MonoBehaviour
{
    // Start is called before the first frame update
    enum EnemyState
    {
        Idle,
        Move, 
        Attack,
        Return,
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
    public int attackPower = 3;
    //
    // 초기 위치 저장용 변수
    Vector3 originPos;

    // 발사 무기 공격력
    public int weaponPower = 5;

    // 이동가능범위
    public float moveDistance = 20f;

    // 에너미의 체력
    public int hp = 15;

    void Start()
    {
        // 최초상태 대기
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();

        //자신의 초기 위치 저장하기 
        originPos = transform.position;
        //
    }

    // Update is called once per frame
    void Update()
    {
    // 에너미 상태상수
        switch(m_State)
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
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("상태전환 : Idle -> Move");
        }    
    }

    void Move()
    {
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // 이동 방향 설정
            Vector3 dir = (player.position - transform.position).normalized;
            //캐릭터 컨트롤러를 이용해 이동하기
            cc.Move(dir * moveSpeed * Time.deltaTime);

        }
        else
        {
            m_State = EnemyState.Attack;
            print("상태전환 Move -> attack");

            // 누적시간을 공격 딜레이 시간만큼 미리 진행시켜 놓는다.
            currentTime = attackDelay; 

        }

        // 만일 현재 위치가 초기 위치에서 이동 가능 범위를 넘어간다면...
        
    }

    void Attack()
    {
        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어 공격
        if(Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // 일정시간마다 플레이어 공격
            currentTime += Time.deltaTime;
            if(currentTime > attackDelay)
            {
                player.GetComponent<JKYPlayerMove>().DamageAction(attackPower);
                print("공격");
                currentTime = 0;
            }
        }
        else
        {
            m_State = EnemyState.Move;
            print("상태전환 : attack ->move");
            currentTime = 0;
        }
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

    public void HitEnemy(int hitPower)
    {
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
            //Die();
        }
    }


}

