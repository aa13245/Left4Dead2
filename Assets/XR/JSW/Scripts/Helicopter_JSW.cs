using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter_JSW : MonoBehaviour
{
    Vector3 startPos;
    Vector3 controlPos;
    Vector3 endPos;

    float duration = 15; // 이동시간
    float elapsedTime;
    Human_KJS[] humans = new Human_KJS[4];
    EndingScene_JSW ending;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        controlPos = GameObject.Find("HeliCtrl").transform.position;
        endPos = GameObject.Find("HeliDest").transform.position;
        humans[0] = GameObject.Find("Player").GetComponent<Human_KJS>();
        for (int i = 1; i < 4; i++) humans[i] = GameObject.Find("Bot" + i).GetComponent<Human_KJS>();
        ending = GameObject.Find("EndingScene").GetComponent<EndingScene_JSW>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnable) return;
        // t 값 업데이트 (0에서 1 사이)
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        t = t * (2 - t);
        // 베지어 곡선을 따라 새로운 위치 계산
        Vector3 newPosition = CalculateQuadraticBezierPoint(t, startPos, controlPos, endPos);

        // 오브젝트 위치 업데이트
        if (!isArrived) transform.position = newPosition;
        if (elapsedTime > 5 && !isArrived) Rot();
        if (isEnable && !isArrived && elapsedTime >= duration) isArrived = true;
        if (isArrived) EnterCheck();
    }

    // 2차 베지어 곡선 계산 함수
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2(1-t)t * P1
        p += tt * p2; // t^2 * P2

        return p;
    }
    float rotSpeed;
    void Rot()
    {
        rotSpeed += Time.deltaTime * 10;
        transform.Rotate(new Vector3(0, -Time.deltaTime * rotSpeed * Mathf.DeltaAngle(0, transform.rotation.y), 0));
    }

    public bool isEnable;
    public bool isArrived;
    public void HelicopterEnable()
    {
        isEnable = true;
    }
    void EnterCheck()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!humans[i].isEntered && humans[i].humanState == Human_KJS.HumanState.Normal) return;
        }
        
        ending.StartScene(new bool[] { !humans[0].isEntered, humans[1].isEntered, humans[2].isEntered , humans[3].isEntered });
    }
}
