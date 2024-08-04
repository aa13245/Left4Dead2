using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKYHammerSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemyPrefab; // 에너미 프리팹
    public Transform[] spawnPoints; // 스폰 지점 배열
    public int totalSpawnTime = 5; // 총 스폰 시간 (초)
    public float spawnInterval = 1.0f; // 스폰 간격 (초)
    public int enemiesPerWave = 1; // 한 번에 스폰되는 에너미 수
    public float initialDelay = 30.0f; // 처음 스폰 시작 전 대기 시간 (초)
    public float nextWaveDelay = 50.0f; // 다음 턴 스폰 전 대기 시간 (초)
    private bool isFirstWave = true;
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SpawnWaves()
    {
        while (true)
        {
            if (isFirstWave)
            {
                yield return new WaitForSeconds(initialDelay);
                isFirstWave = false;
            }
            else
            {
                yield return new WaitForSeconds(nextWaveDelay);
            }

            float elapsedTime = 0f;
            while (elapsedTime < totalSpawnTime)
            {
                SpawnEnemies();
                elapsedTime += spawnInterval;
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length); // 랜덤 스폰 지점 선택
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        }
    }
}
