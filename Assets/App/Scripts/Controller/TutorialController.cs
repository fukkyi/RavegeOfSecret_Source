using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private PatternUI patternUI = null;
    [SerializeField]
    private Animator messageWindowAnim = null;
    [SerializeField]
    private Animator cameraAnim = null;
    [SerializeField]
    private Animator leftDoorAnim = null;
    [SerializeField]
    private Animator rightDoorAnim = null;
    [SerializeField]
    private Animator patternPanelAnim = null;
    [SerializeField]
    private VideoPlayer upArrowVideo = null;
    [SerializeField]
    private VideoPlayer rightArrowVideo = null;
    [SerializeField]
    private VideoPlayer leftArrowVideo = null;

    private bool isDisplayingWindow = false;

    // Start is called before the first frame update
    void Start()
    {
        // プレイ時間の計測を開始する
        ScoreManager.Instance.StartMeasurePlayTime();
        AudioManager.Instance.PlayBGMWithFade("Negation_Beat", fadeTime: 2.0f, volume: 0.8f);

        patternUI.SetCorrectAction(OnCorrectPattern);

        StartCoroutine(ShowPatternPanel());
    }

    private IEnumerator ShowPatternPanel()
    {
        yield return new WaitForSeconds(4.0f);

        AudioManager.Instance.PlaySE("Impact Classic_01");

        patternPanelAnim.SetTrigger("Show");
    }

    [ContextMenu("CorrectPattern")]
    private void OnCorrectPattern()
    {
        StartCoroutine(ShowMessageWindow());
    }

    private IEnumerator ShowMessageWindow()
    {
        messageWindowAnim.SetBool("Show", true);

        yield return new WaitForSeconds(3.0f);

        isDisplayingWindow = true;

        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedOnceAction(typeof(UpperSlashHandDetecter), OnDetectHideMassageMotion);

        upArrowVideo.Play();
    }

    [ContextMenu("HideMessage")]
    private void OnDetectHideMassageMotion()
    {
        if (!isDisplayingWindow) return;

        StartCoroutine(HideMessageWindow());
    }

    private IEnumerator HideMessageWindow()
    {
        messageWindowAnim.SetBool("Show", false);
        isDisplayingWindow = false;

        yield return new WaitForSeconds(2.0f);

        cameraAnim.SetTrigger("Turn");

        yield return new WaitForSeconds(4.0f);

        rightArrowVideo.Play();
        leftArrowVideo.Play();

        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedOnceAction(typeof(RightSlashHandDetecter), OnDetectOpenDoorMotion);
    }

    [ContextMenu("OpenDoor")]
    private void OnDetectOpenDoorMotion()
    {
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        AudioManager.Instance.PlaySE("DOOR_CLOSE_9");

        leftDoorAnim.SetBool("Open", true);
        rightDoorAnim.SetBool("Open", true);

        yield return new WaitForSeconds(1.0f);

        cameraAnim.SetTrigger("MoveDoor");

        yield return new WaitForSeconds(3.0f);

        SceneTransitionManager.Instance.TransitionByName("Hacking", TransitionType.FadeInOut);
    }
}
