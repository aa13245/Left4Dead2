using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction_KJS : MonoBehaviour
{
    // 폭발 이펙트 프리팹 변수
    public GameObject bombEffect;
    // 화염 이펙트 프리팹 변수
    public GameObject fireEffect;

    // 충돌했을 때의 처리
    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 이펙트 프리팹을 생성한다.
        GameObject eff = Instantiate(bombEffect);
        // 폭발 이펙트의 위치를 수류탄 오브젝트 자신의 위치와 동일하게 설정한다.
        eff.transform.position = transform.position;

        // 화염 이펙트 프리팹을 생성한다.
        GameObject fireEff = Instantiate(fireEffect);
        // 화염 이펙트의 위치를 수류탄 오브젝트 자신의 위치와 동일하게 설정한다.
        fireEff.transform.position = transform.position;

        // 화염 이펙트의 지속 시간을 10초로 설정한다.
        Destroy(fireEff, 10f);

        // 자기 자신을 제거한다.
        Destroy(gameObject);
    }
}
