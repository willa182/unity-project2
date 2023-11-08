using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float spawnRate = 5f;
    public Transform[] spawnPoints;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Select a random enemy from the list
            GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Spawn the enemy at the selected spawn point
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);

            // Wait for the specified spawn rate
            yield return new WaitForSeconds(spawnRate);
        }
    }
}