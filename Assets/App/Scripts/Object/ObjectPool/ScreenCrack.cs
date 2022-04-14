using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCrack : PoolableObject
{
    [SerializeField]
    protected Animation animationPlayer = null;
    [SerializeField]
    protected RectTransform rectTransform = null;
    [SerializeField]
    private MinMaxRange crackSizeRange = new MinMaxRange(0, 1000);

    protected void Awake()
    {
        if (animationPlayer == null)
        {
            animationPlayer = GetComponent<Animation>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    protected void Update()
    {
        if (!animationPlayer.isPlaying)
        {
            DisableObject();
        }
    }

    public void DisplayOfPotision(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
        rectTransform.rotation = TransformUtil.GetRandomZRotation();

        float crackSize = crackSizeRange.RandOfRange();
        rectTransform.sizeDelta = Vector2.right * crackSize + Vector2.up * crackSize;

        animationPlayer.Play();

        EnableObject();
    }
}
