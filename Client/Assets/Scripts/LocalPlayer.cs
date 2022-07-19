using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class LocalPlayer : MonoBehaviour
{
    private Player player => GetComponent<Player>();
    public Slider healthBar;
    public Text nameText;

    public Transform mainBone;
    public Transform faceOriginalLocationToMakeForIk;
    public GameObject head;
    public GameObject canvas;
    private float y;

    [Header("SaveModelPos")]
    public float height = 0.9f;
    public Transform modelParent;
    public float angle = 11.4f;

    public Rig ik;
    public bool wallrunning;
    public int side;
    // Start is called before the first frame update
    void Start()
    {
        y = canvas.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas.activeSelf) { 
            var v = canvas.transform.localPosition;
            canvas.transform.localPosition = new Vector3(v.x, y + head.transform.localPosition.y ,v.z);

            nameText.text = player.username;
            healthBar.value = Mathf.Lerp(healthBar.value, player.health / 100, Time.deltaTime * 12f);

            faceOriginalLocationToMakeForIk.position =  mainBone.position + camForward * 5f;

            modelParent.localPosition = new Vector3(0, height, 0);
            modelParent.localRotation = Quaternion.Euler(0, angle, 0);

            ik.weight = wallrunning && side == -1 ? 1 : 0;
        }
        
    }
    private void LateUpdate()
    {
        if (canvas.activeSelf)
        {
            modelParent.localPosition = new Vector3(0, height, 0);
            modelParent.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public Renderer[] fullModel;
    public Vector3 camForward;

    public void Death()
    {
        foreach (var item in fullModel)
        {
            item.enabled = false;
        }
        canvas.SetActive(false);
    }

    public void Respawn()
    {
        foreach (var item in fullModel)
        {
            item.enabled = true;
        }
        canvas.SetActive(true);
    }
}
