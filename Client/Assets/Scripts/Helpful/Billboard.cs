using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private void LateUpdate()
    {
        Transform cam = Player.list[NetworkManager.Singleton.Client.Id].GetComponentInChildren<Camera>().transform;
        transform.LookAt(transform.position + cam.forward);
    }
}
