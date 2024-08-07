using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotate_KJS : MonoBehaviour
{
    public float rotSpeed = 200;
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

    private Vector3 originalCameraRotation;
    private Vector3 currentCameraRecoil;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // 게임 상태가 '게임 중' 상태일 때만 조작할 수 있게 한다.
        if (GameManager_KJS.gm.gState != GameManager_KJS.GameState.Run)
        {
            return;
        }

        // 흔들림 효과 적용
        if (shakeTime > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeTime = 0f;
            transform.localPosition = originalPosition;
        }

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
}