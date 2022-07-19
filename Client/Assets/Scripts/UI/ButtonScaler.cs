using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private bool mousehover;
    private bool mouseclick;
    public void OnPointerEnter(PointerEventData eventData)
    { mousehover =true; }
    public void OnPointerDown(PointerEventData eventData) 
    { mouseclick = true; }  
    public void OnPointerUp(PointerEventData eventData)
    { mouseclick = false; }
    public void OnPointerExit(PointerEventData eventData)
    { mousehover = false; }

    private const float SizeUp = 1.05f;
    private const float SizeDown = 0.95f;
    private const float Smoothness = 0.04f;

    public float multi = 1;
    private Vector3 sScale;
    private Vector3 r;
    private void Awake()
    {
        sScale = transform.localScale;
    }
    public void Update()
    {
        multi = multi == 0 ? 1 : multi;

        if (mouseclick)
            transform.localScale = Vector3.SmoothDamp(transform.localScale, sScale * SizeDown * multi,ref r, Smoothness);
        else if (mousehover)
            transform.localScale = Vector3.SmoothDamp(transform.localScale, sScale * SizeUp * multi, ref r, Smoothness);
        else
            transform.localScale = Vector3.SmoothDamp(transform.localScale, sScale * multi, ref r, Time.deltaTime * Smoothness);
    }
}