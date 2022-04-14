using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

/// <summary>
/// 左から右に手を薙ぎ払うモーションのディテクター
/// </summary>
public class RightSlashHandDetecter : MotionDetecter
{
    // 薙ぎ払いを検知する時間
    [SerializeField]
    private float slashDetectTime = 0.5f;
    // 薙ぎ払いを検知する長さ
    [SerializeField]
    private float slashDetectDistance = 0.25f;
    // 薙ぎ払いを検知する手の速さ
    [SerializeField]
    private float slashDetectHandSpeed = 0.005f;

    private bool isValidRightHandHeight = false;
    private bool isValidLeftHandHeight = false;
    private bool isRightSlashStarted = false;
    private bool isLeftSlashStarted = false;
    private float rightSlashTime = 0;
    private float leftSlashTime = 0;
    private Vector3 rightSlashStartPos = Vector3.zero;
    private Vector3 leftSlashStartPos = Vector3.zero;

    protected override bool CheckMotion()
    {
        Vector3 handRightPos = detectTrackingState.TrackingBody.GetConvertJointPositions3D(JointId.HandRight);
        Vector3 elbowRightPos = detectTrackingState.TrackingBody.GetConvertJointPositions3D(JointId.ElbowRight);
        Vector3 handLeftPos = detectTrackingState.TrackingBody.GetConvertJointPositions3D(JointId.HandLeft);
        Vector3 elbowLeftPos = detectTrackingState.TrackingBody.GetConvertJointPositions3D(JointId.ElbowLeft);
        Vector3 handRightDiff = detectTrackingState.LastFlameDiffBody.GetConvertJointPositions3D(JointId.HandRight);
        Vector3 handLeftDiff = detectTrackingState.LastFlameDiffBody.GetConvertJointPositions3D(JointId.HandLeft);

        bool detectedRightMotion = CheckOnceHandMotion(detectTrackingState, handRightPos, elbowRightPos, handRightDiff,
            ref isValidRightHandHeight, ref isRightSlashStarted, ref rightSlashTime, ref rightSlashStartPos);
        bool detectedLeftMotion = CheckOnceHandMotion(detectTrackingState, handLeftPos, elbowLeftPos, handLeftDiff,
            ref isValidLeftHandHeight, ref isLeftSlashStarted, ref leftSlashTime, ref leftSlashStartPos);
        // 右腕か左腕で薙ぎ払っていれば検知とみなす
        return (detectedRightMotion || detectedLeftMotion);
    }

    /// <summary>
    /// 片腕の薙ぎ払いモーションの検知を行う
    /// </summary>
    /// <param name="trackingState"></param>
    /// <param name="handPos"></param>
    /// <param name="elbowPos"></param>
    /// <returns></returns>
    private bool CheckOnceHandMotion(
        TrackingState trackingState,
        Vector3 handPos,
        Vector3 elbowPos,
        Vector3 handDiff,
        ref bool isValidHandHeight,
        ref bool isSlashStarted,
        ref float slashTime,
        ref Vector3 slashStartPos)
    {
        Vector3 spineNavelPos = trackingState.TrackingBody.GetConvertJointPositions3D(JointId.SpineNavel);
        Vector3 headPos = trackingState.TrackingBody.GetConvertJointPositions3D(JointId.Head);

        // 手がお腹より上でかつ頭の下にあるか
        isValidHandHeight = handPos.y > spineNavelPos.y && handPos.y < headPos.y;
        // 手が有効な位置にない場合は検知を無効にする
        if (!isValidHandHeight)
        {
            isSlashStarted = false;
            return false;
        }

        bool isHandOutSideElbow = handPos.x > elbowPos.x;
        bool isStartSlash = !isSlashStarted && handDiff.x < -slashDetectHandSpeed;
        // 手が肘より左側にある時、モーション検知を開始する
        if (isHandOutSideElbow && isStartSlash)
        {
            isSlashStarted = true;
            slashStartPos = handPos;
            slashTime = 0;
        }

        if (!isSlashStarted) return false;

        // 手の動きが止まった時に、薙ぎ払いを始めてからの距離と時間から判定を行う
        if (handDiff.x >= -slashDetectHandSpeed)
        {
            isSlashStarted = false;
            float slashDistance = Vector3.Distance(handPos, slashStartPos);

            bool isSlashed = slashDistance > slashDetectDistance && slashTime < slashDetectTime;

            return isSlashed;
        }

        slashTime += trackingState.UpdateDeltaTime;

        return false;
    }
}
