using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

public class HidableWindow : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;
    private RectTransform rectTransform = null;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// ウィンドウを隠す
    /// </summary>
    /// <param name="direction"></param>
    public void Hide(HideDirection direction)
    {
        animator.SetInteger("Direction", (int)direction);
        animator.SetBool("Hide", true);
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }
}

public enum HideDirection
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}
