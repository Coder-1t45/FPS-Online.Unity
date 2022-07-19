using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public float position = 10f;
    public float rotation = 180f;

    private Vector3 originalPosition;
    private Vector3 originalRotation;

    private Vector3 velocity;
    private Vector3 lastPosition;

    private float x, y;


    //Weapon Stats:
    public int Ammo, MaxAmmo;
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = new Vector3(0, 90, 0);
    }

    void Update()
    {
        if (MouseRules.Focus) { 
            x = Input.GetAxis("Mouse X") * Time.deltaTime * position;
            y = Input.GetAxis("Mouse Y") * Time.deltaTime * position;
        }
        else
        {
            x = 0;
            y = 0;
        }
        velocity = Vector3.Lerp(velocity, (transform.position - lastPosition) / Time.deltaTime, Time.deltaTime * 10f);
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        var _position = originalPosition + new Vector3(-x, -y);
        var _rotation = Quaternion.Euler(originalRotation + new Vector3(0, -x, -y) * rotation);

        var vel = new Vector3(-velocity.x / 1000, velocity.y / -300, velocity.z / 1000) * Time.deltaTime * 500;
        transform.localPosition = Vector3.Lerp(transform.localPosition, _position + vel, Time.deltaTime * 7f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _rotation, Time.deltaTime * 7f);

        PlayerUI canvas = PlayerUI.Singleton;
        CursorView cursor = canvas.cursorView;
        cursor.accuracyValue = velocity.magnitude;

    }
}
