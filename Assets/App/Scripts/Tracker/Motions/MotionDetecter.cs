using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.Azure.Kinect.BodyTracking;

public abstract class MotionDetecter : MonoBehaviour
{
    /// <summary>
    /// モーション検知に使用する体の位置
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
    /// モーション検知状態を更新する
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
    /// 期待したモーションをしているか
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckMotion();

    /// <summary>
    /// モーションを検知した際に実行するアクションを追加する
    /// </summary>
    /// <param name="action"></param>
    public void AddDetectedAction(UnityAction action)
    {
        onDetectMotionEvent.AddListener(action);
    }

    /// <summary>
    /// モーションを検知した際に一度のみ実行するアクションを追加する
    /// </summary>
    /// <param name="action"></param>
    public void AddDetectedOnceAction(UnityAction action)
    {
        onDetectMotionEvent.AddListener(action);
        onceActionList.Add(action);
    }

    /// <summary>
    /// 指定したアクションを取り除く
    /// </summary>
    /// <param name="action"></param>
    public void RemoveDetectedAction(UnityAction action)
    {
        onDetectMotionEvent.RemoveListener(action);
        onceActionList.Remove(action);
    }

    /// <summary>
    /// 検知した際のアクションを全て消す
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
    /// モーションを検知した時のアクションを実行する
    /// </summary>
    protected void InvokeDetectedAction()
    {
        onDetectMotionEvent.Invoke();
        // 一度のみ実行するActionを排除する
        onceActionList.ForEach((UnityAction onceAction) => {
            onDetectMotionEvent.RemoveListener(onceAction); 
        });
        onceActionList.Clear();
    }

    /// <summary>
    /// 指定した左右どちらかの関節位置を取得
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
    /// 指定した左右どちらかの関節角度を取得
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
    /// 指定した左右どちらかの関節位置差分を取得する
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
