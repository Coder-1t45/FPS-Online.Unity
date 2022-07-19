using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMousrTweak : MonoBehaviour
{
    public float ammount;
    private Vector3 lerped;
    private Vector3 adadad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mPositionUI = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2,0) *2;
        var scaledMouse = new Vector3(mPositionUI.y / Screen.height, mPositionUI.x / Screen.width, 0);
        lerped = Vector3.SmoothDamp(lerped, scaledMouse.Clamp(-1, 1), ref adadad, 0.2f);
        transform.localRotation = Quaternion.Euler(Vector3.zero + lerped * -ammount);
        
    }
}
