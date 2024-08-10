using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Rendering;
using UnityEngine;

public class BulletMove_KJS : MonoBehaviour
{
    public float speed = 100;
    //총알 효과 주소
    public Rigidbody rb;
    public float timer;

    // 적이 다른 물체와 충돌했으니까
    private void OnCollisionEnter(Collision collision)
    {
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
        timer += Time.deltaTime;
        if (timer > 1)
        {
            gameObject.SetActive(false);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
