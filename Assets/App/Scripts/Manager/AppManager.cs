using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : AutoGenerateBaseManager<AppManager>
{
    private static string ScreenShotFileName = "ScreenShot";
    private static string ScreenShotsBaseAddress = "Assets/ScreenShots";

    [SerializeField]
    private float resetPushingTime = 1.0f;

    private float currentResetPushingTime = 0;

    /// <summary>
    /// マネージャーを生成する (ゲームロード時に自動で呼ばれる)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GenerateThisManager()
    {
        GenerateManager();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateResetInput();
#if UNITY_EDITOR
        UpdateScreenShot();
#endif
    }

    /// <summary>
    /// リセット入力の更新を行う
    /// </summary>
    private void UpdateResetInput()
    {
        if (Input.GetKey(KeyCode.R))
        {
            currentResetPushingTime += Time.unscaledDeltaTime;
        }
        else
        {
            currentResetPushingTime = 0;
        }

        if (currentResetPushingTime >= resetPushingTime)
        {
            ResetApp();
            currentResetPushingTime = float.MinValue;
        }
    }

    /// <summary>
    /// ゲームをリセットする
    /// </summary>
    public void ResetApp()
    {
        ScoreManager.Instance.StopMeasurePlayTime();
        ScoreManager.Instance.ResetScore();
        AudioManager.Instance.StopCurrentBGMWithFade();
        TrackingManager.Instance.ClearAllMotionAction();
        SceneTransitionManager.Instance.TransitionByName("Title", TransitionType.FadeInOut);
    }

    public void UpdateScreenShot()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TakeScreenShot();
        }
    }

    private void TakeScreenShot()
    {
        DateTime nowTime = DateTime.Now;
        string nowTimeText = nowTime.ToString("yyyy-MM-dd hh-mm-ss");
        string savePath = $"{ScreenShotsBaseAddress}/{nowTimeText} {ScreenShotFileName}.png";

        ScreenCapture.CaptureScreenshot(savePath);
        Debug.Log($"ScreenShot Successful [{savePath}]");
    }
}
