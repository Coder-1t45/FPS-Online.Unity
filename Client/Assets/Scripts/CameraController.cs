using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    public bool wallrunning;
    public int wallside;
    public bool sliding;
    private float side;
    private float slidingAmmount;

    public Vector2 offset;

    private void OnValidate()
    {
        if (player == null)
            player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleCursorMode();

        if (Cursor.lockState == CursorLockMode.Locked)
            Look();

        Debug.DrawRay(transform.position, transform.forward * 2f, Color.green);
    }
    private void FixedUpdate()
    {

    }
    private void Look()
    {
        float mouseVertical = -Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("ySens", 25) / 25f;
        float mouseHorizontal = Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("xSens", 25) / 25f;

        verticalRotation += mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);
        side = wallrunning ? Mathf.Lerp(side, wallside, Time.deltaTime * 7f) : Mathf.Lerp(side, 0, Time.deltaTime * 7f);
        slidingAmmount = Mathf.Lerp(slidingAmmount, (sliding ? 10 : 0), Time.deltaTime * 7f);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, side * 15f - slidingAmmount);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        var camera = transform.GetComponent<Camera>();
        
        var ppFOV = PlayerPrefs.GetFloat("fov", 70);
        camera.fieldOfView = wallrunning ? Mathf.Lerp(camera.fieldOfView, ppFOV * 1.1875f, Time.deltaTime * 7f)
            : Mathf.Lerp(camera.fieldOfView, ppFOV, Time.deltaTime * 7f);
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }


}
