using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYPlayerFire : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bulletEffect;

    ParticleSystem ps;
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            //레이가 부디ㅈㅎㅣㄴ 대사ㅇㅇㅢ 정ㅂㅗㄹㅡㄹ 저자ㅇㅎㅏㄹ 변ㅅㅜ
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(ray, out hitInfo))
            {
                bulletEffect.transform.position = hitInfo.point;
                bulletEffect.transform.forward = hitInfo.normal;
                ps.Play(); ;
            }
        }
    }
}
