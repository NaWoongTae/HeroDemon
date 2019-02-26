using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void DrawGraph(Vector2 start, Vector2 end, float _lineWidth = 1.0f)
    {
        Vector2 differenceVector = end - start;
        rectTransform.sizeDelta = new Vector2(differenceVector.magnitude, _lineWidth);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.position = start;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
