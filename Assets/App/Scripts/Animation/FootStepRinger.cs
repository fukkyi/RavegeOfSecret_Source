using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepRinger : MonoBehaviour
{
    /// <summary>
    /// �A�j���[�V�����C�x���g�o�R�ő�����炷
    /// </summary>
    private void PlayFootStep()
    {
        AudioManager.Instance.PlayRandomPitchSE("Wood footstep 6");
    }
}
