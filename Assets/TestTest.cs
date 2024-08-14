using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTest : MonoBehaviour
{
    [SerializeField] FullScreenPassRendererFeature full;
    public Material mat;
    public float lerpDuration = 10f; // Lerp의 지속 시간
    private float lerpTime = 0f;    // Lerp 시간 계산
    private float startValue = 1f;  // 시작 색상 비율
    private float endValue = -0.2f; // 종료 색상 비율
    GameObject qw;
    bool check = false;
    public GameObject player;
    float colorScale;

    // Start is called before the first frame update
    void Start()
    {
        colorScale = mat.GetFloat("_ColorScale");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            mat.SetFloat("_ColorScale", 1f);
            //colorScale = 1f;
            full.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            full.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            //StartCoroutine(Asd());
            check = true;

        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerControler_KJS qw = player.gameObject.GetComponent<PlayerControler_KJS>();
            qw.BumerAttack();
        }

        if (check == true)
        {
            lerpTime += Time.deltaTime / lerpDuration;

            float lerpedValue = Mathf.Lerp(startValue, endValue, lerpTime);

            // Material의 속성 업데이트
            if (mat != null)
            {
                mat.SetFloat("_ColorScale", lerpedValue);
            }

     

            if (colorScale < -0.2f)
            {
                // Lerp 완료 시, 시간과 색상 비율 초기화
                if (lerpTime >= 1f)
                {
                    lerpTime = 1f;
                    full.SetActive(false);
                    enabled = false;  // 스크립트 비활성화 (옵션)
                    mat.SetFloat("_ColorScale", 1f);

                }
            }
        }
    }
    IEnumerator Asd()
    {
        while (true)
        {

            lerpTime += Time.deltaTime / lerpDuration;

            float lerpedValue = Mathf.Lerp(startValue, endValue, lerpTime);

            // Material의 속성 업데이트
            if (mat != null)
            {
                mat.SetFloat("_ColorScale", lerpedValue);
            }

            yield return new WaitForSeconds(0.1f);

            if(colorScale < -0.2f)
            {
                // Lerp 완료 시, 시간과 색상 비율 초기화
                if (lerpTime >= 1f)
                {
                    lerpTime = 1f;
                    enabled = false;  // 스크립트 비활성화 (옵션)
                }

                break;
            }
        }
       
        yield return new WaitForSeconds(1f);
    }


}
