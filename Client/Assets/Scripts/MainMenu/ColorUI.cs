using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{
    public InputField number => GetComponent<InputField>();
    public Image show => GetComponent<Image>();
    public Color value { get { return show.color; } set { show.color = value; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        number.onEndEdit.AddListener(delegate {
            if (ColorUtility.TryParseHtmlString(number.text, out Color color))
            {
                show.color = color;
            }
        });
        
    }
}
