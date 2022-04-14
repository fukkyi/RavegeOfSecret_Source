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
    /// マネージャーを生成する (ゲームロード時に自動で呼ばれる)
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
    /// ダメージを受けた回数を増やす
    /// </summary>
    public void AddDamageCount()
    {
        DamegeCount++;
    }

    /// <summary>
    /// ゲームプレイ時間の計測を開始する
    /// </summary>
    public void StartMeasurePlayTime()
    {
        IsMeasurementPlayTime = true;
    }

    /// <summary>
    /// ゲームプレイ時間の計測を終了する
    /// </summary>
    public void StopMeasurePlayTime()
    {
        IsMeasurementPlayTime = false;
    }

    /// <summary>
    /// スコアをリセットする
    /// </summary>
    public void ResetScore()
    {
        PlayTime = 0;
        PlayScore = 0;
        DamegeCount = 0;
    }

    /// <summary>
    /// プレイ時間からランクを取得する
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
    /// プレイ時間をTimeSpan形式で取得する
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
    /// プレイ時間を更新する
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