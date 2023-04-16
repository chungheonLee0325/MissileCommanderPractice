using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager 
{
    Building prefab;
    Transform[] buildingLocators;
    Factory effectFactory;

    List<Building> buildings = new List<Building>();

    public bool HasBuilding { get { return buildings.Count > 0; } }

    public int BuildingCount { get { return buildings.Count; } }

    public Action AllBuildingsDestroyed;

    public BuildingManager(Building prefab, Transform[] buildingLocators, Factory effectFactory)
    {
        this.prefab = prefab;
        this.buildingLocators = buildingLocators;
        this.effectFactory = effectFactory;

        Debug.Assert(this.prefab != null, "null building prefabs!");
        Debug.Assert(this.buildingLocators != null, "null buildingLocators!");
        Debug.Assert(this.effectFactory != null, "null effectFactory!");
    }

    void CreateBuildings()
    {
        if(buildings.Count > 0)
        {
            Debug.LogWarning("Buildings have been already created!");
            return;
        }
        for(int i = 0;i<buildingLocators.Length;i++)
        {
            Building building = GameObject.Instantiate(prefab);
            building.transform.position = buildingLocators[i].position;
            building.Destroyed += this.OnBuildingDestroyed;
            buildings.Add(building);
        }
    }
    void OnBuildingDestroyed(Building building)
    {
        AudioManager.instance.PlaySound(SoundId.BuildingExplosion);

        Vector3 lastPosition = building.transform.position;

        building.Destroyed -= this.OnBuildingDestroyed;
        int index = buildings.IndexOf(building);
        buildings.RemoveAt(index);
        GameObject.Destroy(building.gameObject);

        RecycleObject effect = effectFactory.Get();
        effect.Activate(lastPosition);
        effect.Destroyed += this.OnEffectDestroyed;

        if(buildings.Count == 0)
        {
            AllBuildingsDestroyed?.Invoke();
        }
    }
    void OnEffectDestroyed(RecycleObject effect)
    {
        effect.Destroyed -= this.OnEffectDestroyed;
        effectFactory.Restore(effect);
    }
    public void OnGameStarted()
    {
        CreateBuildings();
    }
    public Vector3 GetRandomBuildingPosition()
    {
        Debug.Assert(buildings.Count > 0, "no element in buildings!");

        Building building = buildings[UnityEngine.Random.Range(0, buildings.Count)];
        return building.transform.position;
    }
}
