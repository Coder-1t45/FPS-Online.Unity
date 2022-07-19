using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    public Text text;
    private Slider slider => GetComponent<Slider>();
    private void Awake()
    {
        text.text = slider.value.ToString2();
    }
    void Update()
    {
        slider.onValueChanged.AddListener(delegate { text.text = slider.value.ToString2(); });
    }
}
