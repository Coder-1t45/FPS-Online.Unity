using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFolderUI : MonoBehaviour
{
    public void Awake()
    {
        int selected = 0;
        int i = 0;
        foreach (Transform child in transform)
        {
            if (i == selected) child.gameObject.SetActive(true);
            else child.gameObject.SetActive(false);
            i++;
        }
    }
    public void Select(int selected)
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            if (i == selected) child.gameObject.SetActive(true);
            else child.gameObject.SetActive(false);
            i++;
        }
    }

}
