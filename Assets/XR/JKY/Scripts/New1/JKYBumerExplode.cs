﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYBumerExplode : MonoBehaviour
{
    public float attackPower = 40f;
    public LayerMask layer;
    public float explosionRadius = 20f;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layer);
        foreach (Collider nearbyObject in colliders)
        {
            //PlayerControler_KJS pc = nearbyObject.GetComponent<PlayerControler_KJS>();
            //if (pc != null)
            {
                //pc.ApplyBoomerEffect();
                Human_KJS human = nearbyObject.GetComponent<Human_KJS>();
                nearbyObject.GetComponent<Human_KJS>().GetDamage(attackPower, gameObject);
                if (nearbyObject.gameObject.layer == LayerMask.NameToLayer("Player_KJS"))
                {
                    PlayerControler_KJS qw = player.gameObject.GetComponent<PlayerControler_KJS>();
                    qw.BumerAttack();
                }
                //pc.TakeDamage(explosionDamage); // 데미지 적용
                nearbyObject.gameObject.GetComponent<Human_KJS>().ApplyKnockBack(gameObject, false);
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
