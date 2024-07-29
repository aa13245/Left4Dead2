using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow_KJS : MonoBehaviour
{
    // 발사 위치
    public GameObject firePosition;
    // 투척 무기 오브젝트 
    public GameObject bombFactory;
    //투척 파워
    public float throwPower = 15F;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ////게임 상태가 '게임 중' 상태일때만 조작할 수 있게한다.
        //if (GameManager_KJS.gm.gState != GameManager_KJS.GameState.Run)
        //{
        //    return;
        //}
        ////마우스 오른쪽 버튼을 누르면 시선이 바라보는 방향으로 수류탄을 던지고 싶다.

        ////마우스 오른쪽 버튼을 입력받는다.
        //if (Input.GetMouseButtonDown(1))
        //{
        //    //수류탄 오브젝트를 생성한 후 수류탄의 생성위치를 발사 위치로 한다.
        //    GameObject bomb = Instantiate(bombFactory);
        //    bomb.transform.position = firePosition.transform.position;

        //    //수류탄 오브젝트의 Rigidbody component를 가져온다.
        //    Rigidbody rb = bomb.GetComponent<Rigidbody>();

        //    //카메라의 정면 방향으로 수류탄에 물리적인 힘을 가한다.
        //    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
        //}
    }
}
