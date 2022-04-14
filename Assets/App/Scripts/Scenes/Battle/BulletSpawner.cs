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
    /// objPool����I�u�W�F�N�g���擾
    /// prefab��ray����ɔ�΂�
    /// prefab�̏����ʒu�ݒ�
    /// �e����炷
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
