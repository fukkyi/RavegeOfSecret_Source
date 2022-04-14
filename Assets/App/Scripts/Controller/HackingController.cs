using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class HackingController : MonoBehaviour
{
    [SerializeField]
    private Animator cameraAnim = null;
    [SerializeField]
    private Animator puzzleAnim = null;
    [SerializeField]
    private Animator leftDoorAnim = null;
    [SerializeField]
    private Animator rightDoorAnim = null;
    [SerializeField]
    private Animator hackButtonAnim = null;
    [SerializeField]
    private Animator diveImageAnim = null;
    [SerializeField]
    private Animator patternWindowAnim = null;
    [SerializeField]
    private Animator barrierAnim = null;
    [SerializeField]
    private Animator lockImageAnim = null;
    [SerializeField]
    private Animator alertAnim = null;
    [SerializeField]
    private VideoPlayer barrierVideo = null;
    [SerializeField]
    private VideoPlayer[] arrowVideos = null;
    [SerializeField]
    private HidableWindow[] hidableWindowObjs = null;
    [SerializeField]
    private Transform windowParent = null;
    [SerializeField]
    private PatternUI patternWindow = null;
    [SerializeField]
    private PuzzleHandler puzzleHandler = null;
    [SerializeField]
    private int windowCount = 100;
    [SerializeField]
    private List<int> showPatternWindowCountList = new List<int>();
    [SerializeField]
    private CorrentPattern[] correntPatternLists = null;

    private HidableWindow[] hideableWindows = null;

    private bool isReadyHacking = false;
    private bool isDisplayingPatternWindow = false;
    private int currentWindowCount = 0;

    [System.Serializable]
    private class CorrentPattern
    {
        public List<PatternPoint> correctPatternList = new List<PatternPoint>();
    }

    // Start is called before the first frame update
    void Start()
    {
        patternWindow.IsEnablePoint = false;
        patternWindow.SetCorrectAction(OnCorrectPatternWindow);

        StartCoroutine(PlayThroughPuzzleWalkToConsoleAnim());
        GenerateWindow();
    }

    [ContextMenu("Hack")]
    public void OnPushHackButton()
    {
        if (!isReadyHacking) return;

        StartCoroutine(PlayDiveToConsoleAnim());
    }

    private void GenerateWindow()
    {
        hideableWindows = new HidableWindow[windowCount];
        for(int i = 0; i < windowCount; i++)
        {
            float windowXPos = Random.Range(-5.0f, 5.0f) + windowParent.position.x;
            float windowYPos = Random.Range(-1.8f, 1.2f) + windowParent.position.y;
            Vector3 windowPos = Vector3.right * windowXPos + Vector3.up * windowYPos + Vector3.forward * windowParent.position.z;

            // 生成するウィンドウはいくつかのパターンからランダム
            int generateWindowType = Random.Range(0, hidableWindowObjs.Length);
            HidableWindow generateWindowObj = hidableWindowObjs[generateWindowType];

            HidableWindow generateWindow = Instantiate(generateWindowObj, windowPos, Quaternion.identity, windowParent);
            // 新しいウィンドウが手前に表示されるようにヒエラルキーの一番上に移動させる
            generateWindow.transform.SetSiblingIndex(0);
            hideableWindows[i] = generateWindow;
        }
    }

    private void SetHideMotion()
    {
        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(UpperSlashHandDetecter), HideNextWindowToUpper);
        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(DownerSlashHandDetecter), HideNextWindowToDonwer);
        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(RightSlashHandDetecter), HideNextWindowToRight);
        TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(LeftSlashHandDetecter), HideNextWindowToLeft);
    }

    private void RemoveHideMotion()
    {
        TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(UpperSlashHandDetecter), HideNextWindowToUpper);
        TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(DownerSlashHandDetecter), HideNextWindowToDonwer);
        TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(RightSlashHandDetecter), HideNextWindowToRight);
        TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(LeftSlashHandDetecter), HideNextWindowToLeft);
    }

    private void HideNextWindow(HideDirection direction)
    {
        // 妨害ウィンドウ表示中はウィンドウをどかせない
        if (isDisplayingPatternWindow) return;
        if (currentWindowCount >= hideableWindows.Length) return;

        hideableWindows[currentWindowCount].Hide(direction);
        currentWindowCount++;

        AudioManager.Instance.PlaySE("Bullet Flyby 10");

        // 妨害ウィンドウを表示するカウントだったら妨害ウィンドウを表示する
        if (showPatternWindowCountList.Contains(currentWindowCount))
        {
            ChangePatternWindow();
        }

        if (currentWindowCount == hideableWindows.Length)
        {
            CompleteHideWindow();
        }
    }

    [ContextMenu("CompleteHideWindow")]
    private void CompleteHideWindow()
    {
        RemoveHideMotion();
        StartCoroutine(PlayFinishHackingAnim());
    }

    [ContextMenu("HideWindowToUpper")]
    private void HideNextWindowToUpper()
    {
        HideNextWindow(HideDirection.Up);
    }

    [ContextMenu("HideWindowToDonwer")]
    private void HideNextWindowToDonwer()
    {
        HideNextWindow(HideDirection.Down);
    }

    [ContextMenu("HideWindowToRight")]
    private void HideNextWindowToRight()
    {
        HideNextWindow(HideDirection.Right);
    }

    [ContextMenu("HideWindowTooLeft")]
    private void HideNextWindowToLeft()
    {
        HideNextWindow(HideDirection.Left);
    }

    private void ChangePatternWindow()
    {
        if (correntPatternLists.Length <= 0) return;

        int patternIndex = Random.Range(0, correntPatternLists.Length);
        List<PatternPoint> correntPatternList = correntPatternLists[patternIndex].correctPatternList;

        patternWindow.SetCorrectPattern(correntPatternList);
        StartCoroutine(PlayShowPatternWindowAnim());
    }

    [ContextMenu("CorrectPattern")]
    private void OnCorrectPatternWindow()
    {
        StartCoroutine(PlayHidePatternWindowAnim());
    }

    private IEnumerator PlayPuzzleAnim()
    {
        yield return StartCoroutine(PlayWalkToDoor());

        yield return new WaitForSeconds(5.0f);

        puzzleHandler.isReadyPuzzle = true;
        puzzleHandler.EnableMotion();
    }

    private IEnumerator PlayThroughPuzzleWalkToConsoleAnim()
    {
        cameraAnim.SetTrigger("WalkToDoor");

        yield return new WaitForSeconds(3.0f);

        AudioManager.Instance.PlaySE("DOOR_CLOSE_9");

        leftDoorAnim.SetBool("Open", true);
        rightDoorAnim.SetBool("Open", true);

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(PlayWalkToConsoleAnim());
    }

    [ContextMenu("ClearPuzzle")]
    public void OnClearPuzzle()
    {
        StartCoroutine(PlayClearPuzzleAnim());
    }

    public void OnPuzzleTimeUp()
    {
        StartCoroutine(PlayTimeUpPuzzleAnim());
    }

    private IEnumerator PlayWalkToDoor()
    {
        cameraAnim.SetTrigger("WalkToDoor");

        yield return new WaitForSeconds(3.0f);

        puzzleAnim.SetTrigger("Show");
    }

    private IEnumerator PlayClearPuzzleAnim()
    {
        puzzleHandler.DisableMotion();

        lockImageAnim.SetBool("UnLock", true);

        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlaySE("Debuff 2");

        yield return new WaitForSeconds(0.5f);

        puzzleAnim.SetTrigger("Hide");

        yield return new WaitForSeconds(1.0f);

        AudioManager.Instance.PlaySE("DOOR_CLOSE_9");

        leftDoorAnim.SetBool("Open", true);
        rightDoorAnim.SetBool("Open", true);

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(PlayWalkToConsoleAnim());
    }

    private IEnumerator PlayTimeUpPuzzleAnim()
    {
        puzzleHandler.DisableMotion();
        puzzleAnim.SetTrigger("TimeUp");

        yield return new WaitForSeconds(1.5f);

        AudioManager.Instance.PlaySE("DOOR_CLOSE_9");

        leftDoorAnim.SetBool("Open", true);
        rightDoorAnim.SetBool("Open", true);

        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(PlayWalkToConsoleAnim());
    }

    private IEnumerator PlayWalkToConsoleAnim()
    {
        cameraAnim.SetTrigger("WalkToConsole");

        yield return new WaitForSeconds(3.0f);

        leftDoorAnim.SetBool("Open", false);
        rightDoorAnim.SetBool("Open", false);

        yield return new WaitForSeconds(4.5f);

        hackButtonAnim.SetTrigger("Show");

        yield return new WaitForSeconds(1.0f);

        isReadyHacking = true;
    }

    private IEnumerator PlayDiveToConsoleAnim()
    {
        isReadyHacking = false;

        AudioManager.Instance.PlaySE("Light Drone Sound (button hover) 2");

        hackButtonAnim.SetTrigger("Push");

        AudioManager.Instance.StopCurrentBGMWithFade(4.0f);

        yield return new WaitForSeconds(0.5f);

        cameraAnim.SetTrigger("Dive");
        diveImageAnim.SetTrigger("Dive");

        AudioManager.Instance.PlaySE("Mysterious atmosphere 4", lifeTime: 5.0f);

        yield return new WaitForSeconds(6.0f);

        AudioManager.Instance.PlayBGMWithFade("Curse_Screw", fadeTime: 1.0f, volume: 0.7f);

        foreach(VideoPlayer videoPlayer in arrowVideos)
        {
            videoPlayer.Play();
        }

        SetHideMotion();
    }

    private IEnumerator PlayShowPatternWindowAnim()
    {
        isDisplayingPatternWindow = true;

        patternWindowAnim.SetBool("Show", true);
        barrierAnim.SetBool("Show", true);

        yield return new WaitForSeconds(0.5f);

        patternWindow.IsEnablePoint = true;
    }

    private IEnumerator PlayHidePatternWindowAnim()
    {
        patternWindowAnim.SetBool("Show", false);
        barrierAnim.SetBool("Show", false);
        barrierVideo.Play();

        yield return new WaitForSeconds(0.5f);

        isDisplayingPatternWindow = false;
    }

    private IEnumerator PlayFinishHackingAnim()
    {
        // ScoreManager.Instance.StopMeasurePlayTime();
        AudioManager.Instance.StopCurrentBGMWithFade(6.0f);

        yield return new WaitForSeconds(1.0f);

        diveImageAnim.SetTrigger("Dive");

        yield return new WaitForSeconds(4.0f);

        cameraAnim.SetTrigger("ExitConsole");

        yield return new WaitForSeconds(2.0f);

        AudioManager.Instance.PlayBGMWithFade("Blue_Garnet", fadeTime: 1.0f, volume: 0.7f);
        AudioManager.Instance.PlaySE("WarningSiren");
        alertAnim.SetBool("Blink", true);

        yield return new WaitForSeconds(3.0f);

        cameraAnim.SetTrigger("Escape");

        yield return new WaitForSeconds(1.5f);

        leftDoorAnim.SetBool("Open", true);
        rightDoorAnim.SetBool("Open", true);

        yield return new WaitForSeconds(1.5f);

        SceneTransitionManager.Instance.TransitionByName("Battle", TransitionType.FadeInOut);
    }
}
