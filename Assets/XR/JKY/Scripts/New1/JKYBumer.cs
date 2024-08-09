using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JKYBumer : MonoBehaviour
{
    public float explosionRadius = 10f;
    public float explosionForce = 700f;
    public float explosionDelay = 2f;
    public GameObject explosionEffect;

    public int explosionDamage = 20; // 폭발 데미지

    public float vomitRange = 25f; // 구토 공격 범위
    public float vomitCooldown = 9f; // 구토 공격 쿨다운
    public GameObject vomitEffect;
    public int vomitDamage = 10; // 구토 데미지
    public float vomitAngle = 120f; // 구토 공격 시야각
    public PlayerControler_KJS pc;
    private NavMeshAgent agent;
    private Transform target;
    private bool hasExploded = false;
    private bool canVomit = true;
    Animator animator;
    public LayerMask layer;
    public float hp;
    public float maxhp = 120;
    //[SerializeField]

    public BumerState _currentState;
    public  enum BumerState
    {
        Walking,
        Vomit,
        Explode
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        hp = maxhp;
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어를 타겟으로 설정
        GetComponent<JKYEnemyHPSystem>().getDamage = HitEnemy;
    }

    public void Update()
    {
        print(_currentState);
        FindClosestTarget();
        switch (_currentState)
        {
            case BumerState.Walking:
                Walking();
                break;
            case BumerState.Vomit:
                Vomit();
                break;
            case BumerState.Explode:
                Explode();
                break;
        }

    }
    public float currTime;
    public void ChangeState(BumerState state)
    {
        _currentState = state;
        currTime = 0;
        agent.isStopped = true;
        switch(_currentState)
        {
            case BumerState.Walking:
                animator.SetTrigger(_currentState.ToString());
                Walking();
                break;
            case BumerState.Vomit:
                animator.SetTrigger("Attack");
                Vomit();
                break;
            case BumerState.Explode:
                CapsuleCollider coll = GetComponent<CapsuleCollider>();
                coll.enabled = false;
                break;
        }
    }

    void onDie()
    {
        StartCoroutine(Explode1());
        hasExploded = true;
        //ChangeState(BumerState.Explode);
    }
    void Walking()
    {
        if (!hasExploded)
        {
            //agent.SetDestination(target.position);

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= explosionRadius)
            {
                _currentState = BumerState.Explode;

            }
            else if (distanceToTarget <= vomitRange && canVomit)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= vomitAngle / 2f)
                {
                    //_currentState = BumerState.Vomit;
                    //Vomit();
                    StartCoroutine(Vomitto(directionToTarget));
                }
            }
            else
            {
                agent.SetDestination(target.transform.position);
                agent.isStopped = false;
            }
        }
    }
    void Explode()
    {
        StartCoroutine(Explode1());
        hasExploded = true;
    }
    IEnumerator Explode1()
    {
        yield return new WaitForSeconds(explosionDelay);

        // 폭발 효과
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // 폭발 범위 내의 모든 플레이어를 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layer);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Player"))
            {
                PlayerControler_KJS pc = nearbyObject.GetComponent<PlayerControler_KJS>();
                if (pc != null)
                {
                    //pc.ApplyBoomerEffect();
                    //pc.TakeDamage(explosionDamage); // 데미지 적용
                }
            }
        }

        // 부머 제거
        Destroy(gameObject);
    }
    void Vomit()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        StartCoroutine(Vomitto(directionToTarget));
        print("?");
        if (Vector3.Distance(transform.position, target.transform.position) > vomitRange)
        {
            print("설마?");
             ChangeState(BumerState.Walking);

        }          
            

    }
    IEnumerator Vomitto(Vector3 directionToTarget)
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        canVomit = false;
        print(111);
        // 구토 효과

        // 구토 범위 내의 모든 플레이어를 찾기
        GameObject vomit = Instantiate(vomitEffect, target.transform.position, transform.rotation);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, vomitRange, layer))
        {
            print(hit);
            print("독생성");
            if (hit.collider.CompareTag("Player"))
            {
                //Human_KJS pcr = hit.collider.GetComponent<Human_KJS>();
                //if (pcr != null)
                //{
                //    //pcr.ApplyBoomerEffect();
                //    //pcr.TakeDamage(vomitDamage); // 데미지 적용
                //}
            }
        }
        yield return new WaitForSeconds(vomitCooldown);
        canVomit = true;

        // 구토 효과 제거
        Destroy(vomit, 6f);

    }

    public void HitEnemy(float hitPower, GameObject attacker)
    {
        //만일, 이미 피격 상태이거나 사망 상태 또느 ㄴ복귀 상태라면 아무런 처리도 하지 않고 함수를 종ㅇ료
        //if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        //{
        //    return;
        //}
        //플레이어 공격력만큼 에너미의 체력을 감소시킨다.
        hp -= hitPower;
        print(hp);
        // 에너미의 체력이 0보다 크면 피격 상태로 전환
        if (hp > 0)
        {
            //m_State = EnemyState.Damaged;
            //print("상태 전환 Any State -> Damaged");
            //Damaged();
        }
        // 죽음
        else
        {

            onDie();
        }
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




    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, explosionRadius);
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, vomitRange);
    //}
}
