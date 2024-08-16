using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjRotate_KJS : MonoBehaviour
{
    public GameObject muzzleFlashPrefab;
    public Vector3 muzzleFlashScale = new Vector3(1f, 1f, 1f);  // 머즐 플래시의 크기 조정
    public float rotSpeed = 200f;
    float rotY;
    float rotX;
    public bool useRotX;
    public bool useRotY;

    // 흔들림 효과 관련 변수
    public float shakeDuration = 0.5f; // TriggerShake로 발생하는 흔들림 지속 시간
    public float shakeMagnitude = 0.1f; // TriggerShake로 발생하는 기본 흔들림 세기
    private float shakeTime = 0f;
    private Vector3 originalPosition;
    public bool knockedCamOffset;

    public float maxShakeDistance = 20f; // 탱커 이동 시 최대 거리 (이 거리 이하에서 흔들림 발생)
    public float maxShakeMagnitude = 0.5f; // 탱커 이동 시 최대 흔들림 세기
    public float tankerShakeFactor = 0.5f; // 탱커가 다가올 때의 진동 강도 조정 계수 (기본 0.5)

    private Transform tankerTransform; // Tanker의 Transform을 참조
    private JKYHammerFS hammerFS; // TankerController를 참조
    private Camera mainCamera;

    // 탱커 진동 강도
    private float tankerShakeMagnitude = 0f;

    // 탱커의 이전 프레임 위치를 저장할 변수
    private Vector3 previousTankerPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        mainCamera = Camera.main;

        // Tanker 오브젝트를 찾아서 JKYHammerFS 컴포넌트를 참조
        GameObject tanker = GameObject.FindGameObjectWithTag("Tanker");
        if (tanker != null)
        {
            hammerFS = tanker.GetComponent<JKYHammerFS>();
            tankerTransform = tanker.transform;

            // 탱커의 초기 위치를 저장
            previousTankerPosition = tankerTransform.position;
        }
    }

    void Update()
    {
        // 1. 기본 TriggerShake 진동 계산
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeMagnitude = 0f;
        }

        // 탱커 이동에 따른 추가 진동 계산
        if (tankerTransform != null && hammerFS != null && hammerFS.tankCamerashake)
        {
            float distance = Vector3.Distance(transform.position, tankerTransform.position);
            float normalizedDistance = Mathf.Clamp01(distance / maxShakeDistance);

            // 진동 강도 계산에 진동 계수(tankerShakeFactor)를 적용
            tankerShakeMagnitude = Mathf.Lerp(maxShakeMagnitude * tankerShakeFactor, 0f, normalizedDistance);
        }
        else
        {
            tankerShakeMagnitude = 0f; // 진동 비활성화
        }

        // 3. 두 진동을 합산하여 적용
        ApplyShake(shakeMagnitude + tankerShakeMagnitude);

        // 카메라 회전 처리
        HandleCameraRotation();

        // 기타 로직 처리
        HandleOtherLogic();
    }

    // 카메라 흔들림 적용 함수
    private void ApplyShake(float totalShakeMagnitude)
    {
        transform.localPosition = originalPosition + Random.insideUnitSphere * totalShakeMagnitude;
    }

    // 외부에서 호출 가능한 카메라 흔들림 트리거
    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTime = duration;
    }

    // 카메라 회전 처리 함수
    private void HandleCameraRotation()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        if (useRotY) rotY += mx * rotSpeed * Time.deltaTime;
        if (useRotX) rotX += my * rotSpeed * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -80, 80);
        transform.localEulerAngles = new Vector3(-rotX, rotY, mainCamera.transform.localEulerAngles.z);
    }

    // 기타 로직 처리 함수
    private void HandleOtherLogic()
    {
        if (knockedCamOffset)
        {
            mainCamera.transform.Translate(new Vector3(0, -0.7f, 0), Space.World);
        }

        CursorSet();
    }

    void CursorSet()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void CreateMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && mainCamera != null)
        {
            Debug.Log("Muzzle flash is being created");  // 디버그 메시지 추가
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, mainCamera.transform.position, mainCamera.transform.rotation);
            muzzleFlash.transform.localScale = muzzleFlashScale;
            Destroy(muzzleFlash, 0.1f);  // 0.1초 후에 머즐 플래시를 삭제
        }
    }
}