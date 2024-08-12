using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject fpsModel1;       // FPSModel1
    public GameObject fpsModel2;       // FPSModel2
    public GameObject pro;             // 프로젝타일 무기
    public GameObject mediKitPrefab;   // 메디킷

    private GameObject currentWeapon;
    private int currentWeaponIndex = -1;

    Inventory_JSW inventory;

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
        if (fpsModel2 != null) fpsModel2.SetActive(true); // 기본적으로 서브 무기를 활성화

        // medikit과 projectile 모델을 초기화하고 비활성화
        InitializeWeaponObjects();
    }

    void Update()
    {
        // 마우스 휠 입력에 따라 무기 전환
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) // 휠을 위로 스크롤
        {
            SwitchToNextWeapon();
        }
        else if (scroll < 0f) // 휠을 아래로 스크롤
        {
            SwitchToPreviousWeapon();
        }

        // 숫자 키 입력에 따라 총기 모델 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inventory[0] != null) // 슬롯 0이 비어있지 않을 때만
            {
                SwitchWeapon(fpsModel1, 0);
                UpdateWeaponObjects(); // 숫자 1로 무기 교체 시 관련 오브젝트 비활성화
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inventory[1] != null) // 슬롯 1이 비어있지 않을 때만
            {
                SwitchWeapon(fpsModel2, 1);
                UpdateWeaponObjects(); // 숫자 2로 무기 교체 시 관련 오브젝트 비활성화
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inventory[2] != null && !hasThrownProjectile) // 슬롯 2가 비어있지 않고 프로젝타일이 투척되지 않은 경우만
            {
                SwitchWeapon(pro, 2);
                // 프로젝타일 무기에는 FPS 모델 변경을 적용하지 않음
                UpdateWeaponObjects(); // 숫자 3으로 무기 교체 시 프로젝타일 오브젝트 활성화
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inventory[3] != null) // 슬롯 3이 비어있지 않을 때만
            {
                SwitchWeapon(mediKitPrefab, 3);
                // 메디킷 무기에는 FPS 모델 변경을 적용하지 않음
                UpdateWeaponObjects(); // 숫자 4로 무기 교체 시 메디킷 오브젝트 활성화
            }
        }

        // 왼쪽 마우스 클릭 시 현재 장착된 총기가 숫자 3으로 배정된 무기일 때만 프로젝타일 투척
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (currentWeaponIndex == 2)
        //    {
        //        ThrowProjectile();
        //    }
        //}
    }

    private void SwitchToNextWeapon()
    {
        int startIndex = (currentWeaponIndex + 1) % 4;
        int count = 0;

        while (count < 4)
        {
            if (inventory[startIndex] != null)
            {
                SwitchWeaponByIndex(startIndex);
                return;
            }

            startIndex = (startIndex + 1) % 4;
            count++;
        }

        Debug.Log("No valid weapons to switch to.");
    }

    private void SwitchToPreviousWeapon()
    {
        int startIndex = (currentWeaponIndex - 1 + 4) % 4;
        int count = 0;

        while (count < 4)
        {
            if (inventory[startIndex] != null)
            {
                SwitchWeaponByIndex(startIndex);
                return;
            }

            startIndex = (startIndex - 1 + 4) % 4;
            count++;
        }

        Debug.Log("No valid weapons to switch to.");
    }

    private void SwitchWeaponByIndex(int index)
    {
        if (index < 0 || index >= 4) return;

        GameObject weaponToSwitch = null;
        if (index == 0) weaponToSwitch = fpsModel1;
        else if (index == 1) weaponToSwitch = fpsModel2;
        else if (index == 2) weaponToSwitch = pro;
        else if (index == 3) weaponToSwitch = mediKitPrefab;

        SwitchWeapon(weaponToSwitch, index);
        UpdateWeaponObjects();
    }

    void SwitchWeapon(GameObject newWeapon, int num)
    {
        if (inventory[num] == null) return;

        // 현재 장착된 무기 비활성화
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        // 현재 FPS 모델 비활성화
        if (fpsModel1 != null && fpsModel1.activeSelf)
        {
            fpsModel1.SetActive(false);
        }

        if (fpsModel2 != null && fpsModel2.activeSelf)
        {
            fpsModel2.SetActive(false);
        }

        // 새로운 무기 활성화
        currentWeapon = newWeapon;
        currentWeaponIndex = num;
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(true);
        }
        // 새 FPS 모델 활성화
        if (currentWeapon == fpsModel1)
        {
            if (fpsModel1 != null)
            {
                fpsModel1.SetActive(true);
            }
        }
        else if (currentWeapon == fpsModel2)
        {
            if (fpsModel2 != null)
            {
                fpsModel2.SetActive(true);
            }
        }
    }

    void UpdateWeaponObjects()
    {
        if (medikitObject != null)
        {
            // 메디킷 오브젝트는 숫자키 4번일 때만 활성화
            medikitObject.SetActive(currentWeapon == mediKitPrefab);
        }

        if (projectileObject != null)
        {
            // 프로젝타일 오브젝트는 숫자키 3번일 때만 활성화, 단 한 번 투척된 후에는 다시 활성화되지 않음
            projectileObject.SetActive(currentWeapon == pro);
        }
    }

    void DisableAllWeapons()
    {
        if (fpsModel1 != null) fpsModel1.SetActive(false);
        if (fpsModel2 != null) fpsModel2.SetActive(false);
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
}

    //void UseMediKit()
    //{
    //    if (currentWeapon != null && mediKitPrefab != null)
    //    {
    //        currentWeapon.SetActive(false); // 사용 후 메디킷 비활성화
    //        UpdateWeaponObjects(); // 메디킷 사용 후 관련 오브젝트도 비활성화

    //    }
    //}

    //void ThrowProjectile()
    //{
    //    if (currentWeapon != null && pro != null)
    //    {
    //        currentWeapon.SetActive(false); // 투척 후 무기 비활성화
    //        hasThrownProjectile = true; // 프로젝타일이 투척되었음을 표시
    //        UpdateWeaponObjects(); // 프로젝타일 투척 후 관련 오브젝트도 비활성화
    //    }
    //}