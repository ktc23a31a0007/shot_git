using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float xOffset = 12f; // 画面外右端

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnRate);
    }

    void Spawn()
    {
        // Y軸（上下）のランダムな位置に生成
        float randomY = Random.Range(-4.5f, 4.5f);
        Vector3 spawnPos = new Vector3(xOffset, randomY, 0);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}