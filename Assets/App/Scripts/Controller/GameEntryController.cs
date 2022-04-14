using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntryController : MonoBehaviour
{
    private bool isStarted = false;

    void Update()
    {
        if (!isStarted && IsExistsAllManagersInstance())
        {
            SceneTransitionManager.Instance.TransitionByName("Title", TransitionType.FadeInOut, fadeOutTime: 0.1f);
            isStarted = true;
        }
    }

    /// <summary>
    /// マネージャークラスのインスタンスが全て存在するかチェックする
    /// </summary>
    /// <returns></returns>
    private bool IsExistsAllManagersInstance()
    {
        return
            AudioManager.Instance != null &&
            TrackingManager.Instance != null &&
            ScoreManager.Instance != null &&
            SceneTransitionManager.Instance != null &&
            EffectManager.Instance != null;
    }
}
