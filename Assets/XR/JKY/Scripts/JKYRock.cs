using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYRock : MonoBehaviour
{
    public GameObject py;
    
    // Start is called before the first frame update
    void Start()
    {
        py = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject, 1);
        }
    }
    public float dmg = 10;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
        {
            print("스턴되면서 카메라흔들림");
            Human human = other.GetComponent<Human>();
            human.Stun(gameObject);
            human.GetDamage(dmg, gameObject);
            
            //StunPlayers(other.gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject, 1);
            print("없앳다");
            
        }
    }


    public void StunPlayers(GameObject py)
    {
        //py.GetComponent<Human_KJS>().stun
        //if (py.GetComponent<Human_KJS>().stun && P1stun == false)
        //{
        //    P1stun = true;
        //    PlayerMove playerMove = py.GetComponent<PlayerMove>();
        //    StartCoroutine(playerMove.Stun(1.0f, transform.position));
        //}
        //else if (py.GetComponent<PlayerMove2>() && P2stun == false)
        //{
        //    P2stun = true;
        //    PlayerMove2 playerMove2 = py.GetComponent<PlayerMove2>();
        //    StartCoroutine(playerMove2.Stun(1.0f, transform.position));
        //}
        //else if (py.GetComponent<JShellyFSM>() && P3stun == false)
        //{
        //    P3stun = true;
        //    JShellyFSM JShellyfsm = py.GetComponent<JShellyFSM>();
        //    StartCoroutine(JShellyfsm.Stun(1.0f, transform.position));
        //}


    }
}
