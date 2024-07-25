using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_KJS : MonoBehaviour
{
    public float moveSpeed = 10;
    public float acceleration = 4f; // 가속도
    public float deceleration = 4f; // 감속도

    private CharacterController cc;
    private Vector3 velocity;

    public float jumPower = 3;
   
    float gravity = -9.81f;
    
    float yVelocity;

   
    public int jumpMaxcnt = 2;
    
    int jumpcurrCnt;

    // Start is called before the first frame update
    void Start()
    {
      
        cc = GetComponent<CharacterController>();
        velocity = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dirH = transform.right * h;                 
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        dir.Normalize();

        if (!cc.isGrounded)
        {
            yVelocity += gravity * Time.deltaTime;
        }
        else
        {
            yVelocity = -1f;
        }

        if (Input.GetButtonDown("Jump"))
       
        {
            if (cc.isGrounded)
            {   // 점프 실행
                yVelocity = jumPower;
            }
        }

        // 입력이 있을 시
        if (h != 0 || v != 0)
        {   // 가속을 주겠다
            velocity += dir * acceleration * Time.deltaTime;
        }
        else
        {
            // 감속
            velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * Time.deltaTime);
        }
        // 속력을 최고속력으로 제한함
        if (velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        
        // 움직임
        cc.Move((velocity * moveSpeed + Vector3.up * yVelocity) * Time.deltaTime);
    }
}
