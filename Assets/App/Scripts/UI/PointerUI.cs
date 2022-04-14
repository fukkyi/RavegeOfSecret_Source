using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Microsoft.Azure.Kinect.BodyTracking;
public class PointerUI : MonoBehaviour
{
    [SerializeField]
    private Image rightPointerImage = null;
    [SerializeField]
    private Image leftPointerImage = null;
    #region 変更点
    [SerializeField]
    private Image RightSightLines = null;
    [SerializeField]
    private Image LeftSightLines = null;
    [SerializeField]
    private Sprite[] sprite=new Sprite[2];
    #endregion
    private RectTransform parentRectTrans = null;
    private RectTransform rightPointerRectTrans = null;
    private RectTransform leftPointerRectTrans = null;

    private void Start()
    {
        parentRectTrans = GetComponent<RectTransform>();
        rightPointerRectTrans = rightPointerImage.rectTransform;
        leftPointerRectTrans = leftPointerImage.rectTransform;
    }

    public void UpdateDebugPointer(TrackingState trackingState)
    {
        if (trackingState == null) return;
        #region 変更点2
        var currentScene = SceneManager.GetActiveScene();
        if(currentScene.name=="Battle")
        {
            rightPointerImage.sprite = sprite[1];
            leftPointerImage.sprite = sprite[1];
            //RightSightLines.enabled = trackingState.IsRightAimOnScreen;
            //LeftSightLines.enabled = trackingState.IsLeftAimOnscreen;
        }
        else
        {
            rightPointerImage.sprite = sprite[0];
            leftPointerImage.sprite = sprite[0];
        }
        #endregion
        rightPointerImage.enabled = trackingState.IsRightAimOnScreen;
        leftPointerImage.enabled = trackingState.IsLeftAimOnscreen;

        rightPointerRectTrans.anchoredPosition = trackingState.RightAimCoord * parentRectTrans.sizeDelta;
        leftPointerRectTrans.anchoredPosition = trackingState.LeftAimCoord * parentRectTrans.sizeDelta;

        Vector3 rightEuler = trackingState.TrackingBody.GetConvertJointRotation(JointId.HandRight).eulerAngles;
        Vector3 leftEuler = trackingState.TrackingBody.GetConvertJointRotation(JointId.HandLeft).eulerAngles;

        rightPointerRectTrans.rotation = Quaternion.Euler(Vector3.forward * rightEuler.x);
        leftPointerRectTrans.rotation = Quaternion.Euler(Vector3.forward * leftEuler.x);
    }
}
