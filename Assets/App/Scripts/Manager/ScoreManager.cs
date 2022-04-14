using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : AutoGenerateBaseManager<ScoreManager>
{
    public bool IsMeasurementPlayTime { get; private set; } = false;
    public float PlayTime { get; private set; } = 0;
    public int PlayScore { get; private set; } = 0;
    public int DamegeCount { get; private set; } = 0;

    [SerializeField]
    private int maxScore = 10000;
    [SerializeField]
    private int scoreForOneSecond = 17;
    [SerializeField]
    private int damageScore = -500;
    [SerializeField]
    private int borderScoreRankS = 0;
    [SerializeField]
    private int borderScoreRankA = 0;
    [SerializeField]
    private int borderScoreRankB = 0;

    /// <summary>
    /// �}�l�[�W���[�𐶐����� (�Q�[�����[�h���Ɏ����ŌĂ΂��)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GenerateThisManager()
    {
        GenerateManager();
    }

    private void Update()
    {
        UpdatePlayScore();
    }

    /// <summary>
    /// �_���[�W���󂯂��񐔂𑝂₷
    /// </summary>
    public void AddDamageCount()
    {
        DamegeCount++;
    }

    /// <summary>
    /// �Q�[���v���C���Ԃ̌v�����J�n����
    /// </summary>
    public void StartMeasurePlayTime()
    {
        IsMeasurementPlayTime = true;
    }

    /// <summary>
    /// �Q�[���v���C���Ԃ̌v�����I������
    /// </summary>
    public void StopMeasurePlayTime()
    {
        IsMeasurementPlayTime = false;
    }

    /// <summary>
    /// �X�R�A�����Z�b�g����
    /// </summary>
    public void ResetScore()
    {
        PlayTime = 0;
        PlayScore = 0;
        DamegeCount = 0;
    }

    /// <summary>
    /// �v���C���Ԃ��烉���N���擾����
    /// </summary>
    /// <returns></returns>
    public PlayRank GetPlayRank()
    {
        if (PlayScore >= borderScoreRankS)
        {
            return PlayRank.S;
        }
        else if (PlayScore >= borderScoreRankA)
        {
            return PlayRank.A;
        }
        else if (PlayScore >= borderScoreRankB)
        {
            return PlayRank.B;
        }

        return PlayRank.C;
    }

    /// <summary>
    /// �v���C���Ԃ�TimeSpan�`���Ŏ擾����
    /// </summary>
    /// <returns></returns>
    public TimeSpan GetPlayRankForTimeSpan()
    {
        return TimeSpan.FromSeconds(PlayTime);
    }

    private void UpdatePlayScore()
    {
        UpdatePlayTime();

        int timeScore = maxScore - Mathf.FloorToInt(PlayTime) * scoreForOneSecond;
        int damagedScore = timeScore + damageScore * DamegeCount;

        PlayScore = Mathf.Clamp(damagedScore, 0, maxScore);
    }

    /// <summary>
    /// �v���C���Ԃ��X�V����
    /// </summary>
    private void UpdatePlayTime()
    {
        if (!IsMeasurementPlayTime) return;

        PlayTime += Time.deltaTime;
    }
}

public enum PlayRank
{
    S,
    A,
    B,
    C
}