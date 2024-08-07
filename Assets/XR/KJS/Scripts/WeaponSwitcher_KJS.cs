using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject main;
    public GameObject sub;
    public GameObject pro;
    public GameObject mediKitPrefab;

    private GameObject currentWeapon;
    private Camera playerCamera;
    private int currentWeaponIndex = -1; // 현재 장착된 무기의 인덱스

    Inventory_JSW inventory;

    void Start()
    {
        // 카메라 참조 가져오기
        playerCamera = Camera.main;
        // 인벤토리 컴포넌트 가져오기
        inventory = GetComponentInParent<Inventory_JSW>();
        currentWeapon = Instantiate(sub, transform.position, Quaternion.identity);
        currentWeaponIndex = 1; // 숫자 2번에 해당하는 무기 인덱스

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
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(pro, 2);
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

        // 왼쪽 마우스 클릭 시 현재 장착된 총기가 숫자 3으로 배정된 무기일 때만 프로젝타일 투척
        if (Input.GetMouseButtonDown(0)) // 0은 왼쪽 마우스 버튼을 의미합니다.
        {
            if (currentWeaponIndex == 2) // 현재 장착된 무기가 숫자 3에 해당하는 경우
            {
                ThrowProjectile();
            }
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
            currentWeaponIndex = num; // 현재 장착된 무기의 인덱스 업데이트

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

    void ThrowProjectile()
    {
        if (currentWeapon != null && pro != null)
        {
            // 발사 위치 계산
            Vector3 firePosition = playerCamera.transform.position + playerCamera.transform.forward * 2;

            // 프로젝타일 생성
            GameObject projectile = Instantiate(pro, firePosition, Quaternion.identity);

            // 즉시 파괴하도록 설정 (0초 후 제거)
            Destroy(projectile, 0f);

            // 총기 모델 제거
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
        Vector3 weaponOffset = cameraForward * 2 + cameraUp * -0.5f + cameraRight * -1.0f;

        // 총기 위치를 업데이트
        currentWeapon.transform.position = playerCamera.transform.position;

        // 총기 회전 방향 설정 (카메라와 같은 방향으로 회전)
        currentWeapon.transform.rotation = Quaternion.LookRotation(cameraForward, cameraUp);

        // 오프셋 수정
        currentWeapon.transform.Translate(weaponOffset, Space.World);
    }
}


