using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Human_KJS;

public class JKYEnemyHPSystem : MonoBehaviour
{
    public Action<float, GameObject> getDamage;
    public bool isDead;

    public void GetDamage(float value, GameObject attacker)
    {
        getDamage(value, attacker);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //public void GetDamage(float value, GameObject attacker)
    //{
    //    if (humanState == HumanState.Dead) return;
    //    HP -= value;
    //    if (isPlayer)
    //    {
    //        gameObject.DamageAction();
    //    }
    //    if (slow != null) slow();
    //}
}
