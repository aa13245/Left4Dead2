using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class BombAction_KJS : MonoBehaviour
{
    public GameObject bombEffect; // 폭발 이펙트 프리팹 변수
    private bool hasExploded = false; // 폭발 여부를 추적
    public AudioSource bombSound; // 오디오 소스
    private ParticleSystem particleSystem; // 파티클 시스템
    public float bombEffectScale = 2f; // 폭발 이펙트의 크기 배율
    private ObjRotate_KJS objRotateScript; // ObjRotate_KJS 스크립트 참조
    public float muzzleFlashDuration = 0.5f; // 머즐 플래시의 유지 시간

    void Start()
    {
        if (bombSound == null)
        {
            bombSound = GetComponent<AudioSource>();
        }

        GameObject mainCamera = Camera.main.gameObject;
        if (mainCamera != null)
        {
            objRotateScript = mainCamera.GetComponent<ObjRotate_KJS>();
        }
    //// 재생 속도를 1.5배로 높임
    //if (bombSound != null)
    //{
    //    bombSound.pitch = 1.2f;
    //}

    // 자식 오브젝트에서 파티클 시스템 찾기
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private IEnumerator HandleProjectile(float damage, float range, float duration)
    {
        // 투척과 동시에 bombSound 재생 시작
        if (bombSound != null && !bombSound.isPlaying)
        {
            // 재생 시작 지점을 5초로 설정
            bombSound.time = 14f;
            bombSound.Play();
        }

        // duration(지속 시간)만큼 대기
        yield return new WaitForSeconds(duration);

        // 폭발 처리
        Explode(damage, range);
    }

    public void Initialize(float damage, float range, float duration)
    {
        // HandleProjectile 코루틴을 시작하여 duration 시간 후 폭발 처리
        StartCoroutine(HandleProjectile(damage, range, duration));
    }

    private void Explode(float damage, float range)
    {
        if (hasExploded) return;
        hasExploded = true;

        // 폭발 이펙트 생성
        GameObject eff = Instantiate(bombEffect, transform.position, Quaternion.identity);
        eff.transform.localScale *= bombEffectScale; // 폭발 이펙트의 크기를 키움

        // 머즐 플래시 생성
        if (objRotateScript != null)
        {
            objRotateScript.muzzleFlashScale = new Vector3(10f, 10f, 10f); // 머즐 플래시 크기 조정
            GameObject muzzleFlash = Instantiate(objRotateScript.muzzleFlashPrefab, Camera.main.transform.position + Camera.main.transform.forward * 1f, Camera.main.transform.rotation);
            muzzleFlash.transform.localScale = objRotateScript.muzzleFlashScale;
            Destroy(muzzleFlash, muzzleFlashDuration); // 설정한 시간 동안 머즐 플래시를 유지
        }

        if (objRotateScript != null)
        {
            objRotateScript.TriggerShake(0.5f, 0.1f); // 흔들림 효과를 적용 (지속 시간: 0.5초, 강도: 0.1)
        }

        // 파티클 시스템 비활성화
        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collider.gameObject.GetComponent<JKYEnemyHPSystem>()?.GetDamage(damage, gameObject);

                Rigidbody enemyRb = collider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    enemyRb.AddExplosionForce(55f, transform.position, 120f, 120f, ForceMode.VelocityChange);
                }
            }
        }

        // 폭발 소리 재생 (이미 재생 중이면 중복 재생되지 않음)
        if (bombSound != null && !bombSound.isPlaying)
        {
            bombSound.Play();
        }

        // 자신 오브젝트와 파티클 시스템을 비활성화
        Destroy(gameObject, 4f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded)
        {
            // 충돌 처리
        }
    }
}
