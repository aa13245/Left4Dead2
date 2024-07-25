using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class JKYPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public CharacterController cc;

    // 점프파워
    public float jumpPower = 3;
    // 중력
    float gravity = -9.81f;
    // y 방향 속력
    float yVelocity;

    // 최대 점프 횟수
    public int jumpMaxCnt = 2;
    // 현재 점프 횟수
    int jumpCurrCnt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //Vector3 DirH = transform.right * h;     //Vector3.right = 1, 0, 0
        //transform.right = 0, 0 ,1
        //Vector3 DirV = transform.forward * v;
        // Vector3 dir = DirH + DirV;

        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;
        dir = Camera.main.transform.TransformDirection(dir);
        //dir.Normalize();
        if (cc.isGrounded)
        {
            yVelocity = 0;
            jumpCurrCnt = 0;
        }
        // 만약에 스페이스바를 누르면
        //if(Input.GetButtonDown("Jump"))
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 만약에 현재 점프 횟수가 최대 점프 횟수 보다 작으면
            if (jumpCurrCnt < jumpMaxCnt)
            {
                // yVelocity 에 jumpPower를 셋팅
                yVelocity = jumpPower;
                // 현재 점프 횟수를 증가시키자.\
                jumpCurrCnt++;

            }


        }


        // yVelocity 를 중력값을 이용해서 감소시킨다.
        // v = v0 + at;
        yVelocity += gravity * Time.deltaTime;
        // dir.y 값에 yvelocity를 셋팅
        dir.y = yVelocity;
        // 3. 그 방향으로 움직이자. (P = P0 + vt)
        //transform.position += dir * moveSpeed * Time.deltaTime;
        cc.Move(dir * moveSpeed * Time.deltaTime);


        
    }

    //플레이어 피격함수
    public void DamageAction(int damage)
    {
        //hp -= damage;
    }
}
