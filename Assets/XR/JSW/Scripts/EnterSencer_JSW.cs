using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSencer_JSW : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW")){
            other.transform.GetComponent<Human_KJS>().isEntered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
        {
            other.transform.GetComponent<Human_KJS>().isEntered = false;
        }
    }
}
