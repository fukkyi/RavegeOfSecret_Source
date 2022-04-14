using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;

public class JointStatusItem : MonoBehaviour
{
    [SerializeField]
    private Text jointNameText = null;
    [SerializeField]
    private Text jointPosXText = null;
    [SerializeField]
    private Text jointPosYText = null;
    [SerializeField]
    private Text jointPosZText = null;
    [SerializeField]
    private Text jointRotXText = null;
    [SerializeField]
    private Text jointRotYText = null;
    [SerializeField]
    private Text jointRotZText = null;
    [SerializeField]
    private Text jointPreText = null;

    private JointId joint = JointId.Count;

    public void SetJoint(JointId jointId)
    {
        jointNameText.text = jointId.ToString();
        joint = jointId;
    }

    public void UpdateJointStatusText()
    {
        if (joint == JointId.Count) return;
        if (TrackingManager.Instance.MainTrackingState == null) return;

        Body trackingBody = TrackingManager.Instance.MainTrackingState.TrackingBody;

        Vector3 jointPos = trackingBody.GetConvertJointPositions3D(joint);
        Vector3 jointRot = trackingBody.GetConvertJointRotation(joint).eulerAngles;
        JointConfidenceLevel jointPre = trackingBody.JointPrecisions[(int)joint];

        jointPosXText.text = jointPos.x.ToString("F3");
        jointPosYText.text = jointPos.y.ToString("F3");
        jointPosZText.text = jointPos.z.ToString("F3");
        jointRotXText.text = jointRot.x.ToString("F3");
        jointRotYText.text = jointRot.y.ToString("F3");
        jointRotZText.text = jointRot.z.ToString("F3");
        jointPreText.text = jointPre.ToString();
    }
}
