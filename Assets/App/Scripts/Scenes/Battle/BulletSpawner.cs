using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] public Transform rightSpawnPoint;
    [SerializeField] public Transform leftSpawnPoint;
    [SerializeField] private ObjectPool playerBulletPool;
    [SerializeField] private float SpawnIntervallTime = 5f;
    private float SpawnElapsedTime = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SpawnElapsedTime += Time.deltaTime;
        if (SpawnElapsedTime >= SpawnIntervallTime)
        {
            if (TrackingManager.Instance.MainTrackingState.IsRightAimOnScreen)
                RightBulletSpawn();
            if(TrackingManager.Instance.MainTrackingState.IsLeftAimOnscreen)
                LeftBulletSpawn();
            SpawnElapsedTime = 0f;
        }
    }

    /// <summary>
    /// objPoolからオブジェクトを取得
    /// prefabをray線上に飛ばす
    /// prefabの初期位置設定
    /// 銃声を鳴らす
    /// </summary>
    private void RightBulletSpawn()
    {
        PlayerBullet bullet = playerBulletPool.GetObject<PlayerBullet>();
        bullet.RightBulletMove(rightSpawnPoint);
        bullet.RightSetSpawn(rightSpawnPoint);
        AudioManager.Instance.PlaySE("Shot");
    }
    private void LeftBulletSpawn()
    {
        PlayerBullet bullet = playerBulletPool.GetObject<PlayerBullet>();
        bullet.LeftBulletMove(leftSpawnPoint);
        bullet.LeftSetSpawn(leftSpawnPoint);
        AudioManager.Instance.PlaySE("Shot");
    }
}
