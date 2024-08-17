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
    public float shakeDuration = 0.5f; // 흔들림 지속 시간
    public float shakeMagnitude = 0.1f; // 흔들림 세기
    private float shakeTime = 0f;
    private Vector3 originalPosition;
    public bool knockedCamOffset;

    //public float maxShakeDistance = 20f; // 최대 거리 (이 거리 이하에서 흔들림 발생)
    //public float maxShakeMagnitude = 0.5f; // 최대 흔들림 세기
    //private Transform tankerTransform; // Tanker의 Transform을 참조
    //private JKYHammerFS hammerFS; // TankerController를 참조

    private Camera mainCamera;

    void Start()
    {
        originalPosition = transform.localPosition;
        mainCamera = Camera.main;
        // Tanker 오브젝트를 찾아서 TankerController와 Transform을 참조
        //GameObject tanker = GameObject.FindGameObjectWithTag("Tanker"); // 태그를 통해 Tanker 찾기
        //if (tanker != null)
        //{
        //    hammerFS = FindObjectOfType<JKYHammerFS>(); // TankerController 컴포넌트 참조
        //}
    }

    void Update()
    {
        // 흔들림 효과 적용
        if (useRotX)
        {
            if (shakeTime > 0)
            {
                transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
                shakeTime -= Time.deltaTime;
            }
            else
            {
                shakeTime = 0f;
                transform.localPosition = originalPosition;
                // Tanker의 카메라 흔들림 변수 확인
            }
        }
        //// Tanker가 애니메이션 상태일 때만 진동 발생
        //if (tankerTransform != null && hammerFS != null && hammerFS.tankCamerashake)
        //{
        //    float distance = Vector3.Distance(transform.position, tankerTransform.position);
        //    float normalizedDistance = Mathf.Clamp01(distance / maxShakeDistance);
        //    float currentShakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, normalizedDistance);

        //    // 흔들림을 적용
        //    TriggerShake(shakeDuration, currentShakeMagnitude);
        //}


        // 마우스 움직임값을 받아오자.
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // 회전 각도를 누적
        if (useRotY) rotY += mx * rotSpeed * Time.deltaTime;
        if (useRotX) rotX += my * rotSpeed * Time.deltaTime;

        // rotX의 값을 -80 ~ 80 도로 제한
        rotX = Mathf.Clamp(rotX, -80, 80);

        // 물체를 회전 각도로 셋팅 하자.
        transform.localEulerAngles = new Vector3(-rotX, rotY, Camera.main.transform.localEulerAngles.z);
        // 기절 높이 오프셋
        if (knockedCamOffset) Camera.main.transform.Translate(new Vector3(0, -0.7f, 0), Space.World);

        CursorSet();
    }

    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTime = duration;
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
        if (muzzleFlashPrefab != null && Camera.main != null)
        {
            Debug.Log("Muzzle flash is being created");  // 디버그 메시지 추가
            // 머즐 플래시를 카메라 위치에 생성
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, Camera.main.transform.position, Camera.main.transform.rotation);

            // 머즐 플래시의 크기 조정
            muzzleFlash.transform.localScale = muzzleFlashScale;

            Destroy(muzzleFlash, 0.1f);  // 0.1초 후에 머즐 플래시를 삭제
        }
    }
}