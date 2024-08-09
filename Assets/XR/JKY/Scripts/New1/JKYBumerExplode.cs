using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYBumerExplode : MonoBehaviour
{
    public float attackPower = 40f;
    public LayerMask layer;
    public float explosionRadius = 20f;
    // Start is called before the first frame update
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layer);
        foreach (Collider nearbyObject in colliders)
        {
            //PlayerControler_KJS pc = nearbyObject.GetComponent<PlayerControler_KJS>();
            //if (pc != null)
            {
                //pc.ApplyBoomerEffect();
                nearbyObject.GetComponent<Human_KJS>().GetDamage(attackPower, gameObject);
                //pc.TakeDamage(explosionDamage); // 데미지 적용
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    print(layer);
    //    print(other.gameObject.layer);
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
    //    {
    //        print(other + "자폭");

            
    //    }
    }

}
