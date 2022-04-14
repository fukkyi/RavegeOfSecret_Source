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
    /// ���݈����Ă�������X�V����
    /// </summary>
    public void UpdateCurrentLine()
    {
        if (currentLine == null) return;

        Vector2 screenPos = TrackingManager.Instance.MainTrackingState.GetScreenPointByHand(PointingHand);
        Camera poitingCamera = TrackingManager.Instance.PointingCamera;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(pointParent, screenPos, poitingCamera, out Vector2 pointingPos)) return;
        // �p�^�[���E�B���h�E����O�ɏo���ꍇ�͍ŏ�����ɂ���
        if (!IsInsidePointArea(pointingPos))
        {
            Destroy(currentLine.gameObject);
            StartCoroutine(PlayErrorManage());
        }

        currentLine.UpdateLine(pointingPos);
    }

    /// <summary>
    /// �_���C���^���N�g�����ۂ̏���
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
        // �O��̐�������ꍇ�́A�|�C���g�����ꏊ�ɐ������킹��
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
    /// �������̃A�N�V������ǉ�����
    /// </summary>
    /// <param name="onCorrect"></param>
    public void SetCorrectAction(UnityAction onCorrect)
    {
        onCorrectAction = onCorrect;
    }

    /// <summary>
    /// �����p�^�[����ύX����
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
    /// ���Ɍq����_���擾����
    /// </summary>
    /// <returns></returns>
    private PatternPoint GetNextPoint()
    {
        if (connectCount >= correctPatternList.Count || connectCount < FirstConnectIndex) return null;

        return correctPatternList[connectCount];
    }

    /// <summary>
    /// �w�肵�����W���|�C���g����͈͂ɓ����Ă��邩
    /// </summary>
    /// <param name="pointingPos"></param>
    /// <returns></returns>
    private bool IsInsidePointArea(Vector2 pointingPos)
    {
        if (PointingHand == PointingHand.Left)
        {
            // ����Ń|�C���g���Ă���ۂɁA���肪�|�C���^�[����O�ꂽ��A�͈͂ɓ����Ă��Ȃ��Ƃ���
            if (!TrackingManager.Instance.MainTrackingState.IsLeftAimOnscreen) return false;
        }
        else if (PointingHand == PointingHand.Right)
        {
            // �E��Ń|�C���g���Ă���ۂɁA�E�肪�|�C���^�[����O�ꂽ��A�͈͂ɓ����Ă��Ȃ��Ƃ���
            if (!TrackingManager.Instance.MainTrackingState.IsRightAimOnScreen) return false;
        }

        return pointParent.rect.Contains(pointingPos);
    }
    
    /// <summary>
    /// ���Ɍq����_���n�C���C�g������
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
    /// ����Ńn�C���C�g���Ă���_�̃n�C���C�g������
    /// </summary>
    private void SetUnHighlight()
    {
        PatternPoint nextPoint = GetNextPoint();
        if (nextPoint == null) return;

        nextPoint.Animator.SetBool(PatternPoint.AnimParamNameHighlight, false);
    }

    /// <summary>
    /// �S�Ă̓_�̃A�j���[�V�����̃p�����[�^��ݒ肷�� (Bool)
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="value"></param>
    private void SetAnimBoolForPoint(string animName, bool value)
    {
        patternPointList.ForEach((patternPoint) => { patternPoint.Animator.SetBool(animName, value); });
    }

    /// <summary>
    /// �S�Ă̓_�̃A�j���[�V�����̃p�����[�^��ݒ肷�� (Trigger)
    /// </summary>
    /// <param name="animName"></param>
    private void SetAnimTriggerForPoint(string animName)
    {
        patternPointList.ForEach((patternPoint) => { patternPoint.Animator.SetTrigger(animName); });
    }

    /// <summary>
    /// ����S�ď���
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
