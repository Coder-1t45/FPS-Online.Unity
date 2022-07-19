using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CursorUI
{
    public bool showAccuracy = true;
    public Color color = Color.white;
    public float thickness = 10;
    public float crosshairLength = 40; 
    public float centerGap = 100;
    public float opacity = 100;
    public float dotSize = 10;
    public float dotOpacity = 100;

    public void Set(bool a, Color c, float t, float l, float g, float o, float d, float dO)
    {
        this.showAccuracy = a;
        this.color = c;
        this.thickness = t;
        this.crosshairLength = l;
        this.centerGap = g;
        this.opacity = o;
        this.dotSize = d;
        this.dotOpacity = dO;
    }
}
