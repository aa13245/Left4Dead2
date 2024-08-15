using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class JKYBumerAttack : MonoBehaviour
{
    public float attackPower = 7f;
    public float currTime;
    public float damageInterval = 2f;
    public GameObject player;
    public Material mat;
    [SerializeField] FullScreenPassRendererFeature full;
    //[SerializeField] 
    //public UniversalRendererData data;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Player_KJS"))// || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
    //    {
    //        // PlayerControler_KJS qw = player.gameObject.GetComponent<PlayerControler_KJS>();
    //        // qw.BumerAttack();
    //        PlayerControler_KJS qw = player.gameObject.GetComponent<PlayerControler_KJS>();
    //        qw.BumerAttack();
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS"))// || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
        {
            print("들어왔다>?");
            // PlayerControler_KJS qw = player.gameObject.GetComponent<PlayerControler_KJS>();
            // qw.BumerAttack();
            PlayerControler_KJS qw = player.gameObject.GetComponent<PlayerControler_KJS>();
            qw.BumerAttack();
        }
    }


}
