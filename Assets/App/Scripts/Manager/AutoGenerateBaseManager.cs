using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public abstract class AutoGenerateBaseManager<T> : MonoBehaviour where T : AutoGenerateBaseManager<T>
{
    public static T Instance { get; private set; } = null;

    protected static string ManagerAssetBaseAddress = "Assets/App/Prefabs/Managers/";
    protected static string ManagerAssetMineType = ".prefab";

    /// <summary>
    /// �}�l�[�W���[�𐶐����� (�Q�[�����[�h���Ɏ����ŌĂ΂��)
    /// </summary>
    protected static void GenerateManager()
    {
        if (Instance != null) return;

        Addressables.LoadAssetAsync<GameObject>(GenerateManagerAddress()).Completed += OnManagerLoaded;
    }

    /// <summary>
    /// Addressable�œǂݍ��܂ꂽ�I�u�W�F�N�g����TrackingManager�̃C���X�^���X��ǂݍ���
    /// </summary>
    /// <param name="handle"></param>
    protected static void OnManagerLoaded(AsyncOperationHandle<GameObject> handle)
    {
        Instance = Instantiate(handle.Result).GetComponent<T>();

        DontDestroyOnLoad(Instance);
    }

    /// <summary>
    /// Addressable�p�ɃA�h���X�𐶐�����
    /// </summary>
    /// <returns></returns>
    protected static string GenerateManagerAddress()
    {
        Type managerType = typeof(T);
        // [Manager��Prefab������t�H���_]+[�N���X��]+[.prefab]
        return ManagerAssetBaseAddress + managerType.Name + ManagerAssetMineType;
    }
}
