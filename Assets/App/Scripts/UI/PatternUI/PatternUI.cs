using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PatternUI : MonoBehaviour
{
    public PointingHand PointingHand { get; private set; } = PointingHand.None;
    public bool IsEnablePoint { get; set; } = true;

    private static readonly int FirstConnectIndex = 0;

    [SerializeField]
    private PatternLine patternLinePrefab = null;
    [SerializeField]
    private RectTransform lineParent = null;
    [SerializeField]
    private RectTransform pointParent = null;
    [SerializeField]
    private List<PatternPoint> correctPatternList = new List<PatternPoint>();

    [SerializeField]
    private float colorAnimTime = 2.0f;

    private PatternLine currentLine = null;
    private PatternPoint currentPoint = null;
    private List<PatternPoint> patternPointList = null;
    private List<PatternLine> connectLineList = new List<PatternLine>();
    private UnityAction onCorrectAction = null;
    private int connectCount = FirstConnectIndex;

    private void Start()
    {
        patternPointList = new List<PatternPoint>(pointParent.GetComponentsInChildren<PatternPoint>());
        SetHighlightForNextPoint();
    }

    private void Update()
    {
        UpdateCurrentLine();
    }

    /// <summary>
    /// 現在引いている線を更新する
    /// </summary>
    public void UpdateCurrentLine()
    {
        if (currentLine == null) return;

        Vector2 screenPos = TrackingManager.Instance.MainTrackingState.GetScreenPointByHand(PointingHand);
        Camera poitingCamera = TrackingManager.Instance.PointingCamera;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(pointParent, screenPos, poitingCamera, out Vector2 pointingPos)) return;
        // パターンウィンドウから外に出た場合は最初からにする
        if (!IsInsidePointArea(pointingPos))
        {
            Destroy(currentLine.gameObject);
            StartCoroutine(PlayErrorManage());
        }

        currentLine.UpdateLine(pointingPos);
    }

    /// <summary>
    /// 点をインタラクトした際の処理
    /// </summary>
    /// <param name="point"></param>
    public void SetPoint(PatternPoint point, PointingHand hand)
    {

        if (!IsEnablePoint) return;
        if (point != GetNextPoint()) return;

        if (connectCount == FirstConnectIndex)
        {
            PointingHand = hand;
        }
        else if (PointingHand != hand) return;

        Vector2 pointPos = point.RectTransform.anchoredPosition;
        // 前回の線がある場合は、ポイントした場所に線を合わせる
        if (currentLine != null)
        {
            currentLine.UpdateLine(pointPos);
            connectLineList.Add(currentLine);
        }

        currentPoint = point;

        connectCount++;
        SetHighlightForNextPoint();

        AudioManager.Instance.PlaySE("Clicks Sound (button hover) 3");

        if (connectCount >= correctPatternList.Count)
        {
            currentLine = null;
            StartCoroutine(PlayCorrectManage());
            return;
        }

        currentLine = Instantiate(patternLinePrefab, lineParent);
        currentLine.SetOriginPos(pointPos);
    }

    /// <summary>
    /// 成功時のアクションを追加する
    /// </summary>
    /// <param name="onCorrect"></param>
    public void SetCorrectAction(UnityAction onCorrect)
    {
        onCorrectAction = onCorrect;
    }

    /// <summary>
    /// 成功パターンを変更する
    /// </summary>
    /// <param name="newCorrentPatternList"></param>
    public void SetCorrectPattern(List<PatternPoint> newCorrentPatternList)
    {
        SetUnHighlight();

        correctPatternList.Clear();
        correctPatternList.AddRange(newCorrentPatternList);

        connectCount = 0;
        currentPoint = null;

        SetHighlightForNextPoint();

        SetAnimBoolForPoint(PatternPoint.AnimParamNameCorrect, false);
        ClearConnectLine();
    }

    /// <summary>
    /// 次に繋げる点を取得する
    /// </summary>
    /// <returns></returns>
    private PatternPoint GetNextPoint()
    {
        if (connectCount >= correctPatternList.Count || connectCount < FirstConnectIndex) return null;

        return correctPatternList[connectCount];
    }

    /// <summary>
    /// 指定した座標がポイントする範囲に入っているか
    /// </summary>
    /// <param name="pointingPos"></param>
    /// <returns></returns>
    private bool IsInsidePointArea(Vector2 pointingPos)
    {
        if (PointingHand == PointingHand.Left)
        {
            // 左手でポイントしている際に、左手がポインターから外れたら、範囲に入っていないとする
            if (!TrackingManager.Instance.MainTrackingState.IsLeftAimOnscreen) return false;
        }
        else if (PointingHand == PointingHand.Right)
        {
            // 右手でポイントしている際に、右手がポインターから外れたら、範囲に入っていないとする
            if (!TrackingManager.Instance.MainTrackingState.IsRightAimOnScreen) return false;
        }

        return pointParent.rect.Contains(pointingPos);
    }
    
    /// <summary>
    /// 次に繋げる点をハイライトさせる
    /// </summary>
    private void SetHighlightForNextPoint()
    {
        if (currentPoint != null)
        {
            currentPoint.Animator.SetBool(PatternPoint.AnimParamNameHighlight, false);
        }

        PatternPoint nextPoint = GetNextPoint();

        if (nextPoint != null)
        {
            nextPoint.Animator.SetBool(PatternPoint.AnimParamNameHighlight, true);
        }
    }

    /// <summary>
    /// 現状でハイライトしている点のハイライトを消す
    /// </summary>
    private void SetUnHighlight()
    {
        PatternPoint nextPoint = GetNextPoint();
        if (nextPoint == null) return;

        nextPoint.Animator.SetBool(PatternPoint.AnimParamNameHighlight, false);
    }

    /// <summary>
    /// 全ての点のアニメーションのパラメータを設定する (Bool)
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="value"></param>
    private void SetAnimBoolForPoint(string animName, bool value)
    {
        patternPointList.ForEach((patternPoint) => { patternPoint.Animator.SetBool(animName, value); });
    }

    /// <summary>
    /// 全ての点のアニメーションのパラメータを設定する (Trigger)
    /// </summary>
    /// <param name="animName"></param>
    private void SetAnimTriggerForPoint(string animName)
    {
        patternPointList.ForEach((patternPoint) => { patternPoint.Animator.SetTrigger(animName); });
    }

    /// <summary>
    /// 線を全て消す
    /// </summary>
    private void ClearConnectLine()
    {
        connectLineList.ForEach((connectLine) => { Destroy(connectLine.gameObject); });
        connectLineList.Clear();
    }

    private IEnumerator PlayCorrectManage()
    {
        IsEnablePoint = false;

        AudioManager.Instance.PlaySE("Locked 1");

        SetAnimBoolForPoint(PatternPoint.AnimParamNameCorrect, true);

        yield return new WaitForSeconds(colorAnimTime);

        onCorrectAction?.Invoke();
    }

    private IEnumerator PlayErrorManage()
    {
        IsEnablePoint = false;

        AudioManager.Instance.PlaySE("Power up 6");

        SetUnHighlight();

        connectCount = 0;
        currentPoint = null;

        SetHighlightForNextPoint();

        SetAnimTriggerForPoint(PatternPoint.AnimParamNameError);

        ClearConnectLine();

        yield return new WaitForSeconds(colorAnimTime);

        IsEnablePoint = true;
    }
}
