using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


public class MotionUpdater : MonoBehaviour
{
    [SerializeField]
    private Transform motionParent = null;

    private List<MotionDetecter> motionDetecterList = new List<MotionDetecter>();

    // Start is called before the first frame update
    void Awake()
    {
        if (motionParent == null)
        {
            motionParent = transform;
        }

        foreach (MotionDetecter motionDetecter in motionParent.GetComponentsInChildren<MotionDetecter>())
        {
            motionDetecterList.Add(motionDetecter);
        }
    }

    /// <summary>
    /// �g���b�J�[�̃��[�V�������m���X�V����
    /// </summary>
    public void UpdateMotionDetectersByTracker(TrackingState state)
    {
        motionDetecterList.ForEach((detecter) => {
            detecter.UpdateDetecter(state);
        });
    }

    /// <summary>
    /// �����g���b�J�[�̃��[�V�������m���X�V����
    /// </summary>
    public void UpdateMotionDetectersByTrackers(TrackingState[] states)
    {
        foreach(TrackingState state in states)
        {
            UpdateMotionDetectersByTracker(state);
        }
    }

    /// <summary>
    /// �^���烂�[�V�����f�B�e�N�^�[���擾����
    /// </summary>
    /// <param name="motionDetecter"></param>
    /// <returns></returns>
    private MotionDetecter FindDetecterByType(Type detecterType)
    {
        return motionDetecterList.Find(detecter => detecter.GetType() == detecterType);
    }

    /// <summary>
    /// ���[�V���������m�����ۂ̃C�x���g��ǉ�����
    /// </summary>
    /// <param name="addDetecter"></param>
    /// <param name="action"></param>
    public void AddMotionDetectedAction(Type detecterType, UnityAction action)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.AddDetectedAction(action);
    }

    /// <summary>
    /// ���[�V���������m�����ۂ̈�x����̃C�x���g��ǉ�����
    /// </summary>
    /// <param name="addDetecter"></param>
    /// <param name="action"></param>
    public void AddMotionDetectedOnceAction(Type detecterType, UnityAction action)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.AddDetectedOnceAction(action);
    }

    /// <summary>
    /// ���[�V���������m�����ۂ̃C�x���g��S�ď���
    /// </summary>
    /// <param name="addDetecter"></param>
    public void ClearMotionDetectedAction(Type detecterType)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.ClearDetectedAction();
    }

    /// <summary>
    /// �S�Ẵ��[�V�������m�̃C�x���g��S�ď���
    /// </summary>
    public void ClearAllMotionDetectedAction()
    {
        motionDetecterList.ForEach((detecter) => {
            ClearMotionDetectedAction(detecter.GetType());
        });
    }

    /// <summary>
    /// ���[�V���������m�����ۂ̃C�x���g����菜��
    /// </summary>
    /// <param name="addDetecter"></param>
    /// <param name="action"></param>
    public void RemoveMotionDetectedAction(Type detecterType, UnityAction action)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.RemoveDetectedAction(action);
    }
}
