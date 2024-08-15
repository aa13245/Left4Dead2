using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static ItemTable_JSW;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class Human_KJS : MonoBehaviour
{
    public bool isPlayer;
    CharacterController cc;
    Animator anim;
    public Animator anim2;
    public Animator arAnim;
    public Animator hGAnim;
    
    public bool reloading;
    public GameObject bulletFactory;
    //총알 효과 주소
    public GameObject bulletEffectFactory;

    public float destroyTime = 1.5f;
    // Start is called before the first frame update
    public GameObject firePosition;
    //미사일을 담을 수 있는 배열
    public GameObject[] bulletArray;

    public float muzzleFlashInterval = 0.1f;  // 머즐 플래시 간격
    private float nextMuzzleFlashTime = 0f;   // 다음 머즐 플래시 생성 가능 시간

    public float fireRate = 0.1f;

    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;

    public GameObject bombFactory;

    public float throwPower = 15F;
    //무기를 담을 수 있는 배열
    public GameObject[] weapons;

    private float currTime;

    private GameObject currentWeapon;
    private ObjRotate_KJS objRotate;

    public AudioClip[] fireSounds;
    public AudioClip footSound;
    float footSoundTimer;
    public float speed;
    AudioSource audioSource;
    WaistAngle waistAngle;
    JKYshoot shoot;
    public bool isEntered;
    float minRecoil = 2;

    void PlayFireSound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex >= fireSounds.Length)
        {
            Debug.LogWarning("잘못된 사운드 인덱스입니다.");
            return;
        }

        if (fireSounds[soundIndex] != null)
        {
            audioSource.PlayOneShot(fireSounds[soundIndex]);
        }
    }

    public float MinRecoil
    {
        get { return minRecoil; }
        set 
        {
            minRecoil = value;
            recoil = value;
        }
    }
    float maxRecoil = 5;
    public float MaxRecoil
    {
        get { return maxRecoil; }
        set { maxRecoil = value; }
    }
    float recoil;
    public float Recoil
    {
        get { return recoil; }
        set
        {
            recoil = Mathf.Min(maxRecoil, value);
        }
    }

    //플레이어 체력 변수
    float hp = 100;
    public float HP
    {
        get { return hp; }
        set
        {   // 회복하는 경우
            if (hp <= value)
            {
                // 채워지는 양
                float v = value - hp;
                // 일시체력이 있을 때
                if (tempHP > 0)
                {
                    tempHP = Mathf.Max(0, tempHP - v);
                }
                hp = value;
            }
            else // 감소
            {
                // 감소하는 양
                float v = hp - value;
                if (tempHP > 0)
                {
                    tempHP = Mathf.Max(0, tempHP - v);
                }
                hp = value;
            }
        }
    }

    //최대 체력 변수
    public float maxHP = 100;
    public float knockedDownMaxHP = 300;
    // 일시적인 체력
    float tempHP;
    public float TempHP
    {
        get { return tempHP; }
        set { tempHP = value; }
    }
    bool isReloaing;
    public bool IsReloaing { get { return isReloaing; } }
    

    //PlayerMove_KJS player;
    public PlayerControler_KJS player;
    // 인벤토리 컴포넌트
    Inventory_JSW inventory;
    // 공격받을 때 슬로우걸리는 함수
    public Action slow;

    public enum HumanState
    {
        Normal,
        KnockedDown,
        Dead,

    }
    public HumanState humanState;
    public void ChangeHumanState(HumanState s)
    {
        if (humanState == s) return;
        humanState = s;
        // 부활
        if (s == HumanState.Normal)
        {
            HP = 30;
            TempHP = 29;
            if (isPlayer) player.SetKnockedDown(false);
        }
        // 기절
        else if (s == HumanState.KnockedDown)
        {
            HP = knockedDownMaxHP;
            TempHP = knockedDownMaxHP;
            Reload(false);
            anim.ResetTrigger("Revive");
            anim.CrossFade("KnockDown", 0.01f, 0);
            anim.CrossFade("KnockDown", 0.01f, 1);
            if (isPlayer) player.SetKnockedDown(true);
        }
        // 사망
        else if (s == HumanState.Dead)
        {
            Reload(false);
            anim.SetTrigger("Die");
        }
    }
    public enum InteractionState
    {
        None,
        Reviving,
        GetReviving,
        Healing,
        SelfHealing,
        GetHealing
    }
    public InteractionState interactionState;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        if (gameObject.name == "Player") isPlayer = true;
        player = GameObject.Find("Player").GetComponent<PlayerControler_KJS>();
        inventory = GetComponent<Inventory_JSW>();
        hp = maxHP;
        objRotate = Camera.main.GetComponent<ObjRotate_KJS>();
        //anim2 = transform.Find("FPSModel").GetComponent<Animator>();
        //배열을 이용해서 공간을 확보해라
        bulletArray = new GameObject[40];
        //배열을 채우자
        for (int i = 0; i < 40; i++)
        {
            //총알을 배송받는다.
            GameObject go = Instantiate(bulletFactory);
            //배송받은 총알을 채워 넣는다
            bulletArray[i] = go;
            //스위치를 끈다!(움직이지 않는다.)
            go.SetActive(false);
        }
        recoil = minRecoil;
        audioSource = GetComponent<AudioSource>();
        waistAngle = GetComponentInChildren<WaistAngle>();
        shoot = GetComponent<JKYshoot>();
    }
    // Update is called once per frame
    void Update()
    {
        if (currTime < 100) currTime += Time.deltaTime;
        KnockBackUpdate();
        ReloadUpdate();
        HpUpdate();
        InteractionStateUpdate();
        RecoilRecovery();
        FootSound();
    }
    float interactionTime;
    float interactionTimer;
    GameObject interactor;
    void FootSound()
    {
        if (speed > 0.01f)
        {
            footSoundTimer += Time.deltaTime;
            if (footSoundTimer > 1 / (speed * 0.5f))
            {
                audioSource.PlayOneShot(footSound, 1.5f);
                footSoundTimer = 0;
            }
        }
    }
    void InteractionStateUpdate()
    {
        if (interactionState == InteractionState.Reviving) // 팀 소생하는 중
        {
            Human_KJS targetComp = interactor.GetComponent<Human_KJS>();
            interactionTimer += Time.deltaTime;
            // 상호작용 UI 업데이트
            if (isPlayer)
            {
                player.InteractionSliderUpdate(interactionTimer / interactionTime);
            }
            else 
            {
                GetComponent<BotSight_JSW>().Rot(interactor.transform.position);
                if (targetComp.isPlayer) player.InteractionSliderUpdate(interactionTimer / interactionTime);
            }
            if (interactionTimer >= interactionTime)
            {   // 완료
                Revie(interactor, SetInteraction.Success);
            }
            else if (Vector3.Distance(transform.position, interactor.transform.position) > 3 || 
                (Input.GetKeyUp(KeyCode.E) && isPlayer) || humanState != HumanState.Normal || targetComp.humanState != HumanState.KnockedDown)
            {   // 취소
                Revie(interactor, SetInteraction.Cancel);
            }
        }
        else if (interactionState == InteractionState.GetReviving) // 소생 받는 중
        {

        }
        else if (interactionState == InteractionState.Healing) // 팀 회복해 주는 중
        {
            Human_KJS targetComp = interactor.GetComponent<Human_KJS>();
            interactionTimer += Time.deltaTime;
            // 상호작용 UI 업데이트
            if (isPlayer)
            {
                player.InteractionSliderUpdate(interactionTimer / interactionTime);
            }
            else 
            {
                GetComponent<BotSight_JSW>().Rot(interactor.transform.position);
                if (targetComp.isPlayer) player.InteractionSliderUpdate(interactionTimer / interactionTime);
            }
            if (interactionTimer >= interactionTime)
            {   // 완료
                Heal(interactor, SetInteraction.Success);
            }
            else if (Vector3.Distance(transform.position, interactor.transform.position) > 3 || 
                (Input.GetMouseButtonUp(1) && isPlayer) || humanState != HumanState.Normal || targetComp.humanState != HumanState.Normal)
            {   // 취소
                Heal(interactor, SetInteraction.Cancel);
            }
        }
        else if (interactionState == InteractionState.GetHealing) // 회복 받는 중
        {

        }
        else if (interactionState == InteractionState.SelfHealing) // 자힐 중
        {
            interactionTimer += Time.deltaTime;
            // 상호작용 UI 업데이트
            if (isPlayer)
            {
                player.InteractionSliderUpdate(interactionTimer / interactionTime);
            }
            if (interactionTimer >= interactionTime)
            {
                SelfHeal(SetInteraction.Success);
            }
            else if ((Input.GetMouseButtonUp(0) && isPlayer) || humanState != HumanState.Normal)
            {
                SelfHeal(SetInteraction.Cancel);
            }
        }
    }
    public enum SetInteraction
    {
        On, Cancel, Success
    }
    public void Revie(GameObject target, SetInteraction set)    
    {   
        Human_KJS targetComp = target.GetComponent<Human_KJS>();
        if (set == SetInteraction.On)
        {   // 자신 조건 체크
            if (humanState != HumanState.Normal || interactionState != InteractionState.None) return;
            // 대상 조건 체크
            if (targetComp.humanState == HumanState.KnockedDown && targetComp.interactionState == InteractionState.None)
            {
                Reload(false);
                interactionState = InteractionState.Reviving;
                interactor = target;
                targetComp.GetRevive(gameObject, SetInteraction.On);
                interactionTimer = 0;
                interactionTime = 5;
                if (isPlayer)
                {
                    // 소생 UI On 필요
                    player.InteractionUIEnable(true, "동료 소생");
                }
            }
        }
        else
        {
            interactionState = InteractionState.None;
            targetComp.GetRevive(gameObject, set);
            if (isPlayer)
            {
                // 소생 UI Off 필요
                player.InteractionUIEnable(false);
            }
        }
    }
    public void GetRevive(GameObject who, SetInteraction set)
    {
        if (set == SetInteraction.On)
        {
            interactionState = InteractionState.GetReviving;
            interactor = who;
            if (isPlayer)
            {
                // 소생 받는 UI On 필요
                player.InteractionUIEnable(true, "소생 중", "구원자: " + who.GetComponent<BotManager_JSW>().botName);
            }
            anim.SetTrigger("Revive");
        }
        else
        {
            if (set == SetInteraction.Success)
            {
                ChangeHumanState(HumanState.Normal);
                anim.SetTrigger("Revived");
            }
            else
            {
                anim.ResetTrigger("Revive");
                anim.CrossFade("KnockDown", 0.01f, 0);
                anim.CrossFade("KnockDown", 0.01f, 1);
            }
            interactionState = InteractionState.None;
            if (isPlayer)
            {
                // 소생 받는 UI Off 필요
                player.InteractionUIEnable(false);
            }
        }
    }
    public void Heal(GameObject target, SetInteraction set)
    {
        Human_KJS targetComp = target.GetComponent<Human_KJS>();
        if (set == SetInteraction.On)
        {
            if (humanState != HumanState.Normal || interactionState != InteractionState.None) return;
            if (targetComp.humanState == HumanState.Normal && targetComp.interactionState == InteractionState.None)
            {
                interactionState = InteractionState.Healing;
                interactor = target;
                targetComp.GetHeal(gameObject, SetInteraction.On);
                interactionTimer = 0;
                interactionTime = 8;
                if (isPlayer)
                {
                    // 회복 UI On 필요
                    player.InteractionUIEnable(true, "동료 치료", "대상자: " + target.GetComponent<BotManager_JSW>().botName);
                }
            }
        }
        else
        {
            if (set == SetInteraction.Success)
            {
                // 회복템 제거 필요
                inventory.Use(3);
                //
            }
            interactionState = InteractionState.None;
            targetComp.GetHeal(gameObject, set);
            if (isPlayer)
            {
                // 회복 UI Off 필요
                player.InteractionUIEnable(false);
            }
        }
    }
    public void GetHeal(GameObject who, SetInteraction set)
    {
        if (set == SetInteraction.On)
        {
            Reload(false);
            interactionState = InteractionState.GetHealing;
            interactor = who;
            if (isPlayer)
            {
                // 회복 받는 UI On 필요
                player.InteractionUIEnable(true, "치료 중", "치료자: " + who.GetComponent<BotManager_JSW>().botName);
            }
        }
        else
        {
            if (set == SetInteraction.Success)
            {
                float nowHp = HP - TempHP;
                HP = nowHp + (79.8f - (nowHp * 0.7976f));
            }
            interactionState = InteractionState.None;
            if (isPlayer)
            {
                // 회복 받는 UI Off 필요
                player.InteractionUIEnable(false);
            }
        }
    }
    void SelfHeal(SetInteraction set)
    {
        if (set == SetInteraction.On)
        {
            if (humanState != HumanState.Normal || interactionState != InteractionState.None) return;
            interactionState = InteractionState.SelfHealing;
            interactionTimer = 0;
            interactionTime = 8;
            if (isPlayer)
            {
                // 자힐 UI ON 필요
                player.InteractionUIEnable(true, "자가 치료");
            }
        }
        else
        {
            if (set == SetInteraction.Success)
            {
                // 회복템 제거 필요
                inventory.Use(3);
                //
                float nowHp = HP - TempHP;
                HP = nowHp + (79.8f - (nowHp * 0.7976f));
            }
            interactionState = InteractionState.None;
            if (isPlayer)
            {
                // 자힐 UI Off 필요
                player.InteractionUIEnable(false);
            }
        }
    }

    void HpUpdate()
    {   // 일시체력 감소
        if (tempHP > 0)
        {
            float value = Time.deltaTime * 0.1f;
            if (value > tempHP) value = tempHP;
            HP -= value;
        }
        // 기절
        if (humanState == HumanState.Normal)
        {
            if (HP <= 0)
            {
                ChangeHumanState(HumanState.KnockedDown);
            }
        }
        // 사망
        else if (humanState == HumanState.KnockedDown)
        {
            if (HP <= 0)
            {
                ChangeHumanState(HumanState.Dead);
                if (isPlayer) GameManager_KJS.gm.GameOver();
            }
        }
    }
    public void GetDamage(float value, GameObject attacker)
    {
        if (humanState == HumanState.Dead) return;
        HP -= value;
        if (isPlayer)
        {
            player.DamageAction();
        }
        if (slow != null) slow();
    }
    public void MouseClick(Vector3 origin = new Vector3(), Vector3 pos = new Vector3())
    {
        if (humanState == HumanState.Dead || (interactionState != InteractionState.GetHealing && interactionState != InteractionState.None && interactionState != InteractionState.GetReviving)) return;
        //유저가 Fire(LMB)을 클릭하면
        if (inventory[inventory.SlotNum] == null) return;
        // 주무기
        if (inventory.SlotNum == 0)
        {
            MainWeapon(origin, pos);
        }
        // 보조무기
        else if (inventory.SlotNum == 1)
        {
            SubWeapon(origin, pos);
        }
        // 투척
        else if (inventory.SlotNum == 2)
        {
            Projectile(pos);
        }
        // 힐템
        else if (inventory.SlotNum == 3)
        {
            Medikit();
        }
    }
    public void MouseRClick(GameObject target = null)
    {
        if (humanState == HumanState.Dead || interactionState != InteractionState.None) return;
        if (inventory[inventory.SlotNum] == null) return;
        if (inventory.SlotNum == 3)
        {
            if (target == null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 3, 1 << LayerMask.NameToLayer("Bot_JSW")))
                {
                    Medikit(hitInfo.transform.gameObject);
                }
            }
            else
            {
                Medikit(target);
            }
        }
    }
    Vector3 RandDir(float maxAngle, Vector3 dir)
    {
        float randomAngle = UnityEngine.Random.Range(0f, maxAngle);
        float randomAzimuth = UnityEngine.Random.Range(0f, 360f);
        return Quaternion.AngleAxis(randomAzimuth, dir) * (Quaternion.AngleAxis(randomAngle, Quaternion.Euler(new Vector3(0, 90, 0)) * dir) * dir);
    }
    void Bullet(Vector3 origin = new Vector3(), Vector3 dir = new Vector3())
    {
        //비활성화 되어잇는 총알을 찾아라(반복문)
        for (int i = 0; i < 40; i++)
        {
            //bulletArray[i]번째 총알이 비활성화 되어 있다면
            if (bulletArray[i].activeSelf == false)
            {
                {
                    //파이어 포지션의 위치에 옮겨놔라
                    bulletArray[i].transform.position = origin + dir.normalized * 0.5f;  // firePosition.transform.position;
                    // bullet의 각도를 카메라 방향으로 바꾸자
                    bulletArray[i].transform.up = dir;
                    //활성화 시켜라(스위치를 켜라)
                    bulletArray[i].SetActive(true);
                    bulletArray[i].GetComponent<BulletMove_KJS>().timer = 0;
                    //반복문을 종료해라
                    break;
                }
            }
        }
    }
    void MainWeapon(Vector3 origin, Vector3 dir)
    {
        if (humanState == HumanState.Dead) return;
        if (isReloaing) { return; }
        // 아이템 정보
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.MainWeapon itemInfo)
        {
            if (currTime >= itemInfo.fireRate)
            {
                // 장탄 확인
                if (inventory[inventory.SlotNum].value1 == 0)
                {   // 총알이 없을 때

                }
                else
                {
                    // 발사
                    currTime = 0f;
                    RaycastHit hitInfo;
                    Vector3 dirValue = (isPlayer ? Camera.main.transform.forward : dir);
                    void Fire()
                    {
                        Vector3 randDir = RandDir(recoil, dirValue);
                        if (Physics.Raycast(isPlayer ? Camera.main.transform.position : origin,
                                            randDir, out hitInfo, itemInfo.maxRange,
                                            ~((1 << LayerMask.NameToLayer(isPlayer ? "Player_KJS" : "Bot_JSW")) | (1 << LayerMask.NameToLayer("Ignore Raycast")))))
                        {
                            GameObject bullettEffect = Instantiate(bulletEffectFactory);
                            Destroy(bullettEffect, 3);
                            bullettEffect.transform.position = hitInfo.point;
                            bullettEffect.transform.forward = hitInfo.normal;
                            // 데미지 입히기
                            GiveDamage(TopObj(hitInfo.transform.gameObject), itemInfo.baseDmg);
                        }
                        Bullet(isPlayer ? Camera.main.transform.position : origin, randDir);
                    }
                    if (itemInfo.isShotgun) for (int i = 0; i < itemInfo.gauge; i++) Fire(); // 샷건일 경우 게이지 수 만큼 발사
                    else Fire();
                    if (isPlayer)
                    {
                        // 크로스헤어 흔들림 적용
                        FindObjectOfType<Crosshair_KJS>().TriggerCrosshairShake(0.05f, 5f); // 크로스헤어에 진동 적용
                        // 흔들림 효과 적용
                        ApplyShakeToCamera();
                      
                            if (objRotate != null)
                            {
                                objRotate.CreateMuzzleFlash();
                            }
                            nextMuzzleFlashTime = Time.time + muzzleFlashInterval;
                        
                    }
                    
                  

                    // 장탄 -
                    inventory.Use(inventory.SlotNum);

                    // 총기 반동 추가
                    Recoil += itemInfo.recoil;
                    //Fire 총 애니 이름 설정
                    string fireName = "Fire";
                    //총 쏘는 애니메이션 실행
                    if (isPlayer) anim2.CrossFade(fireName, 0.01f, 0, 0);

                    // 사운드 재생
                    PlayFireSound(0); // 첫 번째 사운드를 재생

                    if (waistAngle != null) waistAngle.CharRecoilSet(0.2f);

                }

            }

        }

    }
    void SubWeapon(Vector3 origin, Vector3 dir)
    {
        if (humanState == HumanState.Dead) return;
        if (isReloaing) { return; }

        // 권총이냐
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.SubWeapon itemInfo)
        {
            if (currTime >= itemInfo.fireRate)
            {
                // 장탄 확인
                if (inventory[inventory.SlotNum].value1 == 0)
                {
                    // 총알이 없을 때
                }
                else
                {
                    currTime = 0f;
                    RaycastHit hitInfo;
                    Vector3 dirValue = (isPlayer ? Camera.main.transform.forward : dir);
                    Vector3 randDir = RandDir(recoil, dirValue);
                    if (Physics.Raycast(isPlayer ? Camera.main.transform.position : origin,
                                        randDir, out hitInfo, itemInfo.maxRange,
                                        ~((1 << LayerMask.NameToLayer(isPlayer ? "Player_KJS" : "Bot_JSW")) | (1 << LayerMask.NameToLayer("Ignore Raycast")))))
                    {
                        GameObject bullettEffect = Instantiate(bulletEffectFactory);
                        Destroy(bullettEffect, 3);
                        bullettEffect.transform.position = hitInfo.point;
                        bullettEffect.transform.forward = hitInfo.normal;

                        // 데미지 입히기
                        GiveDamage(TopObj(hitInfo.transform.gameObject), itemInfo.baseDmg);
                    }
                    Bullet(isPlayer ? Camera.main.transform.position : origin, randDir);

                    if (isPlayer)
                    {
                        // 서브 웨폰 발사 시 화면 흔들림 효과 적용 (진동 크기 감소)
                        FindObjectOfType<ObjRotate_KJS>().TriggerShake(0.1f, 0.05f); // 작은 진동 크기 적용
                        // 크로스헤어 흔들림 적용
                        FindObjectOfType<Crosshair_KJS>().TriggerCrosshairShake(0.05f, 5f); // 크로스헤어에 진동 적용
                        if (objRotate != null)
                        {
                            objRotate.CreateMuzzleFlash();
                        }
                    }
                    inventory.Use(inventory.SlotNum);

                    // Fire 총 애니 이름 설정
                    string fireName = "Fire";
                    // 총 쏘는 애니메이션 실행
                    if (isPlayer) hGAnim.CrossFade(fireName, 0.01f, 0, 0);
                    // 총기 반동 추가
                    Recoil += itemInfo.recoil;

                    // 사운드 재생
                    PlayFireSound(1); // 첫 번째 사운드를 재생
                    if (waistAngle != null) waistAngle.CharRecoilSet(0.2f);
                }
            }

        }
        // 근접무기냐
        else if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.MeleeWeapon itemInfo2)
        {

        }
    }
    void RecoilRecovery()
    {
        recoil -= (recoil - minRecoil) * Time.deltaTime * 4;
    }
    void ApplyShakeToCamera()
    {
        if (objRotate != null)
        {
            objRotate.TriggerShake(shakeDuration, shakeMagnitude);
        }
    }

    void Projectile(Vector3 dir)
    {
        if (humanState == HumanState.Dead || isReloaing) return;

        // 현재 아이템이 수류탄인지 확인
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.Projectile itemInfo)
        {
            // 장탄 확인
            if (inventory[inventory.SlotNum].value1 <= 0)
            {
               
            }
            else
            {
                // 수류탄 생성
                GameObject projectile = Instantiate(bombFactory, firePosition.transform.position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce((isPlayer ? Camera.main.transform.forward : dir) * throwPower, ForceMode.Impulse);
                }

                // 수류탄의 데미지, 범위 및 지속 시간 설정
                float damage = itemInfo.dmg;
                float range = itemInfo.range;
                float duration = itemInfo.time;

                // BombAction_KJS 스크립트 가져오기
                BombAction_KJS bombScript = projectile.GetComponent<BombAction_KJS>();
                if (bombScript != null)
                {
                    // BombAction_KJS에 데미지, 범위, 지속 시간 전달
                    bombScript.Initialize(damage, range, duration);
                }
                    inventory.Use(2); // 투척 아이템 사용 및 슬롯 비우기
                
            }
        }
    }

    public void Medikit(GameObject target = null)
    {
        if (humanState == HumanState.Dead) return;
        // 회복템 사용
        if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.Recovery itemInfo)
        {   // 자힐
            if (target == null)
            {
                // 풀피가 아닐 때
                if (HP < 99)
                {
                    // 아이템 사용시간 구현 필요
                    //hp = itemInfo.value;
                
                    SelfHeal(SetInteraction.On);
                }
            }
            else
            {
                if (target.GetComponent<Human_KJS>().HP < itemInfo.value)
                {
                    Heal(target, SetInteraction.On);
                }
            }
        }
    }
    void GiveDamage(GameObject target, float dmg)
    {
        // 좀비 공격
        if (target.layer == LayerMask.NameToLayer("Enemy"))
        {
            target.GetComponent<JKYEnemyHPSystem>().GetDamage(dmg, gameObject);
            //shoot.ShootRagdoll(target);
        }
        // 아군 공격
        else if (target.layer == LayerMask.NameToLayer(isPlayer ? "Bot_JSW" : "Player_KJS"))
        {
            target.GetComponent<Human_KJS>().GetDamage(dmg/100, gameObject);
        }
    }
    void EquipWeapon(int slotNum)
    {
        if (inventory[slotNum] != null)
        {
            // 기존 무기 해제
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            // 새 무기 장착
            currentWeapon = Instantiate(inventory[slotNum].gameObject, firePosition.transform.position, firePosition.transform.rotation, firePosition.transform);
            currentWeapon.SetActive(true);
        }
    }
    public bool ChangeSlotNum(int slotNum)
    {
        if (humanState != HumanState.Normal || interactionState != InteractionState.None) return false;
        if (inventory.SetSlotNum(slotNum))
        {
            Reload(false);
            return true;
        }
        else return false;
    }
    public void Interact(GameObject target = null, int layer = 0)
    {
        if (humanState != HumanState.Normal || interactionState != InteractionState.None) return;
        // 플레이어 일 때
        if (target == null)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 3,
                ~(1 << LayerMask.NameToLayer("Player_KJS"))))
            {   // 대상이 아이템
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Item_JSW"))
                {
                    inventory.PickUp(hitInfo.transform.gameObject);
                }
                // 대상이 기절한 아군
                else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Bot_JSW"))
                {
                    Revie(hitInfo.transform.gameObject, SetInteraction.On);
                }
                // 오브젝트와 상호작용
                else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("InteractObj"))
                {
                    TopObj(hitInfo.transform.gameObject).GetComponent<InteractObj_JSW>().Interact();
                }
            }

        }
        // 봇일 때
        else
        {
            if (layer == LayerMask.NameToLayer("Item_JSW"))
            {
                inventory.PickUp(target);
            }
            else if (layer == LayerMask.NameToLayer("Bot_JSW") || layer == LayerMask.NameToLayer("Player_KJS"))
            {
                Revie(target, SetInteraction.On);
            }
        }
    }
    public void Drop()
    {
        inventory.Drop(inventory.SlotNum);
    }
    float reloadTime;
    float reloadTimer;
    public void Reload(bool on)
    {
        if (humanState == HumanState.Dead || (interactionState != InteractionState.None && interactionState != InteractionState.GetHealing && interactionState != InteractionState.GetReviving)) return;
        if (on)
        {
            if (!isReloaing && inventory.CheckReloadEnable(inventory.SlotNum))
            {   // 주무기일 때
                if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.MainWeapon mainWeapon)
                {   // 장전 On
                    reloadTimer = 0;
                    isReloaing = true;
                    reloadTime = mainWeapon.reloadSpeed;
                    PlayFireSound(2); // 3 번째 사운드를 재생

                    if (isPlayer) anim2.SetTrigger("AR_Reload");
                    if (isPlayer) arAnim.SetTrigger("AR_Reload");
                    anim.CrossFade("Reloaing_Main", 0.01f, 1);
                }
                else if (ItemTable_JSW.instance.itemTable[inventory[inventory.SlotNum].kind] is ItemTable_JSW.SubWeapon subWeapon)
                {   // 장전 On
                    reloadTimer = 0;
                    isReloaing = true;
                    reloadTime = subWeapon.reloadSpeed;
                    PlayFireSound(3); // 4 번째 사운드를 재생
                    if (isPlayer) hGAnim.SetTrigger("HG_Reload");
                    anim.CrossFade("Reloaing_Sub", 0.01f, 1);
                }
            }
        }
        else
        {
            if (isReloaing)
            {   // 장전 Off
                isReloaing = false;
                anim.ResetTrigger("Reloaing_Main");
                anim.ResetTrigger("Reloaing_Sub");
            }
        }
    }
    void ReloadUpdate()
    {
        if (!isReloaing || humanState == HumanState.Dead) return;
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime)
        {   // 장전완료
            isReloaing = false;
            inventory.Reload(inventory.SlotNum);
            if (isPlayer) player.SlotUIChange();
        }
    }
    GameObject TopObj(GameObject obj)
    {
        GameObject topObj = obj;
        while (topObj.transform.parent != null && topObj.transform.parent.gameObject.layer == obj.layer)
        {
            topObj = topObj.transform.parent.gameObject;
        }
        return topObj;
    }
    public Action stun;
    public void Stun(GameObject stone)
    {
        if (humanState == HumanState.Dead) return;
        stun();
    }
    float knockBackPow = 30;
    float boomerKnockBackPow = 9;
    public Vector3 knockBackVector = Vector3.zero;
    public void ApplyKnockBack(GameObject zombie, bool tank)
    {
        if (humanState == HumanState.Dead) return;
        if (tank)
        {   // 탱크 넉백
            Vector3 dir = transform.position - zombie.transform.position + Vector3.up * 0.7f;
            dir.Normalize();
            knockBackVector = dir * knockBackPow;
        }
        else
        {   // 부머 넉백
            Vector3 dir = transform.position - zombie.transform.position;
            dir.Normalize();
            knockBackVector = dir * boomerKnockBackPow;
        }
    }
    void KnockBackUpdate()
    {
        knockBackVector -= knockBackVector * 1 * Time.deltaTime;
    }
    void ChangeAnimatorController(RuntimeAnimatorController controller)
    {
        if (anim2.runtimeAnimatorController == controller) return;

        RuntimeAnimatorController previousController = anim2.runtimeAnimatorController;

        // 애니메이터 교체
        anim2.runtimeAnimatorController = controller;

        // 현재 애니메이션 상태 유지
        AnimatorStateInfo currentAnimatorStateInfo = anim2.GetCurrentAnimatorStateInfo(0);
        anim2.Play(currentAnimatorStateInfo.fullPathHash, 0, currentAnimatorStateInfo.normalizedTime);

        // 트리거, 파라미터, 그리고 상태 복구
        AnimatorControllerParameter[] parameters = anim2.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter param = parameters[i];
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    anim2.SetBool(param.name, anim2.GetBool(param.name));
                    break;
                case AnimatorControllerParameterType.Float:
                    anim2.SetFloat(param.name, anim2.GetFloat(param.name));
                    break;
                case AnimatorControllerParameterType.Int:
                    anim2.SetInteger(param.name, anim2.GetInteger(param.name));
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if (anim2.GetBool(param.name))
                        anim2.SetTrigger(param.name);
                    break;
            }
        }
    }
    
}

