using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> projectilePoolDict = new Dictionary<GameObject, Queue<GameObject>>();

    private void OnEnable()
    {
        EventManager.OnRequestSpawnProjectile += HandleSpawnProjectile;
        EventManager.OnRequestReturnProjectile += ReturnProjectile;
    }

    private void OnDisable()
    {
        EventManager.OnRequestSpawnProjectile -= HandleSpawnProjectile;
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
}