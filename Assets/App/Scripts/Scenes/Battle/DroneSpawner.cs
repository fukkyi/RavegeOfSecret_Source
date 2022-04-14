using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DroneSpawner : MonoBehaviour
{
    [SerializeField]
    private Drone airDrone = null;
    [SerializeField]
    private Transform airDroneSpawnPointsParent = null;
    [SerializeField]
    private MinMaxRange droneSpawnInterval = new MinMaxRange(0, 20.0f);
    [SerializeField]
    private int maxSpawnDroneCount = 2;
    [SerializeField]
    private float clearElapsedTime = 60f;
    [SerializeField]
    private float droneAttackPreparationTime = 2;
    private BattleController battle;
    // private Transform[] airDroneSpawnPoints = null;
    private Transform[] airDroneSpawnPoints = null;
    private Drone[] spawnedAirDrones = null;
    public int DefeatCount { get; private set; }
    private float nextDroneSpawnTime = 0;

    private void Start()
    {
        InitSpawnPoints();
        nextDroneSpawnTime = 2;
    }

    private void Update()
    {
        UpdateDroneSpawn();
    }

    /// <summary>
    /// ドローンが出現するポイントを初期化する
    /// </summary>
    private void InitSpawnPoints()
    {
        int airDroneSpawnPointCount = airDroneSpawnPointsParent.childCount;
        airDroneSpawnPoints = new Transform[airDroneSpawnPointCount];
        spawnedAirDrones = new Drone[airDroneSpawnPointCount];

        for (int i = 0; i < airDroneSpawnPointCount; i++)
        {
            airDroneSpawnPoints[i] = airDroneSpawnPointsParent.GetChild(i);
        }
    }

    private void UpdateDroneSpawn()
    {
        if (nextDroneSpawnTime <= 0)
        {
            nextDroneSpawnTime = droneSpawnInterval.RandOfRange();
            SpawnDrone();
        }
        else
        {
            nextDroneSpawnTime -= Time.deltaTime;
        }
    }

    private void SpawnDrone()
    {
        // ドローンが出現する最大数に達している場合はスポーンさせない
        if (maxSpawnDroneCount <= GetSpawnedDroneCount()) return;

        SpawnAirDrone();
        AudioManager.Instance.PlaySE("EnemySpawn");
    }

    private void SpawnAirDrone()
    {
        int[] shuffledSpawnPointIndexs = GetShuffledSpawnPointIndexs(airDroneSpawnPoints.Length);

        for (int i = 0; i < airDroneSpawnPoints.Length; i++)
        {
            int pointIndex = shuffledSpawnPointIndexs[i];
            if (spawnedAirDrones[pointIndex] != null) continue;

            spawnedAirDrones[pointIndex] = Instantiate(airDrone, airDroneSpawnPoints[pointIndex]);
            spawnedAirDrones[pointIndex].Init(IncDefeatCount, droneAttackPreparationTime);
            break;
        }
    }

    /// <summary>
    /// スポーン用のシャッフルされたインデックスを返す
    /// </summary>
    /// <param name="spawnPointsLength"></param>
    /// <returns></returns>
    private int[] GetShuffledSpawnPointIndexs(int spawnPointsLength)
    {
        // スポーンポイント数の分のインデックス配列を用意する
        int[] indexs = new int[spawnPointsLength];
        for (int i = 0; i < indexs.Length; i++)
        {
            indexs[i] = i;
        }
        // インデックス配列をシャッフル
        for (int i = indexs.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = indexs[i];
            indexs[i] = indexs[j];
            indexs[j] = tmp;
        }

        return indexs;
    }

    /// <summary>
    /// 出現しているドローンの数を取得する
    /// </summary>
    /// <returns></returns>
    private int GetSpawnedDroneCount()
    {
        int droneCount = 0;
        foreach (Drone drone in spawnedAirDrones)
        {
            if (drone != null)
            {
                droneCount++;
            }
        }
        return droneCount;
    }

    /// <summary>
    /// 撃破数を1つ加算する
    /// </summary>
    private void IncDefeatCount()
    {
        DefeatCount++;
    }

}
