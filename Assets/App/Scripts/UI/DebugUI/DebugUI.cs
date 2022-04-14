using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform jointStatusParent = null;
    [SerializeField]
    private JointStatusItem jointStatusItemObj = null;
    [SerializeField]
    private List<JointId> showStatusJointList = new List<JointId>();

    [SerializeField]
    private Text debugTextNumBody = null;
    [SerializeField]
    private Text leftHandGrippingText = null;
    [SerializeField]
    private Text rightHandGrippingText = null;
    [SerializeField]
    private Text leftAimCoordText = null;
    [SerializeField]
    private Text rightAimCoordText = null;
    [SerializeField]
    private Text baseDistanceText = null;
    [SerializeField]
    private Text kinectStatusText = null;
    [SerializeField]
    private Image leftAimPointer = null;
    [SerializeField]
    private Image rightAimPointer = null;
    [SerializeField]
    private RawImage depthImage = null;

    private RectTransform parentCanvas = null;
    private Texture2D depthImageTexture = null;
    private List<JointStatusItem> jointStatusItemList = new List<JointStatusItem>();

    // Start is called before the first frame update
    void Start()
    {
        parentCanvas = GetComponent<RectTransform>();

        InitJointStatus();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (TrackingManager.Instance == null) return;

        TrackingState trackingState = TrackingManager.Instance.MainTrackingState;

        if (!TrackingManager.Instance.IsRunning) return;

        BackgroundData lastFlameData = TrackingManager.Instance.lastFrameData;

        UpdateDebugDepthImage(lastFlameData);
        UpdateDebugText(trackingState, lastFlameData);
        UpdateDebugPointer(trackingState);
        UpdateJointStatus();
    }

    private void UpdateDebugText(TrackingState trackingState, BackgroundData lastFlameData)
    {
        debugTextNumBody.text = "BodyCount: " + lastFlameData.NumOfBodies;

        if (trackingState == null) return;

        leftHandGrippingText.text = "LeftGripping: " + (trackingState.IsLeftGriping ? "Grip" : "UnGrip");
        rightHandGrippingText.text = "RightGripping: " + (trackingState.IsRightGriping ? "Grip" : "UnGrip");

        rightAimCoordText.text = "RightAimCoord: " + trackingState.RightAimCoord;
        leftAimCoordText.text = "LeftAimCoord: " + trackingState.LeftAimCoord;

        baseDistanceText.text = "BaseDistance: " + TrackingManager.Instance.GetDistance2DToBaseByTracker();
    }

    private void UpdateDebugPointer(TrackingState trackingState)
    {
        if (trackingState == null) return;

        rightAimPointer.enabled = trackingState.IsRightAimOnScreen;
        leftAimPointer.enabled = trackingState.IsLeftAimOnscreen;

        rightAimPointer.rectTransform.anchoredPosition = trackingState.RightAimCoord * parentCanvas.sizeDelta;
        leftAimPointer.rectTransform.anchoredPosition = trackingState.LeftAimCoord * parentCanvas.sizeDelta;
    }

    private void UpdateDebugDepthImage(BackgroundData lastFlameData)
    {
        if (!TrackingManager.Instance.IsLatestUpdate) return;

        if (depthImageTexture == null)
        {
            depthImageTexture = new Texture2D(lastFlameData.DepthImageWidth, lastFlameData.DepthImageHeight, TextureFormat.RGB24, false);
        }

        depthImageTexture.LoadRawTextureData(lastFlameData.DepthImage);
        depthImageTexture.Apply();

        depthImage.texture = depthImageTexture;
    }

    private void InitJointStatus()
    {
        showStatusJointList.ForEach((jointId) => {
            JointStatusItem statusItem = Instantiate(jointStatusItemObj, jointStatusParent);
            statusItem.SetJoint(jointId);
            jointStatusItemList.Add(statusItem);
        });
    }

    private void UpdateJointStatus()
    {
        jointStatusItemList.ForEach((jointStatusItem) => { jointStatusItem.UpdateJointStatusText(); });
    }

    public void SetKinectStatus(string status)
    {
        kinectStatusText.text = "KinectStatus: " + status;
    }
}