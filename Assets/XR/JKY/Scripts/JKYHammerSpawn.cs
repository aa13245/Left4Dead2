using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYHammerSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemyPrefab; // 에너미 프리팹
    public Transform[] spawnPoints; // 스폰 지점 배열
    //public int totalSpawnTime = 5; // 총 스폰 시간 (초)
    public float spawnInterval = 1.0f; // 스폰 간격 (초)
    //public int enemiesPerWave = 1; // 한 번에 스폰되는 에너미 수
    public float initialDelay = 1.0f; // 처음 스폰 시작 전 대기 시간 (초)
    //public float nextWaveDelay = 5.0f; // 다음 턴 스폰 전 대기 시간 (초)
    private bool isFirstWave = true;
    bool spawnEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player_KJS"))
        {
            if (!spawnEnable)
            {
                spawnEnable = true;
                StartCoroutine(SpawnWaves());
            }
        }
    }


    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length); // 랜덤 스폰 지점 선택
            // 에너미 생성
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);

            // 다음 생성까지 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    //void spawnenemies()
    //{
    //    //for (int i = 0; i < enemiesperwave; i++)
    //    {
    //        int spawnindex = random.range(0, spawnpoints.length); // 랜덤 스폰 지점 선택
    //        instantiate(enemyprefab, spawnpoints[spawnindex].position, spawnpoints[spawnindex].rotation);
    //    }
    //}
}
