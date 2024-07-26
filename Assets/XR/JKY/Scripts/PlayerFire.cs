using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public int weaponPower = 10;
    // 총알 파편 효과 공장(Prefab)
    public GameObject bulletimpactFactory;

    // Raycast를 이용한 총알 발사에 검출되는 Layer 설정
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       

        //1번키를 누르면
        if (Input.GetKeyDown(KeyCode.V))
        {
            //카메라 위치에서 카메라 앞방향으로 향하는 RaY를 만들ㅈㅏ
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            //Ray를 발사해서 어딘가에 부딪혔다면
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, layerMask))
            {
                if ( hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    JKYEnemyFSM eFSM = hitInfo.transform.GetComponent<JKYEnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }

                {

                // 총알 파편 효과를 생성하자.
                GameObject bulletImpact = Instantiate(bulletimpactFactory);
                // 생성된 효과를 부딪힌 위치에 놓자.
                bulletImpact.transform.position = hitInfo.point;
                // 생성된 효과의 앞방향을 부딪힌 위치의 normal 방향으로 설정
                //bulletImpact.transform.forward = hitInfo.normal;

                // 반사각 구하기
                Vector3 outDirection = Vector3.Reflect(ray.direction, hitInfo.normal);
                //부딪힌 오브젝트의 이름과, 부딪힌 위치를 출력해보자.

                bulletImpact.transform.forward = outDirection;

                //Destroy(bulletimpactFactory, 2);
                }
                //print(hitInfo.transform.name + ", " + hitInfo.point);
                //print(hitInfo.transform.name + ", " + hitInfo.transform.position);


                //Vector3.Distance(Camera.main.transform.position, hitInfo.point);\
                //Vector3 dist = Camera.main.transform.position - hitInfo.point;
                //dist.magnitude;
                // 법선벡터( 면의 수직인 벡터)
                //              hitInfo.normal
            }
            // 부
        }
    }
}
