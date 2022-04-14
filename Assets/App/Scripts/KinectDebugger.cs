using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;

public class KinectDebugger : MonoBehaviour
{
    const int TRACKER_ID = 0;

    public TrackerHandler tracker;
    private SkeletalTrackingProvider trackingProvider;
    public BackgroundData lastFrameData = new BackgroundData();

    [SerializeField]
    private RectTransform parentCanvas = null;
    [SerializeField]
    private Text debugTextNumBody = null;
    [SerializeField]
    private Text leftHandGrippingText = null;
    [SerializeField]
    private Text rightHandGrippingText = null;
    [SerializeField]
    private Text handDistanceText = null;
    [SerializeField]
    private Text leftAimCoordText = null;
    [SerializeField]
    private Text rightAimCoordText = null;
    [SerializeField]
    private Text leftWirstPosText = null;
    [SerializeField]
    private Text rightWirstPosText = null;
    [SerializeField]
    private Text leftWirstRotText = null;
    [SerializeField]
    private Text rightWirstRotText = null;
    [SerializeField]
    private Image leftAimPointer = null;
    [SerializeField]
    private Image rightAimPointer = null;
    [SerializeField]
    private RawImage depthImage = null;

    [SerializeField]
    private Vector3 rightAimOriginOffset = Vector3.zero;
    [SerializeField]
    private Vector3 leftAimOriginOffset = Vector3.zero;

    [SerializeField]
    private float handGripDistance = 0.2f;

    private Camera mainCamera = null;
    private GameObject rightGrappingObject = null;
    private GameObject leftGrappingObject = null;
    private Texture2D depthImageTexture = null;
    private Ray rightAimRay = new Ray();
    private Ray leftAimRay = new Ray();
    private Vector2 rightAimCoord = Vector2.zero;
    private Vector2 leftAimCoord = Vector2.zero;
    private bool isRightGripping = false;
    private bool isLeftGripping = false;
    private bool isRightAimOnScreen = false;
    private bool isLeftAimOnscreen = false;

    void Start()
    {
        //tracker ids needed for when there are two trackers
        trackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!trackingProvider.IsRunning) return;
        if (!trackingProvider.GetCurrentFrameData(ref lastFrameData)) return;
        if (lastFrameData.NumOfBodies == 0) return;

