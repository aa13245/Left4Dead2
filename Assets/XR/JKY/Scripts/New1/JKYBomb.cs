using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class JKYBomb : MonoBehaviour
{
    public float radius;
    public float power = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ExplosionBomb()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);//, LayerMask.NameToLayer("Enemy"));
        //foreach (Collider hit in colliders)
        //{)
        //    Rigidbody rb = hit.GetComponentInChildren<Rigidbody>();
        //    rb.isKinematic = false;

        //}
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null)
            {
                
                //rb.gameObject.GetComponent<NavMeshAgent>().speed = 0;
                if (rb.gameObject.GetComponent<NavMeshAgent>())
                {
                    rb.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                }

                if(rb.gameObject.GetComponent<CharacterController>())
                {

                    // rb.gameObject.GetComponent<CharacterController>().enabled = false;

                }
                rb.isKinematic = false;
                rb.useGravity = true;
                if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    // 적에게 데미지를 적용
                    hit.gameObject.GetComponent<JKYEnemyFSM>()?.HitEnemy(power, gameObject);
                }

                rb.AddExplosionForce(power, transform.position, radius, 10.0f);

            }
        }
    }
}
