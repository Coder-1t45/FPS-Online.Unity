using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FixedInputfield : MonoBehaviour
{
    public UnityEvent<string> whenEnter;
    private bool wasFocused = false;
    private bool end = false;
    private InputField field => GetComponent<InputField>();
    void Update()
    {
        field.onEndEdit.AddListener(delegate {
            end = true; 
        });

        if (end && wasFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            whenEnter.Invoke(field.text);
            field.text = "";
        }

        wasFocused = field.isFocused;
        end = false;
    }
}
