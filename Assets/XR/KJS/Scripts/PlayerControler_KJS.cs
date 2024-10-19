using System.Collections;
//using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler_KJS : MonoBehaviour
{
    public float moveSpeed = 6;
    public float acceleration = 30; // 가속도
    public float deceleration = 30; // 감속도

    private CharacterController cc;
    private Vector3 velocity;

    public float jumPower = 3;
    float gravity = -9.81f;
    float yVelocity;
    public int jumpMaxcnt = 2;
    int jumpcurrCnt;

    // UI 관련 변수
    Slider hpSlider;
    Image hpImage;
    Slider tempHpSlider;
    Image tempHpImage;
    Text hpText;
    GameObject interactionUI;
    Slider interactionSlider;
    GameObject interactionIcons;
    Text interactionText1;
    Text interactionText2;
    public GameObject hitEffect;
    public Human human;
    public Inventory inventory;
    public Animator anim;

    // 새로운 변수 추가: 현재 슬롯 번호를 추적
    private int currentSlotNum = 0;

    void Start()
    {
        colorScale = mat.GetFloat("_ColorScale");

        // 기존 초기화 코드
        cc = GetComponent<CharacterController>();
        human = GetComponent<Human>();
        inventory = GetComponent<Inventory>();
        GameObject canvas = GameObject.Find("Canvas");
        hpSlider = canvas.transform.Find("Info0/HPbar").GetComponent<Slider>();
        hpImage = canvas.transform.Find("Info0/HPbar/Fill Area/Fill").GetComponent<Image>();
        tempHpSlider = canvas.transform.Find("Info0/TempHPbar").GetComponent<Slider>();
        tempHpImage = canvas.transform.Find("Info0/TempHPbar/Fill Area/Fill").GetComponent<Image>();
        hpText = canvas.transform.Find("Info0/Text (Legacy)").GetComponent<Text>();
        hitEffect = canvas.transform.Find("Hit").gameObject;
        interactionUI = canvas.transform.Find("InteractionStatus").gameObject;
        interactionSlider = interactionUI.transform.Find("Slider").GetComponent<Slider>();
        interactionIcons = interactionUI.transform.Find("Icons").gameObject;
        interactionText1 = interactionUI.transform.Find("Text1").GetComponent<Text>();
        interactionText2 = interactionUI.transform.Find("Text2").GetComponent<Text>();
        velocity = Vector3.zero;
        anim = transform.Find("Ch28_nonPBR").GetComponent<Animator>();
        GetComponent<Human>().slow = Slow;
        GetComponent<Human>().stun = Stun;
    }



    void Update()
    {
        Move();
        HandleInput();
        Swap(); // 슬롯 변경 코드 호출
        HpUiUpdate();
        CamRecovery();
        human.speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
    }

    void HandleInput()
    {

        // 클릭을 연속으로 받아야 하는 상황 - 자동 소총을 들고 있는 상황
        if (inventory.SlotNum == 0 &&
            ItemTable.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable.MainWeapon itemInfo &&
            itemInfo.isSniper == false && itemInfo.isShotgun == false)
        {

            if (Input.GetButton("Fire1"))
            {
                human.MouseClick();
                
            }
        }
        // 한번만 받아야 하는 상황
        else if (Input.GetButtonDown("Fire1"))
        {
            
            human.MouseClick();
            

        }
        // 마우스 우클릭
        if (Input.GetMouseButtonDown(1))
        {
            human.MouseRClick();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            human.Interact();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            human.Reload(true);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            human.Drop();
        }
    }

    void Swap()
    {
        // 숫자키 입력에 따른 슬롯 변경
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSlot(3);
        }

        // 마우스 휠 입력에 따른 슬롯 변경
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) // 휠을 위로 스크롤
        {
            ChangeSlot((currentSlotNum + 1) % 4);
        }
        else if (scroll < 0f) // 휠을 아래로 스크롤
        {
            ChangeSlot((currentSlotNum - 1 + 4) % 4);
        }
    }

    void ChangeSlot(int newSlotNum)
    {
        if (currentSlotNum != newSlotNum)
        {
            currentSlotNum = newSlotNum;
            if (human.ChangeSlotNum(currentSlotNum))
            {
                SlotUIChange();
            }
        }
    }

    public void SlotUIChange()
    {
        // 현재 슬롯과 UI를 동기화
        int slotNum = inventory.SlotNum;
    }

    void HpUiUpdate()
    {
        // 현재 플레이어 hp를 hp슬라이더의 value에 반영
        hpSlider.value = (human.HP - human.TempHP) / (human.humanState == Human.HumanState.KnockedDown ? human.knockedDownMaxHP : human.maxHP);
        tempHpSlider.value = human.HP / (human.humanState == Human.HumanState.KnockedDown ? human.knockedDownMaxHP : human.maxHP);
        hpText.text = ((int)human.HP).ToString();
        if (human.humanState == Human.HumanState.Normal)
        {
            Color c = Color.HSVToRGB(Mathf.Lerp(0, 0.3392157f, hpSlider.value), 1, 1);
            c.a = 1;
            hpImage.color = c;
            hpText.color = c;
            c.a = 0.5f;
            tempHpImage.color = c;
        }
        else if (human.humanState == Human.HumanState.KnockedDown)
        {
            Color c = Color.HSVToRGB(0, 1, 1);
            c.a = 1;
            hpImage.color = c;
            hpText.color = c;
            c.a = 0.5f;
            tempHpImage.color = c;
        }
    }

    public void Slow()
    {
        velocity = Vector3.zero;
    }

    bool stun;
    public void Stun()
    {
        if (human.humanState == Human.HumanState.Normal)
        {
            StartCoroutine(StunWait());
        }
    }

    IEnumerator StunWait()
    {
        stun = true;
        Slow();
        Camera.main.transform.Rotate(new Vector3(0, 0, -10));
        yield return new WaitForSeconds(1);
        stun = false;
    }

    void CamRecovery()
    {
        if (human.humanState == Human.HumanState.Normal)
        {
            Camera.main.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 3 * Mathf.DeltaAngle(Camera.main.transform.localEulerAngles.z, 0)));
        }
    }

    public void SetKnockedDown(bool on)
    {
        if (on)
        {
            Camera.main.transform.Rotate(new Vector3(0, 0, Mathf.DeltaAngle(Camera.main.transform.localEulerAngles.z, 0) - 10));
            GetComponent<ObjRotate_KJS>().knockedCamOffset = true;
        }
        else
        {
            Camera.main.transform.Rotate(new Vector3(0, 0, Mathf.DeltaAngle(Camera.main.transform.localEulerAngles.z, 0)));
            GetComponent<ObjRotate_KJS>().knockedCamOffset = false;
        }
    }

    void Move()
    {
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

        if (cc.isGrounded)
        {
            yVelocity = -1;
            jumpcurrCnt = 0;
        }

        if (Input.GetButtonDown("Jump") && cc.isGrounded && human.humanState == Human.HumanState.Normal && human.interactionState == Human.InteractionState.None)
        {
            yVelocity = jumPower;
            jumpcurrCnt++;
        }

        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        if ((h != 0 || v != 0) && !stun && human.humanState == Human.HumanState.Normal && human.interactionState == Human.InteractionState.None)
        {
            velocity += dir * acceleration * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        if (velocity.magnitude > moveSpeed)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        Vector3 moveVector = Quaternion.Euler(Vector3.down * transform.eulerAngles.y) * velocity;
        anim.SetFloat("MoveZ", moveVector.z);
        anim.SetFloat("MoveX", moveVector.x);

        velocity.y = 0;

        cc.Move(((!stun && human.humanState == Human.HumanState.Normal ? velocity : Vector3.zero) + Vector3.up * yVelocity + human.knockBackVector) * Time.deltaTime);
    }

    void Reload()
    {
        int currentSlot = inventory.SlotNum;
        ItemStatus currentItem = inventory[currentSlot];

        if (currentItem != null)
        {
            ItemTable.Items itemKind = currentItem.kind;
            if (ItemTable.instance.itemTable.TryGetValue(itemKind, out var itemObject))
            {
                float reloadSpeed = 0f;

                if (itemObject is ItemTable.MainWeapon mainWeapon)
                {
                    reloadSpeed = mainWeapon.reloadSpeed;
                }
                else if (itemObject is ItemTable.SubWeapon subWeapon)
                {
                    reloadSpeed = subWeapon.reloadSpeed;
                }
                else
                {
                    Debug.Log("현재 아이템은 재장전 속도가 없는 아이템입니다.");
                    return;
                }

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
        Debug.Log("재장전 중... 속도: " + reloadTime + "초");

        yield return new WaitForSeconds(reloadTime);

        Debug.Log("재장전 완료.");

        if (inventory.SlotNum == 0)
        {
            ItemStatus item = inventory[0];
            if (item != null && ItemTable.instance.itemTable.TryGetValue(item.kind, out var itemObject))
            {
                if (itemObject is ItemTable.MainWeapon mainWeapon)
                {
                    item.value1 = mainWeapon.magazineCapacity;
                }
            }
        }
        else if (inventory.SlotNum == 1)
        {
            ItemStatus item = inventory[1];
            if (item != null && ItemTable.instance.itemTable.TryGetValue(item.kind, out var itemObject))
            {
                if (itemObject is ItemTable.SubWeapon subWeapon)
                {
                    item.value1 = subWeapon.magazineCapacity;
                }
            }
        }
    }

    public void DamageAction()
    {
        if (human.HP > 0)
        {
            StartCoroutine(PlayHitEffect());
        }
    }

    IEnumerator PlayHitEffect()
    {
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitEffect.SetActive(false);
    }
    void SwitchPanel(int index)
    {
        if (GameManager_KJS.gm != null)
        {
            GameManager_KJS.gm.SwitchPanel(index);
        }
    }

    public void InteractionSliderUpdate(float _value)
    {
        interactionSlider.value = _value;
    }

    public void InteractionUIEnable(bool enable, string text1 = "", string text2 = "")
    {
        interactionUI.SetActive(enable);
        if (enable)
        {
            interactionSlider.value = 0;
            interactionText1.text = text1;
            interactionText2.text = text2;
            if (text1.Contains("치료"))
            {
                interactionIcons.transform.GetChild(0).gameObject.SetActive(true);
                interactionIcons.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                interactionIcons.transform.GetChild(0).gameObject.SetActive(false);
                interactionIcons.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }


    [SerializeField] FullScreenPassRendererFeature full;
    public Material mat;
    public float lerpDuration = 15f; // Lerp의 지속 시간
    private float lerpTime = 0f;    // Lerp 시간 계산
    private float startValue = 1f;  // 시작 색상 비율
    private float endValue = -0.2f; // 종료 색상 비율
    private float elapsedTime;
    bool check = false;
    float colorScale;
    public void BumerAttack()
    {
        //print("emfd");
        StartCoroutine(Bumer());
    }
    public AudioClip boomerBgm;
    public bool boomerEft;
    IEnumerator Bumer()
    {
        float elapsedTime = 0f;
        yield return new WaitForSeconds(0.1f);
        mat.SetFloat("_ColorScale", 1f);
        //colorScale = 1f;
        full.SetActive(true);
        if (!boomerEft)
        {
            boomerEft = true;
            human.audioSource.PlayOneShot(boomerBgm);
        }

        yield return new WaitForSeconds(2f);
        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpProgress = elapsedTime / lerpDuration;

            // Lerp 계산
            float lerpedValue = Mathf.Lerp(startValue, endValue, lerpProgress);

            // Material의 속성 업데이트
            if (mat != null)
            {
                mat.SetFloat("_ColorScale", lerpedValue);
            }

            // 다음 프레임까지 대기
            yield return null;
        }
        // 마지막 값으로 설정 (Lerp 종료 시)
        if (mat != null)
        {
            mat.SetFloat("_ColorScale", endValue);
            full.SetActive(false);
            boomerEft = false;
        }



        //lerpTime += Time.deltaTime / lerpDuration;

        //float lerpedValue = Mathf.Lerp(startValue, endValue, lerpTime);

        //// Material의 속성 업데이트
        //if (mat != null)
        //{
        //    mat.SetFloat("_ColorScale", lerpedValue);
        //}



        //if (colorScale < -0.2f)
        //{
        //    // Lerp 완료 시, 시간과 색상 비율 초기화
        //    if (lerpTime >= 1f)
        //    {
        //        lerpTime = 1f;
        //        full.SetActive(false);
        //        enabled = false;  // 스크립트 비활성화 (옵션)
        //        mat.SetFloat("_ColorScale", 1f);

        //    }
        //}
    }
    private void OnDestroy()
    {
        full.SetActive(false);
    }
    public void GameEnd()
    {
        full.SetActive(false);
    }
}