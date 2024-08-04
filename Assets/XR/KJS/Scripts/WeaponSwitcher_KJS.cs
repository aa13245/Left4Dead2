using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject main;
    public GameObject sub;
    public GameObject mediKitPrefab;

    private GameObject currentWeapon;
    private Camera playerCamera;

    Inventory_JSW inventory;

    void Start()
    {
        // 카메라 참조 가져오기
        playerCamera = Camera.main;
        // 인벤토리 컴퍼넌트 가저오기
        inventory = GetComponentInParent<Inventory_JSW>();
        currentWeapon = Instantiate(sub, transform.position, Quaternion.identity);

        // 부모 설정
        currentWeapon.transform.SetParent(playerCamera.transform);

        // 총기 활성화
        currentWeapon.SetActive(true);
        // 총기 위치 업데이트
        if (currentWeapon != null)
        {
            UpdateWeaponPosition();
        }
    }
    

    void Update()
    {
        // 숫자 키 입력에 따라 총기 모델 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(main, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(sub, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(mediKitPrefab, 3);
        }

        // MediKit 사용
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseMediKit();
        }
    }

    void SwitchWeapon(GameObject newWeaponPrefab, int num)
    {
        if (inventory[num] == null) return; 
        // 현재 총기가 존재하면 제거
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // 새로운 총기 생성
        if (newWeaponPrefab != null)
        {
            currentWeapon = Instantiate(newWeaponPrefab, transform.position, Quaternion.identity);

            // 부모 설정
            currentWeapon.transform.SetParent(playerCamera.transform);

            // 총기 활성화
            currentWeapon.SetActive(true);
            // 총기 위치 업데이트
            if (currentWeapon != null)
            {
                UpdateWeaponPosition();
            }
        }
    }

    void UseMediKit()
    {
        if (currentWeapon != null && currentWeapon == mediKitPrefab)
        {
            // MediKit 사용 처리 (예: 체력 회복 로직)
            Debug.Log("MediKit 사용");

            // MediKit 모델 제거
            Destroy(currentWeapon);
        }
    }

    void UpdateWeaponPosition()
    {
        if (playerCamera == null) return;

        // 카메라 방향을 기준으로 총기 위치를 조정
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        Vector3 cameraUp = playerCamera.transform.up;

        // 총기의 위치를 카메라 앞쪽과 아래쪽으로 조정
        Vector3 weaponOffset = cameraForward * 2 + cameraUp *-0.5f + cameraRight * -1.0f;

        // 총기 위치를 업데이트
        currentWeapon.transform.position = playerCamera.transform.position;

        // 총기 회전 방향 설정 (카메라와 같은 방향으로 회전)
        currentWeapon.transform.rotation = Quaternion.LookRotation(cameraForward, cameraUp);

        // 오프셋 수정
        currentWeapon.transform.Translate(weaponOffset, Space.World);
    }
}
