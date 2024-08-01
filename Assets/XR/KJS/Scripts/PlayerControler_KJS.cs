using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler_KJS : MonoBehaviour
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

    public GameManager_KJS gameManager;

    //hp 슬라이더 변수
    public Slider hpSlider;
    //Hit 효과 오브젝트
    public GameObject hitEffect;
    // 플레이어 컴퍼넌트
    public Human_KJS human;
    // 인벤토리 컴포넌트
    public Inventory_JSW inventory;

    // Start is called before the first frame update


    void Start()
    {
        cc = GetComponent<CharacterController>();
        human = GetComponent<Human_KJS>();
        inventory = GetComponent<Inventory_JSW>();
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        // 클릭을 연속으로 받아야 하는 상황 - 자동 소총을 들고 있는 상황
        if (inventory.SlotNum == 0 &&
            ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.MainWeapon itemInfo &&
            itemInfo.isSniper == false && itemInfo.isShotgun == false)
        {
            if (Input.GetButton("Fire1"))
            {
                human.MouseClick();
                SlotUIChange();
            }
        }
        // 한번만 받아야 하는 상황
        else if (Input.GetButtonDown("Fire1"))
        {
            human.MouseClick();
            SlotUIChange();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            human.PickUp();
            SlotUIChange();
        }
        if (Input.GetKeyDown(KeyCode.G)) human.Drop();
        Swap();
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            SlotUIChange();
        }

        //현재 플레이어 hp를 hp슬라이더의 value에 반영한다.
        hpSlider.value = (float)human.HP / (float)human.maxHP;
    }
    void Move()
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
        cc.Move((velocity * moveSpeed + Vector3.up * yVelocity + human.knockBackVector) * Time.deltaTime);
    }
    void Swap()
    {
        //각 무기들에 배당된 숫자키
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.SlotNum = 0;
            SlotUIChange();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.SlotNum = 1;
            SlotUIChange();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.SlotNum = 2;
            SlotUIChange();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.SlotNum = 3;
            SlotUIChange();
        }
    }
    public void SlotUIChange()
    {
        // 현재 슬롯과 ui를 동기화
        int slotNum = inventory.SlotNum;

    }
    void Reload()
    {
        if (human.Reload())
        {   // 장전 성공

        }
        else
        {   // 장전 실패

        }
    }

    //플레이어의 피격 함수
    public void DamageAction()
    {
        //만일 플레이어의 체력이 0보다 크면 피격 효과를 출력한다.
        if(human.HP > 0)
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
    void SwitchPanel(int index)
    {
        if (GameManager_KJS.gm != null)
        {
            GameManager_KJS.gm.SwitchPanel(index);
        }
    }
}
 



