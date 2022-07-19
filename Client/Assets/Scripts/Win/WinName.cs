using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinName : MonoBehaviour
{
    static Dictionary<int, WinName> list = new Dictionary<int, WinName>();
    public int place;
    private void Awake()
    {
        list.Add(place, this);
        Debug.Log("ADDED");
    }
    bool done;
    private void Update()
    {
        b = list.Count >= 3;
    }
    static bool b;
    public static IEnumerator Inisiate(Dictionary<int, string> players)
    {
        Debug.Log("Started WinNAme.Inisiate!");
        yield return new WaitUntil(() => b);
        Debug.Log("WAITED");
        foreach (var item in list)
        {
            item.Value.GetComponent<Text>().text = players[item.Key];
        }
    }
}
