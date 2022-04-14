using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.Azure.Kinect.BodyTracking;
using System;

public class TrackingState : MonoBehaviour
{
    [NonSerialized]
    public TrackerHandler tracker;

    [SerializeField]
    private MotionUpdater motionUpdater = null;
    [SerializeField]
    private Vector3 rightAimOriginOffset = Vector3.zero;
    [SerializeField]
    private Vector3 leftAimOriginOffset = Vector3.zero;
    [SerializeField]
    private float handGripDistance = 0.2f;
    [SerializeField]
    private float aimSpeed = 20.0f;

    public MotionUpdater MotionUpdater { get { return motionUpdater; } }
    public PointingState RightPointingState { get { return pointingStateR; } }
    public PointingState LeftPointingState { get { return pointingStateL; } }
    public IPointable RightPointingObject { get { return pointingObjR; } }
    public IPointable LeftPointingObject { get { return pointingObjL; } }
    public Ray RightAimRay { get { return aimRayR; } }
    public Ray LeftAimRay { get { return aimRayL; } }
    public Body TrackingBody { get { return trackingBody; } }
    public Body LastFlameDiffBody { get { return lastFlameDiffBody; } }
    public Vector2 RightAimCoord { get; private set; }  = Vector2.zero;
    public Vector2 LeftAimCoord { get; private set; } = Vector2.zero;
    public Quaternion RightWristRot { get; private set; } = Quaternion.identity;
    public Quaternion LeftWristRot { get; private set; } = Quaternion.identity;

    public bool IsRightGriping { get { return isGripingR; } }
    public bool IsLeftGriping { get { return isGripingL; } }
    public bool IsRightAimOnScreen { get; private set; } = false;
    public bool IsLeftAimOnscreen { get; private set; } = false;
    public bool IsMainTraker { get { return TrackingManager.Instance.MainTrackingState == this; } }
    public int BodyId { get; set; } = 0;
    public float HandDistance { get; private set; } = 0;
    public float UpdateDeltaTime { get; private set; } = 0;

    private PointingState pointingStateR = new PointingState(PointingHand.Right);
    private PointingState pointingStateL = new PointingState(PointingHand.Left);

    private Ray aimRayR = new Ray();
    private Ray aimRayL = new Ray();
    private Body beforeTrackingBody = new Body();
    private Body trackingBody = new Body();
    private Body lastFlameDiffBody = new Body();
    private Vector2 targetAimCoordR = Vector2.zero;
    private Vector2 targetAimCoordL = Vector2.zero;
    private IPointable pointingObjR = null;
    private IPointable pointingObjL = null;
    private IPointable gripingObjR = null;
    private IPointable gripingObjL = null;

    private bool isGriping = false;
    private bool isGripingR = false;
    private bool isGripingL = false;
    private float lastFlameTime = 0;

    private void Awake()
    {
        tracker = GetComponent<TrackerHandler>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(aimRayR);
        Gizmos.DrawSphere(aimRayR.origin, 0.05f);
        Gizmos.DrawRay(aimRayL);
        Gizmos.DrawSphere(aimRayL.origin, 0.05f);
        Gizmos.DrawRay(pointingStateR.pointRay);
        Gizmos.DrawRay(pointingStateL.pointRay);
    }

    public void UpdateState(BackgroundData lastFlameData)
    {
        if (TrackingManager.Instance.IsLatestUpdate)
        {
            tracker.updateTracker(lastFlameData, BodyId);

            beforeTrackingBody = Body.DeepCopy(trackingBody);
            trackingBody = lastFlameData.Bodies[lastFlameData.FindBodyIndexFromId(BodyId)];

            UpdateTrackingBodyDiff();
            motionUpdater.UpdateMotionDetectersByTracker(this);
        }

        UpdatePointer();
        UpdateGripping();
    }

