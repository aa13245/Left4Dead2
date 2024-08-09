using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class JKYEnemyMove1 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private JKYAttackable Attackable;
    [SerializeField]
    NavMeshAgent Agent;
    [SerializeField]
    public Animator Animator;
    [SerializeField]
    public EnemyState State;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float FieldOfView = 65;
    [SerializeField]
    private float LineOfSightDistance = 7f;
    [SerializeField]
    private float IdleSpeedModifier = 0.25f;

    private float InitialSpeed;
    private Vector3 TargetLocation;
    public float attackPower = 3;
    public static NavMeshTriangulation Triangulation;

    public enum EnemyState
    {
        Initial,
        Idle,
        Chasing,
        Attacking,
        Damaged,
        Dead
    }
    private void Awake()
    {
        
        Agent = GetComponent<NavMeshAgent>();
        Attackable = GetComponent<JKYAttackable>();
        Animator = transform.GetComponentInChildren<Animator>();
        //InitialSpeed = Agent.speed;
        State = EnemyState.Chasing;
        //Agent.stoppingDistance = 3f;
        if (Triangulation.vertices == null || Triangulation.vertices.Length == 0)
        {
            Triangulation = NavMesh.CalculateTriangulation();
        }

        Attackable.OnTakeDamage += GetAggressive;
    }

    private void GetAggressive()
    {
        if (State == EnemyState.Idle || State == EnemyState.Initial)
        {
            State = EnemyState.Chasing;
            Agent.speed = InitialSpeed;
            Animator.SetBool("HasTarget", true);
        }
    }

    public void Update()
    {
        FindClosestTarget();
        print(State);
        if (Agent.enabled)
        {
            switch (State)
            {
                case EnemyState.Initial:
                    State = EnemyState.Idle;
                    break;
                case EnemyState.Idle:
                    DoIdleMovement();
                    break;
                case EnemyState.Chasing:
                    DoTargetMovement();
                    break;
                case EnemyState.Attacking:
                    DoAttack();
                    break;
                case EnemyState.Damaged:
                    //DoDamaged();
                    break;
            }
        }
    }

    private void DoIdleMovement()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        print(00);
        if (Vector3.Distance(transform.position, target.position) < LineOfSightDistance)
            //&& Vector3.Dot(transform.forward, direction) >= Mathf.Cos(FieldOfView))
        {
            
            GetAggressive();
        }
        else
        {
            
            Agent.speed = InitialSpeed * IdleSpeedModifier;
            Animator.SetBool("HasTarget", false);

            if (Vector3.Distance(transform.position, TargetLocation) <= Agent.stoppingDistance || TargetLocation == Vector3.zero)
            {
                Vector3 triangle1 = Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)];
                Vector3 triangle2 = Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)];

                TargetLocation = Vector3.Lerp(triangle1, triangle2, Random.value);
                Agent.SetDestination(TargetLocation);
            }
        }
    }

    private void DoTargetMovement()
    {
        Agent.stoppingDistance = 2;
        Animator.SetBool("HasTarget", true);
        if (Vector3.Distance(target.position, transform.position) > (Agent.stoppingDistance + Agent.radius) * 1)
        {
            Agent.SetDestination(target.position);
        }
        else
        {
            State = EnemyState.Attacking;
        }
    }

    private void DoAttack()
    {
        print(Agent.stoppingDistance);
        print(Agent.radius);
        //if (Vector3.Distance(target.position, transform.position) > (Agent.stoppingDistance + Agent.radius) * 1)
        //{
        //    Animator.SetBool("IsAttacking", false);
        //    State = EnemyState.Chasing;
        //}
        //else
        //{
        //    Quaternion lookRotation = Quaternion.LookRotation((target.position - transform.position).normalized);
        //    transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        //    Animator.SetBool("IsAttacking", true);
        //    target.GetComponent<Human_KJS>().GetDamage(attackPower, gameObject);
        //}
        if (IsDelayComplete(damageDelay))
        {
            // 나의 행동을 결정하자.
            // 만약에 Player와 거리가 attakRange보다 작으면
            //float dist = Vector3.Distance(player.transform.position, transform.position);
            if (Vector3.Distance(target.position, transform.position) > (Agent.stoppingDistance + Agent.radius) * 1)
            {
                // 런
                Animator.SetBool("IsAttacking", false);
                State = EnemyState.Chasing;
                currTime = 0;
                
            }
            // 그렇지 않고 인지범위보다 작으면

            // 그렇지 않고 인지범위보다 크면
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation((target.position - transform.position).normalized);
                transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
                Animator.SetBool("IsAttacking", true);
                target.GetComponent<Human_KJS>().GetDamage(attackPower, gameObject);
                currTime = 0;
            }
            
        }
    }

    bool isDamageMotion = false;

    public float damageDelay = 1;
    public void DoDamaged()
    {
        print("여기들와?");
        if(isDamageMotion == false)
        {

            Animator.SetTrigger("Damage");
            isDamageMotion = true;
        }
        //StartCoroutine(damage());
        if (IsDelayComplete(1.3f))
        {
            // 나의 행동을 결정하자.
            // 만약에 Player와 거리가 attakRange보다 작으면
            //float dist = Vector3.Distance(target.transform.position, transform.position);
            if (Vector3.Distance(target.position, transform.position) < (Agent.stoppingDistance + Agent.radius) * 1)
            {
                // 공격상태로 전환
                State = EnemyState.Attacking;
                
            }
            // 그렇지 않고 인지범위보다 작으면

            else
            {
                // 대기상태로 전환
                State = EnemyState.Chasing;
                
            }

            isDamageMotion = false;

        }
    //private IEnumerator damage()
    //{
    //    Animator.SetBool("Damaged", true);
    //    yield return new WaitForSeconds(1.5f);

    //    if (Vector3.Distance(target.position, transform.position) <= (Agent.stoppingDistance + Agent.radius) * 1)
    //    {
    //        // 공격상태로 전환
    //        //Animator.SetBool("Damaged", false);

    //        State = EnemyState.Attacking;


    //    }
    //    // 그렇지 않고 인지범위보다 크면
    //    else
    //    {
    //        //Animator.SetBool("Damaged", false);
    //        // 대기상태로 전환
    //        State = EnemyState.Chasing;
    //    }


    }
    public float currTime;
    bool IsDelayComplete(float delayTime)
    {
        // 시간을 증가 시키자.
        currTime += Time.deltaTime;
        //만약에 시간이 delayTime보다 커지면
        if (currTime >= delayTime)
        {
            //// 현재시간 초기화
            currTime = 0;
            // true반환
            return true;

        }
        // 그렇지 않으면

        // false 반환
        return false;
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
