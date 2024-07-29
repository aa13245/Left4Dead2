using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotate_KJS : MonoBehaviour
{//회전 속력 
    public float rotSpeed = 200;
    //회전 값
    float rotY;
    float rotX;

    // 회전 허용
    public bool useRotX;
    public bool useRotY;

    void Start()
    {

    }


    void Update()
    {
        //게임 상태가 '게임 중' 상태일때만 조작할 수 있게한다.
        if (GameManager_KJS.gm.gState != GameManager_KJS.GameState.Run)
        {
            return;
        }
        // 마우스 움직임값을 받아오자.
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        // 회전 각도를 누적

        if (useRotY) rotY += mx * rotSpeed * Time.deltaTime;
        if (useRotX) rotX += my * rotSpeed * Time.deltaTime;

        // rotX의 값의 -80 ~ 80 도로 제한 (최소값, 최대값)
        rotX = Mathf.Clamp(rotX, -80, 80);
        /*if(rotX < -80)
        { 
            rotX = -80;
        }
        if(rotX > 80)
        {
            rotX = 80;
        }
        */
        // 물체를 회전 각도로 셋팅 하자.
        transform.localEulerAngles = new Vector3(-rotX, rotY, 0);

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
}
