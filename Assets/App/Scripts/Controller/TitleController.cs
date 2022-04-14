using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    private readonly static string OpeningMovieAseetAddress = "Assets/App/Movies/Opening.mp4";

    [SerializeField]
    private Animator scanUIAnimation = null;
    [SerializeField]
    private Animator startUIAnimation = null;

    [SerializeField]
    private Slider scanProgressSlider = null;
    [SerializeField]
    private float scanTime = 3.0f;
    [SerializeField]
    private float scanDistance = 0.2f;

    private bool isComplateScan = false;
    private bool isReadyStart = false;
    private float currentScanTime = 0;

    private void Start()
    {
        AudioManager.Instance.PlayBGMWithFade("Disturbing_Factors", volume: 0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isComplateScan)
        {
            UpdateScanProgress();
        }
        
        if (isReadyStart)
        {
            UpdateSlashManage();
        }
    }

    private void UpdateScanProgress()
    {
        TrackingManager trackingManager = TrackingManager.Instance;

        if (currentScanTime < scanTime)
        {
            float? baseDistance = trackingManager.GetDistance2DToBaseByTracker();
            // トラッキングしている人がいて、ベースエリアにいる場合はスキャンを進める
            bool isEnterScanArea = baseDistance != null && baseDistance.Value < scanDistance;
            currentScanTime += isEnterScanArea ? Time.deltaTime : -Time.deltaTime;
        }
        else if (!isComplateScan)
        {
            StartCoroutine(PlayScanComplateAnim());
        }

        currentScanTime = Mathf.Clamp(currentScanTime, 0, scanTime);

        scanUIAnimation.SetBool("Show", currentScanTime > 0);
        scanProgressSlider.value = currentScanTime / scanTime;
    }

    private void UpdateSlashManage()
    {
        if (TrackingManager.Instance.GetDistance2DToBaseByTracker() == null)
        {
            startUIAnimation.SetBool("Show", false);
            currentScanTime = 0;
            isComplateScan = false;
            isReadyStart = false;
        } 
    }

    private IEnumerator PlayScanComplateAnim()
    {
        isComplateScan = true;
        scanUIAnimation.SetBool("Complate", true);

        yield return new WaitForSeconds(3.0f);

        startUIAnimation.SetBool("Show", true);

        yield return new WaitForSeconds(0.5f);

        isReadyStart = true;

        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedOnceAction(typeof(LeftSlashHandDetecter), OnStartGame);
    }

    [ContextMenu("StartGame")]
    private void OnStartGame()
    {
        startUIAnimation.SetBool("Show", false);

        AudioManager.Instance.PlaySE("Sweep_03");
        AudioManager.Instance.StopCurrentBGMWithFade(3.0f);

        SceneTransitionManager.Instance.TransitionWithMovieByName(SceneTransitionManager.TutorialSceneNama, OpeningMovieAseetAddress, fadeOutTime: 3.0f, fadeInTime: 5.0f);
    }
}
