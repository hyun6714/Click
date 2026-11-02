using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> projectilePoolDict = new Dictionary<GameObject, Queue<GameObject>>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnProjectile += HandleSpawnProjectile;
        EventManager.OnRequestSpawnBuildingProjectile += HandleSpawnBuildingProjectile;
        EventManager.OnRequestReturnProjectile += ReturnProjectile;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnProjectile -= HandleSpawnProjectile;
        EventManager.OnRequestSpawnBuildingProjectile -= HandleSpawnBuildingProjectile;
        EventManager.OnRequestReturnProjectile -= ReturnProjectile;
    }

    private GameObject HandleSpawnProjectile(GameObject prefab, Vector3 spawnPos, Transform target, int damage, float speed)
    {
        if (!projectilePoolDict.ContainsKey(prefab))
        {
            projectilePoolDict[prefab] = new Queue<GameObject>();
        }

        GameObject projObj = null;

        if (projectilePoolDict[prefab].Count > 0)
        {
            projObj = projectilePoolDict[prefab].Dequeue();
            projObj.transform.position = spawnPos;
            projObj.SetActive(true);
        }
        else
        {
            projObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        if (projObj.TryGetComponent<EnemyProjectile>(out var projectile))
        {
            projectile.Initialize(target, damage, speed, prefab);
        }

        return projObj;
    }

    private void ReturnProjectile(GameObject prefabRef, GameObject projObj)
    {
        projObj.SetActive(false);

        if (!projectilePoolDict.ContainsKey(prefabRef))
        {
            projectilePoolDict[prefabRef] = new Queue<GameObject>();
        }

        projectilePoolDict[prefabRef].Enqueue(projObj);
    }

    private void HandleSpawnBuildingProjectile(
        GameObject prefab,
        Vector3 spawnPos,
        Vector3 targetWorldPos,
        Vector3Int targetCell,
        int damage,
    float speed)
    {
        if (prefab == null)
        {
            Debug.LogError("[ProjectilePool] 투사체 프리팹이 null입니다.");
            return;
        }

        if (!projectilePoolDict.ContainsKey(prefab))
        {
            projectilePoolDict[prefab] = new Queue<GameObject>();
        }

        GameObject projectileObj;

        if (projectilePoolDict[prefab].Count > 0)
        {
            projectileObj = projectilePoolDict[prefab].Dequeue();
            projectileObj.transform.position = spawnPos;
            projectileObj.SetActive(true);
        }
        else
        {
            projectileObj = Instantiate(
                prefab,
                spawnPos,
                Quaternion.identity
            );
        }

        if (projectileObj.TryGetComponent<EnemyProjectile>(out var projectile))
        {
            projectile.InitializeBuilding(
                targetWorldPos,
                targetCell,
                damage,
                speed,
                prefab
            );
        }
        else
        {
            Debug.LogError(
                $"투사체 프리팹에 EnemyProjectile이 없습니다: {prefab.name}"
            );

            Destroy(projectileObj);
        }
    }
}