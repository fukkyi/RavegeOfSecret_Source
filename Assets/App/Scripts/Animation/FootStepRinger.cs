using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepRinger : MonoBehaviour
{
    /// <summary>
    /// アニメーションイベント経由で足音を鳴らす
    /// </summary>
    private void PlayFootStep()
    {
        AudioManager.Instance.PlayRandomPitchSE("Wood footstep 6");
    }
}
