using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    private Dictionary<EnemyType, Queue<GameObject>> poolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();

    private Dictionary<GameObject, Queue<GameObject>> projectilePoolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void OnEnable()
    {
        EventManager.OnRequestReturnPool += ReturnEnemyToPool;

        EventManager.OnRequestSpawnProjectile += HandleSpawnProjectile;
        EventManager.OnRequestSpawnBuildingProjectile += HandleSpawnBuildingProjectile;
        EventManager.OnRequestReturnProjectile += HandleReturnProjectile;
    }

    private void OnDisable()
    {
        EventManager.OnRequestReturnPool -= ReturnEnemyToPool;

        EventManager.OnRequestSpawnProjectile -= HandleSpawnProjectile;
        EventManager.OnRequestSpawnBuildingProjectile -= HandleSpawnBuildingProjectile;
        EventManager.OnRequestReturnProjectile -= HandleReturnProjectile;
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
        return enemyObj;
    }

    private GameObject GetOrCreateProjectile(GameObject prefab, Vector3 spawnPos)
    {
        if (!projectilePoolDictionary.ContainsKey(prefab))
        {
            projectilePoolDictionary[prefab] = new Queue<GameObject>();
        }

        GameObject projObj;

        if (projectilePoolDictionary[prefab].Count > 0)
        {
            projObj = projectilePoolDictionary[prefab].Dequeue();
            projObj.transform.position = spawnPos;
            projObj.transform.rotation = Quaternion.identity;
            projObj.SetActive(true);
        }
        else
        {
            projObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        return projObj;
    }

    private GameObject HandleSpawnProjectile(GameObject prefab, Vector3 spawnPos, Transform target, int damage, float speed)
    {
        GameObject projObj = GetOrCreateProjectile(prefab, spawnPos);

        if (projObj.TryGetComponent<EnemyProjectile>(out var projectile))
        {
            projectile.Initialize(target, damage, speed, prefab); 
        }
        return projObj;
    }

    private void HandleSpawnBuildingProjectile(GameObject prefab, Vector3 spawnPos, Vector3 targetWorldPos, Vector3Int targetCell, int damage, float speed)
    {
        GameObject projObj = GetOrCreateProjectile(prefab, spawnPos);
        if (projObj.TryGetComponent<EnemyProjectile>(out var projectile))
        {
            projectile.InitializeBuilding(targetWorldPos, targetCell, damage, speed, prefab);
        }
    }

    private void HandleReturnProjectile(GameObject prefab, GameObject projectileObj)
    {
        projectileObj.SetActive(false);

        if (prefab == null)
        {
            Destroy(projectileObj);
            return;
        }

        if (!projectilePoolDictionary.ContainsKey(prefab))
        {
            projectilePoolDictionary[prefab] = new Queue<GameObject>();
        }

        projectilePoolDictionary[prefab].Enqueue(projectileObj);
    }
}