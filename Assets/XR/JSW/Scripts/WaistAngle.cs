using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Human;

public class WaistAngle : MonoBehaviour
{
    Animator anim;
    Human human;
    float charRecoil;
    // 봇 캐릭터 반동
    public void CharRecoilSet(float value)
    {
        charRecoil = value;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        charRecoil -= charRecoil * Time.deltaTime;
        if (human.humanState == HumanState.Normal && human.interactionState == InteractionState.None)
        {
            anim.SetLookAtWeight(charRecoil, charRecoil, charRecoil);
            Vector3 lookPos = transform.parent.position + transform.parent.forward + Vector3.up * 2;
            anim.SetLookAtPosition(lookPos);
        }
        else anim.SetLookAtWeight(0,0,0);
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        human = GetComponentInParent<Human>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
