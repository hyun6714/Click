using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObjectPool : MonoBehaviour
{
    private Dictionary<EnemyType, Queue<GameObject>> poolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();

    private void OnEnable()
    {
        EventManager.OnRequestReturnPool += ReturnEnemyToPool;
    }

    private void OnDisable()
    {
        EventManager.OnRequestReturnPool -= ReturnEnemyToPool;
    }

    private void ReturnEnemyToPool(EnemyType type, GameObject enemyObj)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            poolDictionary[type] = new Queue<GameObject>();
        }

        poolDictionary[type].Enqueue(enemyObj);
    }

    public GameObject GetEnemy(EnemyData data, Vector3 spawnPos, Transform target)
    {
        EnemyType type = data.enemyType;

        if (!poolDictionary.ContainsKey(type))
        {
            poolDictionary[type] = new Queue<GameObject>();
        }

        GameObject enemyObj = null;

        if (poolDictionary[type].Count > 0)
        {
            enemyObj = poolDictionary[type].Dequeue();
        }
        else
        {
            enemyObj = Instantiate(data.enemyPrefab);
        }

        EnemyBase enemyScript = enemyObj.GetComponent<EnemyBase>();
        if (enemyScript != null)
        {
            enemyScript.Initialize(data, spawnPos, target);
        }

        return enemyObj;
    }
}