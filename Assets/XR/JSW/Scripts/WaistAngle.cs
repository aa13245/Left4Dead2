using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Human_KJS;

public class WaistAngle : MonoBehaviour
{
    Animator anim;
    Human_KJS human;
    float charRecoil;

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
        human = GetComponentInParent<Human_KJS>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
