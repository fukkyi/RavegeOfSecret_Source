using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternPoint : MonoBehaviour, IPointable
{
    public static readonly string AnimParamNameHighlight = "Highlight";
    public static readonly string AnimParamNameError = "Error";
    public static readonly string AnimParamNameCorrect = "Correct";

    public RectTransform RectTransform { get; private set; }
    public Animator Animator { get; private set; }

    private PatternUI patternUI = null;
    private Image pointImage = null;

    private void Awake()
    {
        patternUI = GetComponentInParent<PatternUI>();
        RectTransform = GetComponent<RectTransform>();
        pointImage = GetComponent<Image>();
        Animator = GetComponent<Animator>();
    }

    public void OnGraped(PointingState state)
    {

    }

    public void OnPointed(PointingState state)
    {
        patternUI.SetPoint(this, state.pointingHand);
    }

    public void OnUnGraped(PointingState state)
    {

    }

    public void OnUnPointed(PointingState state)
    {

    }

    public void UpdateGraping(PointingState state)
    {

    }

    public void UpdatePointing(PointingState state)
    {

    }
}
