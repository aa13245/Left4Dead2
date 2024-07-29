using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //플레이어 체력 변수
    public int hp = 20;
    //최대 체력 변수
    int maxHP = 20;
    //hp 슬라이더 변수
    public Slider hpSlider;
    //Hit 효과 오브젝트
    public GameObject hitEffect;

    // Start is called before the first frame update


    void Start()
    {
        cc = GetComponent<CharacterController>();
        velocity = Vector3.zero;

        //초기 체력 설정
        hp = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        //게임 상태가 '게임 중' 상태일때만 조작할 수 있게한다.
        if (GameManager_KJS.gm.gState != GameManager_KJS.GameState.Run)
        {
            return;
        }

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

        //현재 플레이어 hp를 hp슬라이더의 value에 반영한다.
        hpSlider.value = (float)hp / (float)maxHP;
    }

    //플레이어의 피격 함수
    public void DamageAction(int damage)
    {
        //에너미의 공격력만큼 플레이어의 체력을 깎는다.
        hp -= damage;
        //만일 플레이어의 체력이 0보다 크면 피격 효과를 출력한다.
        if(hp > 0)
        {
            //피격 이펙트 코루틴을 시작한다.
            StartCoroutine(PlayHitEffect());
        }
    }
    IEnumerator PlayHitEffect()
    {
        //피격 UI를 활성화 한다.
        hitEffect.SetActive(true);

        //0.3초간 대기한다.
        yield return new WaitForSeconds(0.3f);

        //피격 UI를 비활성화한다.
        hitEffect.SetActive(false);
    }

    public void Heal(int amount)
    {
        hp += amount;
        if(hp > maxHP)
        {
            hp = maxHP;
        }
    }
 }



