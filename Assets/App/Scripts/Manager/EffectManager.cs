using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : AutoGenerateBaseManager<EffectManager>
{
    [SerializeField]
    private ObjectPool[] particlePools = new ObjectPool[0];
    [SerializeField]
    private Canvas screenCrackCanvas = null;
    [SerializeField]
    private ObjectPool screenCrackPool = null;

    /// <summary>
    /// �}�l�[�W���[�𐶐����� (�Q�[�����[�h���Ɏ����ŌĂ΂��)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GenerateThisManager()
    {
        GenerateManager();
    }

    /// <summary>
    /// �p�[�e�B�N���𐶐�����
    /// </summary>
    /// <param name="particleType"></param>
    /// <param name="position"></param>
    public void PlayParticle(ParticleType particleType, Vector3 position, Quaternion rotation)
    {
        int poolIndex = (int)particleType;

        if (poolIndex < 0 && poolIndex >= particlePools.Length) return;

        ObjectPool particlePool = particlePools[(int)particleType];
        ParticleObject particle = particlePool.GetObject<ParticleObject>();

        particle.PlayOfPosition(position, rotation);
    }

    /// <summary>
    /// �_���[�W�G�t�F�N�g���Đ�����
    /// </summary>
    public void PlayDamageEffect()
    {
        Vector2 displayPos = TransformUtil.GetRandomPotisionOnRect(screenCrackCanvas.pixelRect);
        ScreenCrack screenCrack = screenCrackPool.GetObject<ScreenCrack>();

        AudioManager.Instance.PlaySE("GlassCrack");

        screenCrack.DisplayOfPotision(displayPos);
    }
}

public enum ParticleType
{
    DroneExplosion = 0,
    BulletCollision = 1,
}