    /// <summary>
    /// 掴む判定を更新する
    /// </summary>
    private void UpdateGripping()
    {
        if (!TrackingManager.Instance.IsLatestUpdate) return;

        Vector3 leftWirstPos = trackingBody.GetConvertJointPositions3D(JointId.WristLeft);
        Vector3 rightWristPos = trackingBody.GetConvertJointPositions3D(JointId.WristRight);

        float handDistance = Vector3.Distance(leftWirstPos, rightWristPos);
        bool handGriping = handDistance <= handGripDistance;

        bool leftGriping = isGripingL;
        bool rightGriping = isGripingR;
        if (handGriping != isGriping)
        {
            leftGriping = handDistance <= handGripDistance && leftWirstPos.z < rightWristPos.z;
            rightGriping = handDistance <= handGripDistance && rightWristPos.z < leftWirstPos.z;
        }

        // つかむ処理
        CallGripingManage(ref gripingObjL, ref isGripingL, leftGriping, pointingStateL, pointingObjL);
        CallGripingManage(ref gripingObjR, ref isGripingR, rightGriping, pointingStateR, pointingObjR);

        isGriping = handGriping;
        HandDistance = handDistance;
    }

    /// <summary>
    /// 手のポインターを更新する
    /// </summary>
    public void UpdatePointer()
    {
        if (TrackingManager.Instance.IsLatestUpdate)
        {
            Vector3 rightShoulderPos = trackingBody.GetConvertJointPositions3D(JointId.ShoulderRight);
            Vector3 rightWristPos = trackingBody.GetConvertJointPositions3D(JointId.WristRight);
            Vector3 leftShoulderPos = trackingBody.GetConvertJointPositions3D(JointId.ShoulderLeft);
            Vector3 leftWristPos = trackingBody.GetConvertJointPositions3D(JointId.WristLeft);

            Vector3 rightHandDirection = rightWristPos - (rightShoulderPos + rightAimOriginOffset);
            Vector3 leftHandDirection = leftWristPos - (leftShoulderPos + leftAimOriginOffset);

            Vector3 rightWorldHandPos = tracker.transform.position + rightWristPos;
            Vector3 leftWorldHandPos = tracker.transform.position + leftWristPos;

            aimRayR.origin = rightWorldHandPos;
            aimRayR.direction = rightHandDirection;
            aimRayL.origin = leftWorldHandPos;
            aimRayL.direction = leftHandDirection;

            IsRightAimOnScreen = Physics.Raycast(aimRayR, out RaycastHit rightRaycastHit, 100, 1 << 6);
            IsLeftAimOnscreen = Physics.Raycast(aimRayL, out RaycastHit leftRaycastHit, 100, 1 << 6);

            if (IsRightAimOnScreen)
            {
                targetAimCoordR = rightRaycastHit.textureCoord;
            }
            if (IsLeftAimOnscreen)
            {
                targetAimCoordL = leftRaycastHit.textureCoord;
            }
        }

        RightAimCoord = Vector2.Lerp(RightAimCoord, targetAimCoordR, aimSpeed * Time.deltaTime);
        LeftAimCoord = Vector2.Lerp(LeftAimCoord, targetAimCoordL, aimSpeed * Time.deltaTime);

        Vector3 rightPointPos = RightAimCoord;
        Vector3 leftPointPos = LeftAimCoord;
        rightPointPos.z = 5.0f;
        leftPointPos.z = 5.0f;

        CallPointableManage(ref pointingObjR, ref pointingStateR, rightPointPos);
        CallPointableManage(ref pointingObjL, ref pointingStateL, leftPointPos);
    }

