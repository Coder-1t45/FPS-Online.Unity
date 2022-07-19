using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform camTransform;
    public PlayerUI playerUI;
    private bool[] inputs;
    public float jumpCounter;
    private float jCounterTimer;
    public RoundSlider jCounter;
    private void Start()
    {
        inputs = new bool[7];
        jCounter.gameObject.UISetOpacity(0);
    }
    private float jopacity;
    private void Update()
    {

        jCounter.value = Mathf.Lerp(jCounter.value, jumpCounter / 2, Time.deltaTime * 8f);
        if (jumpCounter == 2)
            jCounterTimer += Time.deltaTime;
        else jCounterTimer = 0;

        if (jCounterTimer > 1.5f)
            jopacity = Mathf.Lerp(jopacity, 0f, Time.deltaTime * 8f);
        else jopacity = Mathf.Lerp(jopacity, 100, Time.deltaTime * 8f);

        jCounter.gameObject.UISetOpacity(jopacity);


        if (!MouseRules.Focus)
            return;

        if (Input.GetKey(InputManager.forward))
            inputs[0] = true;

        if (Input.GetKey(InputManager.backward))
            inputs[1] = true;

        if (Input.GetKey(InputManager.left))
            inputs[2] = true;

        if (Input.GetKey(InputManager.right))
            inputs[3] = true;

        if (Input.GetKey(InputManager.jump))
            inputs[4] = true;

        if (Input.GetKey(InputManager.run))
            inputs[5] = true;

        if (Input.GetKey(InputManager.crouch))
            inputs[6] = true;
    }

    private void FixedUpdate()
    {
        SendInput();

        for (int i = 0; i < inputs.Length; i++)
            inputs[i] = false;
    }

    #region Messages
    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector3(transform.forward);
        message.AddVector3(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);
    }
    #endregion
}
