using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectState : MonoBehaviour
{
    public BackgroundData LastFrameData { get { return lastFrameData; } }
    public bool IsLatestUpdate { get; private set; } = false;
    public bool IsRunning { get { return trackingProvider.IsRunning; } }

    private MultipleTrackingProvider trackingProvider = null;
    private BackgroundData lastFrameData = null;

    public KinectState(int index)
    {
        trackingProvider = new MultipleTrackingProvider(index);
        lastFrameData = new BackgroundData();
    }

    /// <summary>
    /// Kinect‚Ìó‘Ô‚ğXV‚·‚é
    /// </summary>
    public void UpdateState()
    {
        if (!trackingProvider.IsRunning) return;

        IsLatestUpdate = trackingProvider.GetCurrentFrameData(ref lastFrameData);
    }

    /// <summary>
    /// Kinect‚ğ”jŠü‚·‚é
    /// </summary>
    public void Dispose()
    {
        if (trackingProvider == null) return;

        trackingProvider.Dispose();
    }
}
