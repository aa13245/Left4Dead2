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

    //hp 슬라이더 변수
    public Slider hpSlider;
    //Hit 효과 오브젝트
    public GameObject hitEffect;
    // 플레이어 컴퍼넌트
    public Human_KJS human;
    // 인벤토리 컴포넌트
    public Inventory_JSW inventory;

    // Start is called before the first frame update

    //애니메이터 변수
    Animator anim;

    void Start()
    {
        //피격 이벤트 오브젝트에서 파티클 시스템 컴포넌트 가져오기
        cc = GetComponent<CharacterController>();
        human = GetComponent<Human_KJS>();
        inventory = GetComponent<Inventory_JSW>();
        GameObject canvas = GameObject.Find("Canvas");
        hpSlider = canvas.transform.Find("HPbar").GetComponent<Slider>();
        hitEffect = canvas.transform.Find("Hit").gameObject;
        velocity = Vector3.zero;
        anim = GetComponentInChildren<Animator>();
        GetComponent<Human_KJS>().slow = Slow;
        GetComponent<Human_KJS>().stun = Stun;
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
        if (Input.GetKeyDown(KeyCode.B)) Stun();
        CamRecovery();
    }
    // 맞았을 때 감속
    public void Slow()
    {
        velocity = Vector3.zero;
    }
    // 탱크 돌 스턴
    bool stun;
    public void Stun()
    {
        StartCoroutine(StunWait());
    }
    IEnumerator StunWait()
    {
        stun = true;
        Slow();
        // 카메라 회전
        Camera.main.transform.Rotate(new Vector3(0, 0, -10));
        yield return new WaitForSeconds(1);
        stun = false;
    }
    void CamRecovery()
    {
        Camera.main.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 3 * Mathf.DeltaAngle(Camera.main.transform.localEulerAngles.z, 0)));
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

        //이동 블렌딩 트리를 호출하고 벡터의 크기 값을 넘겨준다
        //anim.SetFloat("MoveMotion", dir.magnitude);

        if (cc.isGrounded)
        {
            yVelocity = 0;
            jumpcurrCnt = 0;
        }

        // 만약에 스페이스 바를 누르면
        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            // yVelocity에 jumpPower를 셋팅
            yVelocity = jumPower;
            // 현재 점프횟수를 증가 시키자
            jumpcurrCnt++;
        }

        // yVelocity를 중력값을 이용해서 감소시킨다.
        // v = v0 + at;
        yVelocity += gravity * Time.deltaTime;

        // dir.y값에 yVelocity를 셋팅
        dir.y = yVelocity;

        // 이동 적용
        cc.Move(dir * Time.deltaTime);

        // 입력이 있을 시
        if ((h != 0 || v != 0) && !stun)
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
        Vector3 moveVector = Quaternion.Euler(Vector3.down * transform.eulerAngles.y) * velocity;
        anim.SetFloat("MoveZ", moveVector.z);
        anim.SetFloat("MoveX", moveVector.x);

        // 움직임
        cc.Move((stun ? Vector3.zero : velocity + Vector3.up * yVelocity + human.knockBackVector) * Time.deltaTime);
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
        int currentSlot = inventory.SlotNum; // 현재 선택된 슬롯
        Item_JSW currentItem = inventory[currentSlot]; // 현재 슬롯의 아이템

        if (currentItem != null)
        {
            // 아이템의 종류를 확인하여 `ItemTable_JSW`에서 정보를 가져옴
            ItemTable_JSW.Items itemKind = currentItem.kind;
            if (ItemTable_JSW.instance.itemTable.TryGetValue(itemKind, out var itemObject))
            {
                float reloadSpeed = 0f;

                if (itemObject is ItemTable_JSW.MainWeapon mainWeapon)
                {
                    reloadSpeed = mainWeapon.reloadSpeed;
                }
                else if (itemObject is ItemTable_JSW.SubWeapon subWeapon)
                {
                    reloadSpeed = subWeapon.reloadSpeed;
                }
                else
                {
                    Debug.Log("현재 아이템은 재장전 속도가 없는 아이템입니다.");
                    return;
                }

                // 재장전 애니메이션이나 효과를 여기에 추가할 수 있습니다.
                StartCoroutine(ReloadCoroutine(reloadSpeed));
            }
            else
            {
                Debug.Log("아이템 정보를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log("현재 슬롯에 아이템이 없습니다.");
        }
    }

    IEnumerator ReloadCoroutine(float reloadTime)
    {
        // 재장전 애니메이션이나 효과를 여기에 추가할 수 있습니다.
        Debug.Log("재장전 중... 속도: " + reloadTime + "초");

        // 실제 재장전 처리 시간
        yield return new WaitForSeconds(reloadTime);

        // 재장전 완료 후 처리
        // 예를 들어, 총알 수를 다시 채우거나 상태를 업데이트합니다.
        Debug.Log("재장전 완료.");

        // 실제 재장전 후 처리 코드 추가
        // 예: 총알 수를 다시 채우기
        if (inventory.SlotNum == 0) // 주무기 슬롯을 가정
        {
            Item_JSW item = inventory[0];
            if (item != null && ItemTable_JSW.instance.itemTable.TryGetValue(item.kind, out var itemObject))
            {
                if (itemObject is ItemTable_JSW.MainWeapon mainWeapon)
                {
                    // 총알 수를 최대값으로 리셋
                    item.value1 = mainWeapon.magazineCapacity;
                }
            }
        }
        else if (inventory.SlotNum == 1) // 보조무기 슬롯을 가정
        {
            Item_JSW item = inventory[1];
            if (item != null && ItemTable_JSW.instance.itemTable.TryGetValue(item.kind, out var itemObject))
            {
                if (itemObject is ItemTable_JSW.SubWeapon subWeapon)
                {
                    // 총알 수를 최대값으로 리셋
                    item.value1 = subWeapon.magazineCapacity;
                }
            }
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
 



