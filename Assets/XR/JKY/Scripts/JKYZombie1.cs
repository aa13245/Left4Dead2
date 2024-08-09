using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class JKYZombie1 : MonoBehaviour
{
    private enum ZombieState
    {
        Walking,
        Ragdoll
    }
    
    [SerializeField]
    public Transform target;

    private Rigidbody[] _ragdollRigidbodies;
    private ZombieState _currentState = ZombieState.Walking;
    private Animator _animator;
    private CharacterController _characterController;
    NavMeshAgent Agent;
    void Awake()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget();
        switch (_currentState)
        {
            case ZombieState.Walking:
                WalkingBehaviour();
                break;
            case ZombieState.Ragdoll:
                RagdollBehaviour();
                break;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        EnableRagdoll();

        Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);

        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);

        _currentState = ZombieState.Ragdoll;
    }

    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rigidbody.position, hitPoint);
            if( closestRigidbody == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidbody = rigidbody;
            }
        }
        return closestRigidbody;
    }
    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        _animator.enabled = true;
        _characterController.enabled = true;
    }
    
    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        _animator.enabled = false;
        _characterController.enabled = false;
    }

    private void WalkingBehaviour()
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);
        Agent.SetDestination(target.position);
    }

    private void RagdollBehaviour()
    {

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