        tracker.updateTracker(lastFrameData);
        UpdateDebug();
    }

    void OnApplicationQuit()
    {
        if (trackingProvider != null)
        {
            trackingProvider.Dispose();
        }
    }

    private void OnDrawGizmos()
    {
        if (trackingProvider == null) return;
        if (!trackingProvider.IsRunning) return;
        if (lastFrameData.NumOfBodies == 0) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rightAimRay);
        Gizmos.DrawSphere(rightAimRay.origin, 0.05f);
        Gizmos.DrawRay(leftAimRay);
        Gizmos.DrawSphere(leftAimRay.origin, 0.05f);
    }

    private void UpdateDebug()
    {
        debugTextNumBody.text = "BodyCount: " + lastFrameData.NumOfBodies.ToString();
        UpdatePointer();
        UpdateGripping();
        UpdateGrappingObject();
        UpdateDepthImage();
    }

    private void UpdateGripping()
    {
        // 左親指の座標
        // Vector3 leftThumbPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.ThumbLeft]);
        // 左手先の座標
        // Vector3 leftHandTipPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.HandTipLeft]);
        // 左手首の座標
        Vector3 leftWirstPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.WristLeft]);
        // 右親指の座標
        // Vector3 rightThumbPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.ThumbRight]);
        // 右手先の座標
        // Vector3 rightHandTipPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.HandTipRight]);
        // 右手首の座標
        Vector3 rightWristPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.WristRight]);

        float handDistance = Vector3.Distance(leftWirstPos, rightWristPos);
        // float leftDistance = Vector3.Distance(leftThumbPos, leftHandTipPos);
        // float rightDistance = Vector3.Distance(rightThumbPos, rightHandTipPos);

        // bool leftGripping = leftDistance <= handGripDistance;
        // bool rightGripping = rightDistance <= handGripDistance;
        bool leftGripping = handDistance <= handGripDistance && leftWirstPos.z < rightWristPos.z;
        bool rightGripping = handDistance <= handGripDistance && rightWristPos.z < leftWirstPos.z;

        // つかむ処理
        if (isLeftGripping != leftGripping)
        {
            if (leftGripping)
            {
                Vector3 leftAimViewPos = leftAimCoord;
                leftAimViewPos.z = 5.0f;
                Ray leftGrappingAimRay = mainCamera.ViewportPointToRay(leftAimViewPos);

                if (Physics.Raycast(leftGrappingAimRay, out RaycastHit raycastHit, 100, 1 << 7))
                {
                    leftGrappingObject = raycastHit.collider.gameObject;
                }
            }
            else if (leftGrappingObject != null)
            {
                leftGrappingObject = null;
            }
        }
        if (isRightGripping != rightGripping)
        {
            if (rightGripping)
            {
                Vector3 rightAimViewPos = rightAimCoord;
                rightAimViewPos.z = 5.0f;
                Ray rightGrappingAimRay = mainCamera.ViewportPointToRay(rightAimViewPos);

                if (Physics.Raycast(rightGrappingAimRay, out RaycastHit raycastHit, 100, 1 << 7))
                {
                    rightGrappingObject = raycastHit.collider.gameObject;
                }
            }
            else if(rightGrappingObject != null)
            {
                rightGrappingObject = null;
            }
        }

        isLeftGripping = leftGripping;
        isRightGripping = rightGripping;

        leftHandGrippingText.text = "LeftGripping: " + (leftGripping ? "Grip" : "UnGrip");
        rightHandGrippingText.text = "RightGripping: " + (rightGripping ? "Grip" : "UnGrip");

        handDistanceText.text = "HandDistance: " + handDistance.ToString("F3");

        leftWirstPosText.text = "LeftWirstPos: X:" + leftWirstPos.x.ToString("F3") + " Y:" + leftWirstPos.y.ToString("F3") + " Z:" + leftWirstPos.z.ToString("F3");
        rightWirstPosText.text = "RightWirstPos: X:" + rightWristPos.x.ToString("F3") + " Y:"+ rightWristPos.y.ToString("F3") + " Z:" + rightWristPos.z.ToString("F3");

        Vector3 leftWirstDir = ConvertToUnityQuaternion(lastFrameData.Bodies[TRACKER_ID].JointRotations[(int)JointId.ElbowLeft]) * Vector3.right;
        Vector3 RightWirstDir = ConvertToUnityQuaternion(lastFrameData.Bodies[TRACKER_ID].JointRotations[(int)JointId.ElbowRight]) * Vector3.right;

        leftWirstRotText.text = "LeftWirstRot: " + ConvertToUnityQuaternion(lastFrameData.Bodies[TRACKER_ID].JointRotations[(int)JointId.ElbowLeft]);
        rightWirstRotText.text = "RightWirstRot: " + ConvertToUnityQuaternion(lastFrameData.Bodies[TRACKER_ID].JointRotations[(int)JointId.ElbowRight]);
    }

    /// <summary>
    /// SystemのVector3をUnityのVector3に変換する
    /// </summary>
    /// <param name="sysVec"></param>
    /// <returns></returns>
    private Vector3 ConvertToUnityVector3(System.Numerics.Vector3 sysVec)
    {
        // KinectのY軸とUnityのY軸が反対のためY軸のみ反転させる
        return Vector3.right * sysVec.X + Vector3.up * -sysVec.Y + Vector3.forward * sysVec.Z;
    }

    private Quaternion ConvertToUnityQuaternion(System.Numerics.Quaternion sysQtn)
    {
        Quaternion convertedQuaternion = Quaternion.identity;
        convertedQuaternion.x = sysQtn.X;
        convertedQuaternion.y = sysQtn.Y;
        convertedQuaternion.z = sysQtn.Z;
        convertedQuaternion.w = sysQtn.W;

        return convertedQuaternion;
    }

    public void UpdatePointer()
    {
        Vector3 rightShoulderPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.ShoulderRight]);
        Vector3 rightWristPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.WristRight]);
        Vector3 leftShoulderPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.ShoulderLeft]);
        Vector3 leftWristPos = ConvertToUnityVector3(lastFrameData.Bodies[TRACKER_ID].JointPositions3D[(int)JointId.WristLeft]);

        Vector3 rightHandDirection = rightWristPos - (rightShoulderPos + rightAimOriginOffset);
        Vector3 leftHandDirection = leftWristPos - (leftShoulderPos + leftAimOriginOffset);

        Vector3 rightWorldHandPos = tracker.transform.position + rightWristPos;
        Vector3 leftWorldHandPos = tracker.transform.position + leftWristPos;

        rightAimRay.origin = rightWorldHandPos;
        rightAimRay.direction = rightHandDirection;
        leftAimRay.origin = leftWorldHandPos;
        leftAimRay.direction = leftHandDirection;

        isRightAimOnScreen = Physics.Raycast(rightAimRay, out RaycastHit rightRaycastHit, 100, 1 << 6);
        isLeftAimOnscreen = Physics.Raycast(leftAimRay, out RaycastHit leftRaycastHit, 100, 1 << 6);

        rightAimPointer.enabled = isRightAimOnScreen;
        leftAimPointer.enabled = isLeftAimOnscreen;

        if (isRightAimOnScreen)
        {
            rightAimCoord =  rightRaycastHit.textureCoord;
            rightAimCoordText.text = "RightAimCoord: " + rightAimCoord;
            rightAimPointer.rectTransform.anchoredPosition = rightAimCoord * parentCanvas.sizeDelta;
        }
        if (isLeftAimOnscreen)
        {
            leftAimCoord = leftRaycastHit.textureCoord;
            leftAimCoordText.text = "LeftAimCoord: " + leftAimCoord;
            leftAimPointer.rectTransform.anchoredPosition = leftAimCoord * parentCanvas.sizeDelta;
        }
    }

    private void UpdateGrappingObject()
    {
        if (rightGrappingObject != null)
        {
            Vector3 rightGrappingPos = rightAimCoord;
            rightGrappingPos.z = 5.0f;
            rightGrappingObject.transform.position = mainCamera.ViewportToWorldPoint(rightGrappingPos);
        }
        if (leftGrappingObject != null)
        {
            Vector3 leftGrappingPos = leftAimCoord;
            leftGrappingPos.z = 5.0f;
            leftGrappingObject.transform.position = mainCamera.ViewportToWorldPoint(leftGrappingPos);
        }
    }

    private void UpdateDepthImage()
    {
        if (!TrackingManager.Instance.IsLatestUpdate) return;

        if (depthImageTexture == null)
        {
            depthImageTexture = new Texture2D(lastFrameData.DepthImageWidth, lastFrameData.DepthImageHeight, TextureFormat.RGB24, false);
        }

        depthImageTexture.LoadRawTextureData(lastFrameData.DepthImage);
        depthImageTexture.Apply();

        depthImage.texture = depthImageTexture;
    }
}
