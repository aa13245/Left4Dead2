using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYHammerSpawn : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // 에너미 프리팹 배열 (에너미1, 에너미2, 에너미3)
    public Transform[] spawnPoints; // 스폰 지점 배열 (랜덤 스팟 5군데)

    private void Start()
    {
        // 첫 번째 웨이브: 20초 후에 10명 스폰
        StartCoroutine(SpawnEnemiesAfterDelay(10, 3));
        // 두 번째 웨이브: 30초 후에 25명 스폰
        StartCoroutine(SpawnEnemiesAfterDelay(30, 3)); // 20초 + 30초 = 50초 후
        // 세 번째 웨이브: 45초 후에 30명 스폰
        StartCoroutine(SpawnEnemiesAfterDelay(55, 3)); // 50초 + 45초 = 95초 후
    }

    private IEnumerator SpawnEnemiesAfterDelay(float delay, int numberOfEnemies)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        // 랜덤 에너미 선택
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        // 랜덤 스폰 지점 선택
        int spawnIndex = Random.Range(0, spawnPoints.Length);

        Instantiate(enemyPrefabs[enemyIndex], spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }
}
