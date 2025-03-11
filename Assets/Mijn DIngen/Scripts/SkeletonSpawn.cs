using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public float spawnInterval = 10f; 
    public int maxEnemies = 15; 

    private int currentEnemies = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentEnemies < maxEnemies)
            {
                GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                newEnemy.GetComponent<SkeletonAI>().OnDeath += EnemyDied; 
                currentEnemies++;
            }
        }
    }

    void EnemyDied()
    {
        currentEnemies--;
    }
}