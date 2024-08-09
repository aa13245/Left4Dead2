using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JKYAttackable : MonoBehaviour
{
    [field: SerializeField]
    public float Life
    {
        get; private set;
    } = 100;
    [SerializeField]
    private JKYRagdollEnabler RagdollEnabler;
    private JKYEnemyMove1 move1;
    [SerializeField]
    private float FadeOutDelay = 2f;

    public delegate void DeathEvent(JKYAttackable Attackable);
    public DeathEvent OnDie;

    public delegate void TakeDamageEvent();
    public TakeDamageEvent OnTakeDamage;
    CharacterController cc;



    [SerializeField]
    private float _maximumForce;

    [SerializeField]
    private float _maximumForceTime;

    private float _timeMouseButtonDown;

    private Camera _camera;
    private void Start()
    {
        //CharacterController cc = GetComponentInParent<CharacterController>();
        RagdollEnabler = GetComponent<JKYRagdollEnabler>();
        move1 = GetComponent<JKYEnemyMove1>();
       
        if (RagdollEnabler != null)
        {
            
            RagdollEnabler.EnableAnimator();
        }
        GetComponent<JKYEnemyHPSystem>().getDamage = HitEnemy;
    }

    public void Update()
    {

    }

    void TriggerRandomDamage()
    {
        int randomTrigger = Random.Range(1, 3);
        if (randomTrigger == 1)
        {
            move1.Animator.SetTrigger("LDamaged");
        }
        else if (randomTrigger == 2)
        {
            move1.Animator.SetTrigger("RDamaged");
        }
    }
    public void HitEnemy(float hitPower, GameObject attacker)
    {

        TriggerRandomDamage();
        //move1.State = JKYEnemyMove1.EnemyState.Damaged;
        print(Life);
        Life -= hitPower;
        OnTakeDamage?.Invoke();
        if (Life <= 0 && RagdollEnabler != null)
        {
            
            OnDie?.Invoke(this);
            RagdollEnabler.EnableRagdoll();
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        cc.enabled = false;
        yield return new WaitForSeconds(FadeOutDelay);

        if (RagdollEnabler != null)
        {
            RagdollEnabler.DisableAllRigidbodies();
        }

        float time = 0;
        while (time < 1)
        {
            transform.position += Vector3.down * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }




    
}
