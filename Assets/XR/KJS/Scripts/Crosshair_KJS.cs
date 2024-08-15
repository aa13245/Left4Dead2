using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair_KJS : MonoBehaviour
{
    public RectTransform crosshairTop;
    public RectTransform crosshairBottom;
    public RectTransform crosshairLeft;
    public RectTransform crosshairRight;

    private Vector2 originalTopPos;
    private Vector2 originalBottomPos;
    private Vector2 originalLeftPos;
    private Vector2 originalRightPos;

    public float spreadDuration = 0.1f; // 벌어지는데 걸리는 시간
    public float resetDuration = 0.3f;  // 원래 위치로 돌아가는 시간 (길게 설정)
    public float spreadAmount = 10f;    // 크로스헤어 벌어지는 크기 (더 크게 설정)

    private Canvas canvas;

    void Start()
    {
        // 크로스헤어의 RectTransform을 가져옵니다.
        canvas = GetComponentInParent<Canvas>();

        // 초기 위치 저장
        originalTopPos = crosshairTop.anchoredPosition;
        originalBottomPos = crosshairBottom.anchoredPosition;
        originalLeftPos = crosshairLeft.anchoredPosition;
        originalRightPos = crosshairRight.anchoredPosition;
    }

    public void TriggerCrosshairShake(float magnitude, float additionalSpreadAmount)
    {
        StopAllCoroutines(); // 기존 코루틴 중지 (새로운 발사에 대비)
        StartCoroutine(CrosshairSpread(spreadAmount + additionalSpreadAmount));
    }

    private IEnumerator CrosshairSpread(float totalSpreadAmount)
    {
        // 크로스헤어가 벌어지는 애니메이션
        float elapsedTime = 0f;

        while (elapsedTime < spreadDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spreadDuration;

            crosshairTop.anchoredPosition = Vector2.Lerp(originalTopPos, originalTopPos + new Vector2(0, totalSpreadAmount), t);
            crosshairBottom.anchoredPosition = Vector2.Lerp(originalBottomPos, originalBottomPos - new Vector2(0, totalSpreadAmount), t);
            crosshairLeft.anchoredPosition = Vector2.Lerp(originalLeftPos, originalLeftPos - new Vector2(totalSpreadAmount, 0), t);
            crosshairRight.anchoredPosition = Vector2.Lerp(originalRightPos, originalRightPos + new Vector2(totalSpreadAmount, 0), t);

            yield return null;
        }

        // 일정 시간이 지나면 크로스헤어가 원래 위치로 서서히 돌아가는 애니메이션
        elapsedTime = 0f;
        while (elapsedTime < resetDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / resetDuration;

            crosshairTop.anchoredPosition = Vector2.Lerp(crosshairTop.anchoredPosition, originalTopPos, t);
            crosshairBottom.anchoredPosition = Vector2.Lerp(crosshairBottom.anchoredPosition, originalBottomPos, t);
            crosshairLeft.anchoredPosition = Vector2.Lerp(crosshairLeft.anchoredPosition, originalLeftPos, t);
            crosshairRight.anchoredPosition = Vector2.Lerp(crosshairRight.anchoredPosition, originalRightPos, t);

            yield return null;
        }
    }
}