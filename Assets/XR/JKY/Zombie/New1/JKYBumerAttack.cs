using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class JKYBumerAttack : MonoBehaviour
{
    public float attackPower = 7f;
    public float currTime;
    public float damageInterval = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Human_KJS>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
        {
            print("독뎀11");
            currTime += Time.deltaTime;
            if(currTime > damageInterval)
            {
                other.GetComponent<Human_KJS>().GetDamage(attackPower, gameObject);
                currTime = 0;

            }
            if(currTime > 9f)
            {
                Destroy(gameObject);
            }
        }
    }

    

}
