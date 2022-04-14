using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternLine : MonoBehaviour
{
    [SerializeField]
    private float lineWidth = 0.03f;

    private RectTransform rectTransform = null;
    private Image lineImage = null;
    private Vector3 origin = Vector3.zero;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();
    }

    public void UpdateLine(Vector3 target)
    {
        Vector3 direction = target - origin;
        float dirDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float lineDistance = Vector2.Distance(origin, target);

        rectTransform.rotation = Quaternion.Euler(0, 0, dirDeg);
        rectTransform.sizeDelta = Vector2.right * lineDistance + Vector2.up * lineWidth;
    }

    public void SetOriginPos(Vector3 originPos)
    {
        origin = originPos;
        rectTransform.anchoredPosition = origin;
    }

    public void SetImageColor(Color color)
    {
        lineImage.color = color;
    }
}
