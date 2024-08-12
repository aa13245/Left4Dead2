using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction_KJS : MonoBehaviour
{
    public GameObject bombEffect; // 폭발 이펙트 프리팹 변수
    private bool hasExploded = false; // 폭발 여부를 추적
    public AudioSource bombSound;//오디오 소스

    void Start()
    {
        if (bombSound == null)
        {
             bombSound = GetComponent<AudioSource>();
        }
            
    }

    // 수류탄 처리 코루틴
    private IEnumerator HandleProjectile(float damage, float range, float duration)
    {
        // duration(지속 시간)만큼 대기
        yield return new WaitForSeconds(duration);

        // 폭발 처리
        Explode(damage, range);

    }

    // 초기화 메서드: Projectile 메서드에서 호출하여 필요한 파라미터 설정
    public void Initialize(float damage, float range, float duration)
    {
        // HandleProjectile 코루틴을 시작하여 duration 시간 후 폭발 처리
        StartCoroutine(HandleProjectile(damage, range, duration));
    }

    // 폭발 처리 메서드
    private void Explode(float damage, float range)
    {
        // 중복 폭발 방지
        if (hasExploded) return;
        hasExploded = true;

        // 폭발 이펙트 프리팹을 생성한다.
        GameObject eff = Instantiate(bombEffect, transform.position, Quaternion.identity);

        // 폭발 범위 내의 오브젝트들에 대한 충돌 감지
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {

                // 적에게 데미지를 적용
                collider.gameObject.GetComponent<JKYEnemyHPSystem>()?.GetDamage(damage, gameObject);
            }
        }
        if(bombSound != null)
        {
            bombSound.Play();
        }
        // 폭발 후 수류탄 오브젝트 제거
        Destroy(gameObject, 4);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    // 충돌했을 때의 처리
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌 시 즉시 폭발하지 않고, duration 시간이 지나면 폭발하도록 함
        if (!hasExploded)
        {
            // 충돌이 발생해도 duration 전에 폭발하지 않도록 아무 작업도 하지 않음
            // 필요에 따라 충돌 시 이펙트나 사운드만 발생시킬 수 있음
        }
    }
}