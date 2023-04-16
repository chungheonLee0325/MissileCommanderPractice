using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    BulletLauncher launcherPrefab;
    BulletLauncher launcher;

    [SerializeField]
    Transform launcherLocator;

    [SerializeField]
    Building buildingPrefab;

    [SerializeField]
    Transform[] buildingLocators;

    [SerializeField]
    Missile missilePrefab;

    [SerializeField]
    DestroyEffect effectPrefab;

    [SerializeField]
    int maxMissileCount = 20;

    [SerializeField]
    float missileSpawnInterval = 0.5f;

    [SerializeField]
    int scorePerMissile = 50;

    [SerializeField]
    int scorePerBuilding = 5000;

    [SerializeField]
    UIRoot uIRoot;

    bool isAllBuildingDestroyed = false;

    public Action<bool, int> GameEnded; // true : 승리, false : 패배 / 최종 점수 계산을 위한 남은 빌딩 수

    MouseGameController mouseGameController;
    BuildingManager buildingManager;
    TimeManager timeManager;
    MissileManager missileManager;
    ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        launcher = Instantiate(launcherPrefab);
        launcher.transform.position = launcherLocator.position;

        mouseGameController = gameObject.AddComponent<MouseGameController>();
        buildingManager = new BuildingManager(buildingPrefab, buildingLocators, new Factory(effectPrefab, 2));
        timeManager = gameObject.AddComponent<TimeManager>();
        missileManager = gameObject.AddComponent<MissileManager>();
        missileManager.Initialize(new Factory(missilePrefab), buildingManager, maxMissileCount, missileSpawnInterval);
        scoreManager = new ScoreManager(scorePerMissile, scorePerBuilding);

        BindEvent();

        timeManager.StartGame(1.5f);
    }

    void BindEvent()
    {
        mouseGameController.FireButtonPressed += launcher.OnFireButtonPressed;
        timeManager.GameStarted += launcher.OnGameStarted;
        timeManager.GameStarted += buildingManager.OnGameStarted;
        timeManager.GameStarted += missileManager.OnGameStarted;
        timeManager.GameStarted += uIRoot.OnGameStarted;
        missileManager.MissileDestroyed += scoreManager.OnMissileDestroyed;
        scoreManager.ScoreChanged += uIRoot.OnScoreChanged;
        buildingManager.AllBuildingsDestroyed += OnAllBuildingDestroyed;
        missileManager.AllMissileDestroyed += OnAllMissileDestroyed;

        this.GameEnded += launcher.OnGameEnded;
        this.GameEnded += missileManager.OnGameEnded;
        this.GameEnded += scoreManager.OnGameEnded;
        this.GameEnded += uIRoot.OnGameEnded;
    }
    void UnBindEvent()
    {
        mouseGameController.FireButtonPressed -= launcher.OnFireButtonPressed;
        timeManager.GameStarted -= launcher.OnGameStarted;
        timeManager.GameStarted -= buildingManager.OnGameStarted;
        timeManager.GameStarted -= missileManager.OnGameStarted;
        timeManager.GameStarted -= uIRoot.OnGameStarted;
        missileManager.MissileDestroyed -= scoreManager.OnMissileDestroyed;
        scoreManager.ScoreChanged -= uIRoot.OnScoreChanged;
        buildingManager.AllBuildingsDestroyed -= OnAllBuildingDestroyed;
        missileManager.AllMissileDestroyed -= OnAllMissileDestroyed;

        this.GameEnded -= launcher.OnGameEnded;
        this.GameEnded -= missileManager.OnGameEnded;
        this.GameEnded -= scoreManager.OnGameEnded;
        this.GameEnded -= uIRoot.OnGameEnded;
    }
    void OnDestroy()
    {
        UnBindEvent(); 
    }
    void OnAllMissileDestroyed()
    {
        StartCoroutine(DelayedGameEnded());
    }
    void OnAllBuildingDestroyed()
    {
        isAllBuildingDestroyed = true;
        GameEnded?.Invoke(false, buildingManager.BuildingCount);
        AudioManager.instance.PlaySound(SoundId.GameEnd);
    }

    IEnumerator DelayedGameEnded()
    {
        yield return null;

        if(!isAllBuildingDestroyed)
        {
            GameEnded?.Invoke(true, buildingManager.BuildingCount);
            AudioManager.instance.PlaySound(SoundId.GameEnd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
