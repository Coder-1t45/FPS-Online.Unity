using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

static class HelperMethods
{
    public static List<GameObject> GetChildren(this GameObject go)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform tran in go.transform)
        {
            children.Add(tran.gameObject);
        }
        return children;
    }
    public static Text GetText(this GameObject go)
    {
        return go.GetComponent<Text>();
    }
    public static Text GetText(this Transform go)
    {
        return go.GetComponent<Text>();
    }

    public static string ToString2(this float f)
    {
        bool b = Math.Round(f) == f;
        return b ? f.ToString() : f.ToString(".00");
    }

    public static int Index(this string[] l, string s)
    {
        for (int i = 0; i < l.Length; i++)
        {
            if (l[i] == s) return i;
        }
        return -1;
    }
    public static bool IsAllEmpty(this string[] l)
    {
        for (int i = 0; i < l.Length; i++)
            if (l[i] != string.Empty)
                return false;
        return true;
    }
    public static void IfEmptyValue(this string s, string value)
    {
        if (s == string.Empty) s = value;
    }

    public static RectTransform rect(this Transform t)
    {
        return t.GetComponent<RectTransform>();
    }
    public static Image image(this RectTransform t)
    {
        return t.GetComponent<Image>();
    }   
    public static Vector3 Clamp(this Vector3 v, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(v.x, min, max),
            Mathf.Clamp(v.y, min, max),
            Mathf.Clamp(v.z, min, max));
    }


    static Dictionary<Image, Color> imagesa = new Dictionary<Image, Color>();
    static Dictionary<Text, Color> textsa = new Dictionary<Text, Color>();
    public static void UISetOpacity(this GameObject g, float f)
    {
        foreach (var item in g.GetComponentsInChildren<Image>())
        {
            var c = item.color;
            if (!imagesa.ContainsKey(item))
                imagesa.Add(item, c);
            item.color = new Color(c.r, c.g, c.b, imagesa[item].a * (f / 100));

        }

        foreach (var item in g.GetComponentsInChildren<Text>())
        {
            var c = item.color;
            if (!textsa.ContainsKey(item))
                textsa.Add(item, c);
            item.color = new Color(c.r, c.g, c.b, textsa[item].a * (f / 100));

        }
    }
}
