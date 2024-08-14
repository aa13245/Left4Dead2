using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float maxShakeMagnitude = 0.3f;  // 최대 흔들림 강도
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ShakeCamera(float intensity)
    {
        // 흔들림 강도에 따라 카메라 위치를 랜덤하게 이동
        float x = Random.Range(-1f, 1f) * intensity;
        float y = Random.Range(-1f, 1f) * intensity;

        transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
    }

    public void ResetCameraPosition()
    {
        transform.localPosition = originalPosition;
    }
}
