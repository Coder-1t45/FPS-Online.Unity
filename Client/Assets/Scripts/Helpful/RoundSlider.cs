using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundSlider : MonoBehaviour
{
    public Image filler;
    public float MaxValue = 1;
    public float MinValue = 0;
    public float value;
    void Update()
    {
        float ammount = (value / MaxValue) * 1/2;
        filler.fillAmount = ammount;
    }
}
