using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject main;            // 메인 무기
    public GameObject sub;             // 서브 무기
    public GameObject pro;             // 프로젝타일 무기
    public GameObject mediKitPrefab;   // 메디킷

    private GameObject currentWeapon;
    private int currentWeaponIndex = -1;

    Inventory_JSW inventory;

    // FPS 모델 관련
    public GameObject fpsModel1;     // FPSModel1
    public GameObject fpsModel2;     // FPSModel2

    // medikit과 projectile 모델
    private GameObject medikitObject;
    private GameObject projectileObject;
    private bool hasThrownProjectile = false; // 프로젝타일이 이미 투척되었는지 여부를 추적

    void Start()
    {
        inventory = GetComponentInParent<Inventory_JSW>();
        currentWeaponIndex = 1; // 초기 무기는 서브 무기로 설정

        // 무기 및 FPS 모델 초기화
        DisableAllWeapons();
        if (sub != null) sub.SetActive(true); // 기본적으로 서브 무기를 활성화
        currentWeapon = sub;

        // FPS 모델도 기본적으로 FPSModel2를 활성화
        DisableAllFPSModels();
        if (fpsModel2 != null) fpsModel2.SetActive(true);

        // medikit과 projectile 모델을 초기화하고 비활성화
        InitializeWeaponObjects();
        UpdateWeaponObjects(false); // 기본적으로 모든 모델 비활성화
    }

    void Update()
    {
        if (inventory == null)
        {
            // inventory가 null일 때는 무기 변경과 FPS 모델 변경을 수행하지 않음
            return;
        }

        // 숫자 키 입력에 따라 총기 모델 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inventory[0] != null) // 슬롯 0이 비어있지 않을 때만
            {
                SwitchWeapon(main, 0);
                SwitchFPSModel(fpsModel1);
                UpdateWeaponObjects(false); // 숫자 1로 무기 교체 시 관련 오브젝트 비활성화
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inventory[1] != null) // 슬롯 1이 비어있지 않을 때만
            {
                SwitchWeapon(sub, 1);
                SwitchFPSModel(fpsModel2);
                UpdateWeaponObjects(false); // 숫자 2로 무기 교체 시 관련 오브젝트 비활성화
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inventory[2] != null && !hasThrownProjectile) // 슬롯 2가 비어있지 않고 프로젝타일이 투척되지 않은 경우만
            {
                SwitchWeapon(pro, 2);
                // 프로젝타일 무기에는 FPS 모델 변경을 적용하지 않음
                UpdateWeaponObjects(true); // 숫자 3으로 무기 교체 시 프로젝타일 오브젝트 활성화
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inventory[3] != null) // 슬롯 3이 비어있지 않을 때만
            {
                SwitchWeapon(mediKitPrefab, 3);
                // 메디킷 무기에는 FPS 모델 변경을 적용하지 않음
                UpdateWeaponObjects(true); // 숫자 4로 무기 교체 시 메디킷 오브젝트 활성화
            }
        }

        // MediKit 사용
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseMediKit();
        }

        // 왼쪽 마우스 클릭 시 현재 장착된 총기가 숫자 3으로 배정된 무기일 때만 프로젝타일 투척
        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeaponIndex == 2)
            {
                ThrowProjectile();
            }
        }
    }

    void SwitchWeapon(GameObject newWeapon, int num)
    {
        if (inventory[num] == null) return;

        // 현재 장착된 무기 비활성화
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
            UpdateWeaponObjects(false); // 현재 무기 비활성화 시 관련 오브젝트도 비활성화
        }

        // 새로운 무기 활성화
        currentWeapon = newWeapon;
        currentWeaponIndex = num;
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(true);
        }
    }

    void UpdateWeaponObjects(bool isActive)
    {
        if (medikitObject != null)
        {
            // 메디킷 오브젝트는 숫자키 4번일 때만 활성화
            medikitObject.SetActive(isActive && currentWeapon == mediKitPrefab);
        }

        if (projectileObject != null)
        {
            // 프로젝타일 오브젝트는 숫자키 3번일 때만 활성화, 단 한 번 투척된 후에는 다시 활성화되지 않음
            projectileObject.SetActive(isActive && currentWeapon == pro && !hasThrownProjectile);
        }
    }

    void SwitchFPSModel(GameObject newFPSModel)
    {
        if (newFPSModel == null)
        {
            return;
        }

        // 현재 활성화된 FPSModel을 비활성화
        DisableAllFPSModels();

        // 새로운 FPSModel을 활성화
        newFPSModel.SetActive(true);
    }

    void DisableAllFPSModels()
    {
        if (fpsModel1 != null) fpsModel1.SetActive(false);
        if (fpsModel2 != null) fpsModel2.SetActive(false);
    }

    void DisableAllWeapons()
    {
        if (main != null) main.SetActive(false);
        if (sub != null) sub.SetActive(false);
        if (pro != null) pro.SetActive(false);
        if (mediKitPrefab != null) mediKitPrefab.SetActive(false);
    }

    void InitializeWeaponObjects()
    {
        // medikit과 projectile 오브젝트를 player의 자식으로 초기화하고 비활성화
        Transform medikitTransform = transform.Find("MediKit");
        Transform projectileTransform = transform.Find("Projectile");

        if (medikitTransform != null)
        {
            medikitObject = medikitTransform.gameObject;
        }

        if (projectileTransform != null)
        {
            projectileObject = projectileTransform.gameObject;
        }

        // 초기 상태에서 비활성화
        if (medikitObject != null) medikitObject.SetActive(false);
        if (projectileObject != null) projectileObject.SetActive(false);
    }

    void UseMediKit()
    {
        if (currentWeapon != null && mediKitPrefab)
        {
            currentWeapon.SetActive(false); // 사용 후 메디킷 비활성화
            UpdateWeaponObjects(false); // 메디킷 사용 후 관련 오브젝트도 비활성화
        }
    }

    void ThrowProjectile()
    {
        if (currentWeapon != null && pro != null)
        {
            currentWeapon.SetActive(false); // 투척 후 무기 비활성화
            hasThrownProjectile = true; // 프로젝타일이 투척되었음을 표시
            UpdateWeaponObjects(false); // 프로젝타일 투척 후 관련 오브젝트도 비활성화
        }
    }
}