using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float spawnRate = 5f;
    public Transform[] spawnPoints;

    private List<Slider> healthBars = new List<Slider>();

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnAllEnemiesAtStart()
    {
        HashSet<GameObject> spawnedEnemies = new HashSet<GameObject>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            foreach (GameObject enemyPrefab in enemyPrefabs)
            {
                if (!spawnedEnemies.Contains(enemyPrefab))
                {
                    // Spawn the enemy at the selected spawn point
                    GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                    // Get the EnemyHealthManager component of the spawned enemy
                    EnemyHealthManager enemyHealthManager = newEnemy.GetComponent<EnemyHealthManager>();

                    // Assign the appropriate health bar based on the enemy prefab
                    enemyHealthManager.healthBar = GetHealthBarFromCanvas(enemyPrefabs.IndexOf(enemyPrefab));

                    // Disable the health bar at the start
                    enemyHealthManager.healthBar.gameObject.SetActive(false);

                    // Mark the enemy as spawned
                    spawnedEnemies.Add(enemyPrefab);
                }
            }
        }

        yield return null; // You can yield null to finish the coroutine immediately
    }

    private IEnumerator SpawnEnemies()
    {
        // Spawn all enemies at the start
        yield return StartCoroutine(SpawnAllEnemiesAtStart());

        // Wait for a moment before starting continuous spawning
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Select a random enemy from the list
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Spawn the enemy at the selected spawn point
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Get the EnemyHealthManager component of the spawned enemy
            EnemyHealthManager enemyHealthManager = newEnemy.GetComponent<EnemyHealthManager>();

            // Assign the appropriate health bar based on the enemy prefab
            if (enemyPrefabs.Contains(enemyPrefab))
            {
                int healthBarIndex = enemyPrefabs.IndexOf(enemyPrefab);
                enemyHealthManager.healthBar = GetHealthBarFromCanvas(healthBarIndex);
            }

            // Wait for the specified spawn rate
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private Slider GetHealthBarFromCanvas(int index)
    {
        // Assuming you have a Canvas component in your scene that contains health bars
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas != null && index < canvas.transform.childCount)
        {
            Transform healthBarTransform = canvas.transform.GetChild(index);
            Slider healthBar = healthBarTransform.GetComponent<Slider>();

            return healthBar;
        }

        return null;
    }
}