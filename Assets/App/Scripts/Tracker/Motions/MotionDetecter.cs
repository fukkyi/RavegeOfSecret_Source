using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.Azure.Kinect.BodyTracking;

public abstract class MotionDetecter : MonoBehaviour
{
    /// <summary>
    /// ���[�V�������m�Ɏg�p����̂̈ʒu
    /// </summary>
    protected enum DetectSide
    {
        Left,
        Right
    }

    protected TrackingState detectTrackingState = null;
    protected UnityEvent onDetectMotionEvent = new UnityEvent();
    protected List<UnityAction> onceActionList = new List<UnityAction>();

    public bool IsDetectedMotion { get; protected set; }

    /// <summary>
    /// ���[�V�������m��Ԃ��X�V����
    /// </summary>
    public void UpdateDetecter(TrackingState trackingState)
    {
        detectTrackingState = trackingState;

        bool isDetectMotion = CheckMotion();

        if (!IsDetectedMotion && isDetectMotion)
        {
            InvokeDetectedAction();
        }

        IsDetectedMotion = isDetectMotion;
    }

    /// <summary>
    /// ���҂������[�V���������Ă��邩
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckMotion();

    /// <summary>
    /// ���[�V���������m�����ۂɎ��s����A�N�V������ǉ�����
    /// </summary>
    /// <param name="action"></param>
    public void AddDetectedAction(UnityAction action)
    {
        onDetectMotionEvent.AddListener(action);
    }

    /// <summary>
    /// ���[�V���������m�����ۂɈ�x�̂ݎ��s����A�N�V������ǉ�����
    /// </summary>
    /// <param name="action"></param>
    public void AddDetectedOnceAction(UnityAction action)
    {
        onDetectMotionEvent.AddListener(action);
        onceActionList.Add(action);
    }

    /// <summary>
    /// �w�肵���A�N�V��������菜��
    /// </summary>
    /// <param name="action"></param>
    public void RemoveDetectedAction(UnityAction action)
    {
        onDetectMotionEvent.RemoveListener(action);
        onceActionList.Remove(action);
    }

    /// <summary>
    /// ���m�����ۂ̃A�N�V������S�ď���
    /// </summary>
    public void ClearDetectedAction()
    {
        onDetectMotionEvent.RemoveAllListeners();
        onceActionList.Clear();
    }

    public void CopyDetectActionToDetecter(MotionDetecter copyToDetecter)
    {

    }

    /// <summary>
    /// ���[�V���������m�������̃A�N�V���������s����
    /// </summary>
    protected void InvokeDetectedAction()
    {
        onDetectMotionEvent.Invoke();
        // ��x�̂ݎ��s����Action��r������
        onceActionList.ForEach((UnityAction onceAction) => {
            onDetectMotionEvent.RemoveListener(onceAction); 
        });
        onceActionList.Clear();
    }

    /// <summary>
    /// �w�肵�����E�ǂ��炩�̊֐߈ʒu���擾
    /// </summary>
    /// <param name="detectSide"></param>
    /// <param name="leftJoint"></param>
    /// <param name="rightJoint"></param>
    /// <returns></returns>
    protected Vector3 SelectJointPosByDetectSide(DetectSide detectSide, JointId leftJoint, JointId rightJoint)
    {
        return (detectSide == DetectSide.Left) ?
            detectTrackingState.TrackingBody.GetConvertJointPositions3D(leftJoint) :
            detectTrackingState.TrackingBody.GetConvertJointPositions3D(rightJoint);
    }

    /// <summary>
    /// �w�肵�����E�ǂ��炩�̊֐ߊp�x���擾
    /// </summary>
    /// <param name="detectSide"></param>
    /// <param name="leftJoint"></param>
    /// <param name="rightJoint"></param>
    /// <returns></returns>
    protected Quaternion SelectJointRotByDetectSide(DetectSide detectSide, JointId leftJoint, JointId rightJoint)
    {
        return (detectSide == DetectSide.Left) ?
            detectTrackingState.TrackingBody.GetConvertJointRotation(leftJoint) :
            detectTrackingState.TrackingBody.GetConvertJointRotation(rightJoint);
    }

    /// <summary>
    /// �w�肵�����E�ǂ��炩�̊֐߈ʒu�������擾����
    /// </summary>
    /// <param name="detectSide"></param>
    /// <param name="leftJoint"></param>
    /// <param name="rightJoint"></param>
    /// <returns></returns>
    protected Vector3 SelectJointDiffByDetectSide(DetectSide detectSide, JointId leftJoint, JointId rightJoint)
    {
        return (detectSide == DetectSide.Left) ?
            detectTrackingState.LastFlameDiffBody.GetConvertJointPositions3D(leftJoint) :
            detectTrackingState.LastFlameDiffBody.GetConvertJointPositions3D(rightJoint);
    }
}
