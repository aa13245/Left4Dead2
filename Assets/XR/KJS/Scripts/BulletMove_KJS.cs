using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Rendering;
using UnityEngine;

public class BulletMove_KJS : MonoBehaviour
{
    public float speed = 10;
    //총알 효과 주소
    public GameObject bulletEffectFactory;
    public Rigidbody rb;

    // 적이 다른 물체와 충돌했으니까
    private void OnCollisionEnter(Collision collision)
    {
        //총알 효과 공장에서 총알 효과를 하나 만들어야한다.
        GameObject bulletEffect = Instantiate(bulletEffectFactory);

        //총알 이펙트의효과를 총돌한 자리에 발생시킨다
        bulletEffect.transform.position = transform.position;

        //gameobject를 비활성화 시킨다.
        gameObject .SetActive (false);

        // 물리력 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Component의 Rigidbody를 가져온다.
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
