using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public Button lowerButton;
    public Text stateText;
    public Button upperButton;
    public string[] states;
    public int currentIndex;
    [HideInInspector] public string currentState => states[currentIndex];

    void Update()
    {
        upperButton.onClick.AddListener(Add);
        lowerButton.onClick.AddListener(Remove);
        stateText.text = states[currentIndex];
    }
    public void Add()
    {
        if (currentIndex == states.Length - 1)
            currentIndex = 0;
        else
            currentIndex = currentIndex + 1;
    }
    public void Remove()
    {
        if (currentIndex == 0)
            currentIndex = states.Length - 1;
        else
            currentIndex = currentIndex - 1;
    }
}
