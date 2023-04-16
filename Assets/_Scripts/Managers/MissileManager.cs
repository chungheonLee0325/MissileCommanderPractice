using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    Factory missileFactory;
    BuildingManager buildingManager;

    bool isInitialized = false;

    int maxMissileCount = 20;
    int currentMissileCount;

    float missileSpawnInterval = 0.5f;

    Coroutine spawningMissile;

    List<RecycleObject> missiles = new List<RecycleObject>();

    public Action MissileDestroyed;
    public Action AllMissileDestroyed;

    public void Initialize(Factory missileFactory, BuildingManager buildingManager, int maxMissileCount, float missileSpawnInterval)
    {
        if (isInitialized)
            return;
        isInitialized = true;

        this.missileFactory = missileFactory;
        this.buildingManager = buildingManager;
        this.maxMissileCount = maxMissileCount;
        this.missileSpawnInterval = missileSpawnInterval;
    }
    void SpawnMissile()
    {
        Debug.Assert(this.missileFactory != null, "missile factory is null!");
        Debug.Assert(this.buildingManager != null, "building manager is null!");

        RecycleObject missile = missileFactory.Get();
        missile.Activate(GetMissileSpawanPosition(), buildingManager.GetRandomBuildingPosition());
        missile.Destroyed += this.OnMissileDestroyed;
        missile.OutOfScreen += this.OnMissileOutOfScreen;
        missiles.Add(missile);

        currentMissileCount++;
    }
    Vector3 GetMissileSpawanPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = UnityEngine.Random.Range(0f, 1f);
        spawnPosition.y = 1.1f;

        spawnPosition = Camera.main.ViewportToWorldPoint(spawnPosition);
        spawnPosition.z = 0f;
        return spawnPosition;
    }
    public void OnGameStarted()
    {
        currentMissileCount = 0;
        spawningMissile = StartCoroutine(AutoSpawnMissile());
    }

    IEnumerator AutoSpawnMissile()
    {
        while(currentMissileCount < maxMissileCount)
        {
            yield return new WaitForSeconds(missileSpawnInterval);

            if (!buildingManager.HasBuilding)
            {
                Debug.LogWarning("all buildings are destroyed");
                yield break;
            }

            SpawnMissile();
        }
    }
    void OnMissileDestroyed(RecycleObject missile)
    {
        RestoreMissile(missile);
        MissileDestroyed?.Invoke();
    }
    void OnMissileOutOfScreen(RecycleObject missile)
    {
        RestoreMissile(missile);
    }
    void RestoreMissile(RecycleObject missile)
    {
        missile.Destroyed -= this.OnMissileDestroyed;
        missile.OutOfScreen -= this.OnMissileOutOfScreen;
        int index = missiles.IndexOf(missile);
        missiles.RemoveAt(index);
        missileFactory.Restore(missile);

        CheckAllMissileRestored();
    }

    void CheckAllMissileRestored()
    {
        if(currentMissileCount == maxMissileCount && missiles.Count == 0)
        {
            AllMissileDestroyed?.Invoke();
        }
    }
    public void OnGameEnded(bool isVictory, int buildingCount)
    {
        if (missiles.Count == 0)
            return;

        foreach(var missile in missiles)
        {
            missileFactory.Restore(missile);
        }
    }
}
