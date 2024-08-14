using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 텍스트 사용을 위한 네임스페이스

using UnityEngine;
using UnityEngine.UI; // UI 텍스트 사용을 위한 네임스페이스

public class LocationTrigger : MonoBehaviour
{
    public Transform targetLocation; // 도착해야 하는 위치
    public float detectionRadius = 1.0f; // 위치 감지 반경
    public Text notificationText; // UI 텍스트 요소

    private void Update()
    {
        // 플레이어와 목표 위치 간의 거리 계산
        float distance = Vector3.Distance(transform.position, targetLocation.position);

        // 플레이어가 목표 위치에 도착했는지 확인
        if (distance < detectionRadius)
        {
            ShowNotification("목표 위치에 도착했습니다!");
        }
        else
        {
            HideNotification();
        }
    }

    private void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.text = "";
        }
    }
}