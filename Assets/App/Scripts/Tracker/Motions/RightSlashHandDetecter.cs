using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

/// <summary>
/// ������E�Ɏ��ガ�������[�V�����̃f�B�e�N�^�[
/// </summary>
public class RightSlashHandDetecter : MotionDetecter
{
    // �ガ���������m���鎞��
    [SerializeField]
    private float slashDetectTime = 0.5f;
    // �ガ���������m���钷��
    [SerializeField]
    private float slashDetectDistance = 0.25f;
    // �ガ���������m�����̑���
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
        // �E�r�����r�œガ�����Ă���Ό��m�Ƃ݂Ȃ�
        return (detectedRightMotion || detectedLeftMotion);
    }

    /// <summary>
    /// �Иr�̓ガ�������[�V�����̌��m���s��
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

        // �肪��������ł����̉��ɂ��邩
        isValidHandHeight = handPos.y > spineNavelPos.y && handPos.y < headPos.y;
        // �肪�L���Ȉʒu�ɂȂ��ꍇ�͌��m�𖳌��ɂ���
        if (!isValidHandHeight)
        {
            isSlashStarted = false;
            return false;
        }

        bool isHandOutSideElbow = handPos.x > elbowPos.x;
        bool isStartSlash = !isSlashStarted && handDiff.x < -slashDetectHandSpeed;
        // �肪�I��荶���ɂ��鎞�A���[�V�������m���J�n����
        if (isHandOutSideElbow && isStartSlash)
        {
            isSlashStarted = true;
            slashStartPos = handPos;
            slashTime = 0;
        }

        if (!isSlashStarted) return false;

        // ��̓������~�܂������ɁA�ガ�������n�߂Ă���̋����Ǝ��Ԃ��画����s��
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
