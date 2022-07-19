using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorMnagement : MonoBehaviour
{
    private static CursorMnagement _singleton;
    public static CursorMnagement Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(CursorMnagement)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public BoolUI showAccuracy;
    public ColorUI color;
    public Slider thickness;
    public Slider crosshairLength;
    public Slider centerGap;
    public Slider opacity;
    public Slider dotSize;
    public Slider dotOpacity;

    public RectTransform Show => transform.GetChild(1).rect();
    public RectTransform dot => Show.GetChild(0).rect();
    public RectTransform upper => Show.GetChild(1).rect();
    public RectTransform buttom => Show.GetChild(2).rect();
    public RectTransform right => Show.GetChild(3).rect();
    public RectTransform left => Show.GetChild(4).rect();

    public static CursorUI cursorSettings = new CursorUI();
    
    private float pFromDot = 15;

    // Update is called once per frame
    private void Awake()
    {
        //Load Data
        Singleton = this;
        this.getData(cursorSettings);
    }
    void Update()
    {
        float dsize = dotSize.value * 10;
        dot.sizeDelta = new Vector2(dsize, dsize);
        Color c = dot.image().color;
        dot.image().color = new Color(c.r, c.g, c.b, dotOpacity.value / 100);

        foreach (var item in new RectTransform[4] { upper, buttom, right, left })
        {
            c = item.image().color;
            item.image().color = new Color(c.r, c.g, c.b, opacity.value / 100);
        }

        foreach (var item in new RectTransform[2] { upper, buttom })
        {
            item.sizeDelta = new Vector2(2 * thickness.value, 3 * crosshairLength.value);
        }
        foreach (var item in new RectTransform[2] {right, left })
        {
            item.sizeDelta = new Vector2( 3 * crosshairLength.value, 2 * thickness.value);
        }
        float accuracySlim = showAccuracy.Value ? (Mathf.Sin(Time.time * 2f) + 1 ) * 20 :0;
        right.localPosition = new Vector3(pFromDot + centerGap.value * 2 + accuracySlim, right.localPosition.y);
        left.localPosition = new Vector3(-pFromDot - centerGap.value * 2 - accuracySlim, left.localPosition.y);
        upper.localPosition = new Vector3(upper.localPosition.x, pFromDot + centerGap.value * 2 + accuracySlim);
        buttom.localPosition = new Vector3(buttom.localPosition.x, -pFromDot - centerGap.value * 2 - accuracySlim);

        Apply();
    }
    public static readonly CursorUI normalSettings = new CursorUI()
    {
        showAccuracy = false,
        color = Color.white,
        thickness = 8,
        crosshairLength = 60,
        centerGap = 0,
        opacity = 1,
        dotSize = 30,
        dotOpacity = 1
    };
    public void Apply()
    {
        cursorSettings.Set(showAccuracy.Value,
            color.value,
            thickness.value * 2,
            crosshairLength.value * 3,
            centerGap.value * 2,
            opacity.value / 100,
            dotSize.value * 10,
            dotOpacity.value / 100
        );
        
        
    }
    public static void GetData(CursorUI cursor)
    {
        Singleton.getData(cursorSettings);
    }
    public void getData(CursorUI cursor)
    {
        showAccuracy.Value = cursor.showAccuracy;
        color.value = cursor.color;
        thickness.value = cursor.thickness / 2;
        crosshairLength.value = cursor.crosshairLength / 3;
        centerGap.value = cursor.centerGap / 2;
        opacity.value = cursor.opacity * 100;
        dotSize.value = cursor.dotSize / 10;
        dotOpacity.value = cursor.dotOpacity * 100;
        
    }
}
