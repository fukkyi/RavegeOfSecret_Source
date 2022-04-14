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
    /// マネージャーを生成する (ゲームロード時に自動で呼ばれる)
    /// </summary>
    protected static void GenerateManager()
    {
        if (Instance != null) return;

        Addressables.LoadAssetAsync<GameObject>(GenerateManagerAddress()).Completed += OnManagerLoaded;
    }

    /// <summary>
    /// Addressableで読み込まれたオブジェクトからTrackingManagerのインスタンスを読み込む
    /// </summary>
    /// <param name="handle"></param>
    protected static void OnManagerLoaded(AsyncOperationHandle<GameObject> handle)
    {
        Instance = Instantiate(handle.Result).GetComponent<T>();

        DontDestroyOnLoad(Instance);
    }

    /// <summary>
    /// Addressable用にアドレスを生成する
    /// </summary>
    /// <returns></returns>
    protected static string GenerateManagerAddress()
    {
        Type managerType = typeof(T);
        // [ManagerのPrefabがあるフォルダ]+[クラス名]+[.prefab]
        return ManagerAssetBaseAddress + managerType.Name + ManagerAssetMineType;
    }
}
