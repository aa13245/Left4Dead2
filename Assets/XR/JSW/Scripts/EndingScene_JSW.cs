using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndingScene_JSW : MonoBehaviour
{
    bool sceneEnable;

    GameObject camObj;
    Camera cam;
    Vector3 camMovePos;
    Vector3 camLookPos;

    Vector3 heliStartPos;
    Vector3 heliCtrlPos;
    Vector3 heliDestPos;

    float duration = 20; // 이동시간
    float elapsedTime;

    GameObject helicopter;
    Vector3 heliLookPos;

    Canvas canvas;
    GameObject EndingCanvas;
    Image fadeOut;
    GameObject[] humans = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        camObj = transform.Find("Cam").gameObject;
        cam = camObj.GetComponent<Camera>();
        camMovePos = (transform.Find("CamMovePos").transform.position - cam.transform.position).normalized;
        camLookPos = (cam.transform.Find("CamLookPos").transform.position - cam.transform.position).normalized;
        heliStartPos = GameObject.Find("HeliDest").transform.position;
        heliCtrlPos = transform.Find("EndingHeliCtrl").position;
        heliDestPos = transform.Find("EndingHeliDest").position;
        helicopter = GameObject.Find("Helicopter");
        heliLookPos = helicopter.transform.Find("AnglePos").localPosition.normalized;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        EndingCanvas = transform.Find("EndingCanvas").gameObject;
        fadeOut = EndingCanvas.transform.Find("FadeOut").GetComponent<Image>();
        humans[0] = GameObject.Find("Player");
        for (int i = 1; i < 4; i++) humans[i] = GameObject.Find("Bot" + i);
    }

    // Update is called once per frame
    float t;
    float rotT;
    float heliRotT;
    float speed;
    float pitch;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) StartScene(new bool[] {true,false,false,false});
        if (!sceneEnable) return;
        t += Time.deltaTime;
        if (t < 12)
        {
            if (speed < 3) speed += Time.deltaTime * 3;
        }
        else
        {
            if (speed > 0) speed -= Time.deltaTime * 1;
        }
        if (t > 2 && t < 6)
        {
            pitch += Time.deltaTime * 0.0008f;
        }
        else if (t > 8)
        {
            pitch -= Time.deltaTime * 0.0008f;
        }
        cam.transform.position += camMovePos * Time.deltaTime * speed;
        if (rotT < 0.1f) rotT += Time.deltaTime * 0.001f;
        Vector3 newAngle = Vector3.Lerp(cam.transform.forward, camLookPos, rotT);
        cam.transform.forward = newAngle;
        if (heliRotT < 0.2f) heliRotT += Time.deltaTime * 0.0007f;
        Vector3 heliNewAngle = Vector3.Lerp(helicopter.transform.forward, heliLookPos, heliRotT);
        helicopter.transform.forward = heliNewAngle + Vector3.down * pitch;

        // t 값 업데이트 (0에서 1 사이)
        elapsedTime += Time.deltaTime;
        float _t = Mathf.Clamp01(elapsedTime / duration);
        _t = _t * _t;
        // 베지어 곡선을 따라 새로운 위치 계산
        Vector3 newPosition = CalculateQuadraticBezierPoint(_t, heliStartPos, heliCtrlPos, heliDestPos);
        helicopter.transform.position = newPosition;
    }
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
    bool isStarted;
    public void StartScene(bool[] isDead)
    {
        if (isStarted) return;
        StartCoroutine(Fade(isDead));
    }
    IEnumerator Fade(bool[] isDead)
    {
        while(fadeOut.color.a < 1)
        {
            fadeOut.color = new Color(0, 0, 0, fadeOut.color.a + Time.deltaTime * 3);
            yield return null;
        }
        humans[0].SetActive(false);
        for (int i = 1; i < 4; i++) humans[i].SetActive(!isDead[i]);
        camObj.SetActive(true);
        canvas.enabled = false;
        yield return new WaitForSeconds(0.5f);
        helicopter.transform.position = heliStartPos;
        helicopter.transform.eulerAngles = Vector3.zero;
        sceneEnable = true;
        while (fadeOut.color.a > 0)
        {
            fadeOut.color = new Color(0, 0, 0, fadeOut.color.a - Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
