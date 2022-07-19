using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolUI : MonoBehaviour
{
    public Button button => GetComponent<Button>();
    public Text stateText => GetComponentInChildren<Text>();

    public bool Value;
    public void Clicked() 
    {
        Value = !Value;
        stateText.text = Value.ToString();
    }
    public void SetValue(bool v) 
    {
        Value = v;
        stateText.text = v.ToString();
    }
}