    /// <summary>
    /// Pointableなオブジェクト掴んだ際の処理を呼ぶ
    /// </summary>
    /// <param name="beforeGriping"></param>
    /// <param name="griping"></param>
    /// <param name="pointingObj"></param>
    private void CallGripingManage(ref IPointable gripingObj, ref bool beforeGriping, bool griping, PointingState pointingState, IPointable pointingObj)
    {
        if (griping)
        {
            if (beforeGriping != griping)
            {
                gripingObj = pointingObj;
                gripingObj?.OnGraped(pointingState);
            }

            gripingObj?.UpdateGraping(pointingState);
        }
        else if (beforeGriping != griping)
        {
            gripingObj?.OnUnGraped(pointingState);
            gripingObj = null;
        }

        beforeGriping = griping;
    }

    /// <summary>
    /// Pointableなオブジェクトをポイントした際の処理を呼ぶ
    /// </summary>
    /// <param name="beforePointingObj"></param>
    /// <param name="pointRay"></param>
    private void CallPointableManage(ref IPointable beforePointingObj, ref PointingState pointingState, Vector3 pointPos)
    {
        Camera pointingCamara = TrackingManager.Instance.PointingCamera;
        IPointable pointableObject = null;

        if (pointingCamara != null)
        {
            Ray pointRay = pointingCamara.ViewportPointToRay(pointPos);

            if (Physics.Raycast(pointRay, out RaycastHit raycastHit, 100, 1 << 7))
            {
                pointableObject = raycastHit.collider.GetComponent<IPointable>();
            }

            pointingState.pointRay = pointRay;
            pointingState.hitData = raycastHit;
            pointingState.pointDistance = pointPos.z;
        }

        if (pointableObject == null)
        {
            beforePointingObj?.OnUnPointed(pointingState);
        }
        else if (pointableObject != beforePointingObj)
        {
            pointableObject.OnPointed(pointingState);
        }
        else
        {
            pointableObject.UpdatePointing(pointingState);
        }

        beforePointingObj = pointableObject;
    }

    /// <summary>
    /// トラッキングした情報の差分を更新する
    /// </summary>
    private void UpdateTrackingBodyDiff()
    {
        lastFlameDiffBody = Body.DeepCopy(trackingBody);
        UpdateDeltaTime = Time.time - lastFlameTime;
        lastFlameTime = Time.time;

        if (lastFlameDiffBody.Length == 0) return;
        if (beforeTrackingBody.Length == 0) return;

        for(int i = 0; i < lastFlameDiffBody.Length; i++)
        {
            lastFlameDiffBody.JointPositions2D[i] -= beforeTrackingBody.JointPositions2D[i];
            lastFlameDiffBody.JointPositions3D[i] -= beforeTrackingBody.JointPositions3D[i];
            lastFlameDiffBody.JointRotations[i] -= beforeTrackingBody.JointRotations[i];
        }
    }

    /// <summary>
    /// トラッキング対象がどの座標にいるか取得する
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRootPos()
    {
        return trackingBody.GetConvertJointPositions3D(JointId.Pelvis);
    }

    /// <summary>
    /// 特定の手のポイント座標を取得する
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public Vector2 GetAimCoordByHand(PointingHand hand)
    {
        Vector2 aimCoord = Vector2.zero;
        if (hand == PointingHand.Right)
        {
            aimCoord = RightAimCoord;
        }
        else if (hand == PointingHand.Left)
        {
            aimCoord = LeftAimCoord;
        }

        return aimCoord;
    }

    /// <summary>
    /// 特定の手のスクリーン座標を取得する
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public Vector2 GetScreenPointByHand(PointingHand hand)
    {
        Vector2 aimCoord = GetAimCoordByHand(hand);

        return Vector2.right * (Screen.width * aimCoord.x) + Vector2.up * (Screen.height * aimCoord.y);
    }
}

public class PointingState
{
    public PointingHand pointingHand = PointingHand.None;
    public Ray pointRay = new Ray();
    public RaycastHit hitData = new RaycastHit();
    public float pointDistance = 5.0f;

    public PointingState(PointingHand hand = PointingHand.None)
    {
        pointingHand = hand;
    }
}

public enum PointingHand
{
    None,
    Right,
    Left,
}