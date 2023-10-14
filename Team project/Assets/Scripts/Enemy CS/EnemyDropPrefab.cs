using System.Collections;
using UnityEngine;

public class EnemyDropPrefab : MonoBehaviour
{
    public GameObject healthPrefab;
    public float dropProbability = 0.5f;

    private void OnDestroy()
    {
        if (Random.value < dropProbability)
        {
            DropPrefab();
        }
    }

    private void DropPrefab()
    {
        Instantiate(healthPrefab, transform.position, Quaternion.identity);
    }
}
