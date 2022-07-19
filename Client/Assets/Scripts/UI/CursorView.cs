using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorView : MonoBehaviour
{
    public CursorUI cursor => CursorMnagement.cursorSettings;
    public RectTransform dot => transform.GetChild(0).rect();
    public RectTransform upper => transform.GetChild(1).rect();
    public RectTransform buttom => transform.GetChild(2).rect();
    public RectTransform right => transform.GetChild(3).rect();
    public RectTransform left => transform.GetChild(4).rect();
    private float pFromDot = 0;
    public float accuracyValue;

    // Start is called before the first frame update
    void Update()
    {
        float dsize = cursor.dotSize;
        dot.sizeDelta = new Vector2(dsize, dsize);
        Color c = dot.image().color;
        dot.image().color = new Color(c.r, c.g, c.b, cursor.dotOpacity);

        foreach (var item in new RectTransform[4] { upper, buttom, right, left })
        {
            c = item.image().color;
            item.image().color = new Color(c.r, c.g, c.b, cursor.opacity);
        }

        foreach (var item in new RectTransform[2] { upper, buttom })
        {
            item.sizeDelta = new Vector2(cursor.thickness, cursor.crosshairLength);
        }
        foreach (var item in new RectTransform[2] { right, left })
        {
            item.sizeDelta = new Vector2(cursor.crosshairLength, cursor.thickness);
        }
        float accuracySlim = cursor.showAccuracy ? accuracyValue * 2 : 0; //need to calculate accuracy by speed;
        right.localPosition = new Vector3(pFromDot + cursor.centerGap * 2 + accuracySlim, right.localPosition.y);
        left.localPosition = new Vector3(-pFromDot - cursor.centerGap * 2 - accuracySlim, left.localPosition.y);
        upper.localPosition = new Vector3(upper.localPosition.x, pFromDot + cursor.centerGap * 2 + accuracySlim);
        buttom.localPosition = new Vector3(buttom.localPosition.x, -pFromDot - cursor.centerGap * 2 - accuracySlim);
    }
}
