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
    /// �h���[�����o������|�C���g������������
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
        // �h���[�����o������ő吔�ɒB���Ă���ꍇ�̓X�|�[�������Ȃ�
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
    /// �X�|�[���p�̃V���b�t�����ꂽ�C���f�b�N�X��Ԃ�
    /// </summary>
    /// <param name="spawnPointsLength"></param>
    /// <returns></returns>
    private int[] GetShuffledSpawnPointIndexs(int spawnPointsLength)
    {
        // �X�|�[���|�C���g���̕��̃C���f�b�N�X�z���p�ӂ���
        int[] indexs = new int[spawnPointsLength];
        for (int i = 0; i < indexs.Length; i++)
        {
            indexs[i] = i;
        }
        // �C���f�b�N�X�z����V���b�t��
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
    /// �o�����Ă���h���[���̐����擾����
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
    /// ���j����1���Z����
    /// </summary>
    private void IncDefeatCount()
    {
        DefeatCount++;
    }

}
