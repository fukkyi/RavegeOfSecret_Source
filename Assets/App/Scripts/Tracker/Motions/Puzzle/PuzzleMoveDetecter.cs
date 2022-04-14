
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

public abstract class PuzzleMoveDetecter : MotionDetecter
{
    [SerializeField]
    private float stopedHandSpeed = 0.005f;
    [SerializeField]
    private float moveingHandSpeed = 0.015f;
    [SerializeField]
    private float requiredDistance = 0.15f;
    [SerializeField]
    private float readyShoulderDistance = 0.15f;
    [SerializeField]
    private float maxMovedTime = 0.7f;

    private Vector2 detectStartHandPos = Vector2.zero;
    private float moveingTime = 0;
    private bool isDetectingHandMove = false;
    private bool isReadyStartDetect = false;

    /// <summary>
    /// ���[�V�������m�Ɏg�p���鑤���擾����
    /// </summary>
    /// <returns></returns>
    protected abstract DetectSide GetDetectSide();

    /// <summary>
    /// ��𓮂������������烂�[�V�������������邩���肷��
    /// </summary>
    /// <returns></returns>
    protected abstract bool IsValidDirection(Vector2 direction);

    protected override bool CheckMotion()
    {
        DetectSide detectSide = GetDetectSide();

        Vector3 handPos = SelectJointPosByDetectSide(detectSide, JointId.HandLeft, JointId.HandRight);
        Vector2 handPos2D = Vector2.right * handPos.x + Vector2.up * handPos.y;
        Vector3 handDiff = SelectJointDiffByDetectSide(detectSide, JointId.HandLeft, JointId.HandRight); 
        Vector3 shoulderPos = SelectJointPosByDetectSide(detectSide, JointId.ShoulderLeft, JointId.ShoulderRight);
        Vector2 shoulderPos2D = Vector2.right * shoulderPos.x + Vector2.up * shoulderPos.y;
        float handSpeed = handDiff.magnitude;
        bool isValidReadyPos = Vector2.Distance(handPos2D, shoulderPos2D) <= readyShoulderDistance;

        if (isDetectingHandMove)
        {
            // ���f�����������Ď~�܂�܂ł����m����
            if (handSpeed >= moveingHandSpeed)
            {
                moveingTime += detectTrackingState.UpdateDeltaTime;
            }
            else
            {
                float handMovedDistance = Vector2.Distance(handPos2D, detectStartHandPos);
                bool isDetectMotion = false;
                if (moveingTime <= maxMovedTime && handMovedDistance >= requiredDistance)
                {
                    Vector2 moveDirection = (detectStartHandPos - handPos2D).normalized;
                    isDetectMotion = IsValidDirection(moveDirection);
                }

                isDetectingHandMove = false;
                isReadyStartDetect = false;

                return isDetectMotion;
            }
        }
        else if (isReadyStartDetect)
        {
            // ���f���������������m����
            if (handSpeed >= moveingHandSpeed)
            {
                isDetectingHandMove = true;
                detectStartHandPos = handPos2D;
                moveingTime = 0;
            }
            else if (!isValidReadyPos)
            {
                isReadyStartDetect = false;
            }
        }
        else
        {
            // �肪���܂蓮���Ă��Ȃ��ŁA���ƕI�̊Ԃɂ����Ԃ��猟�m���J�n����
            isReadyStartDetect = handSpeed <= stopedHandSpeed && isValidReadyPos;
        }

        return false;
    }
}
