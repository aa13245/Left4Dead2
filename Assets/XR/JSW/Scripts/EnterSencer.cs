using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 헬기 입장 체크
public class EnterSencer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW")){
            other.transform.GetComponent<Human>().isEntered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS") || other.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
        {
            other.transform.GetComponent<Human>().isEntered = false;
        }
    }
}
