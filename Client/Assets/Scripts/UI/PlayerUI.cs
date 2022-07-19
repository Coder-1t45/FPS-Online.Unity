using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    
    public Player player => transform.parent.GetComponent<Player>();
    static public PlayerUI Singleton => Player.list[NetworkManager.Singleton.Client.Id].GetComponentInChildren<PlayerUI>();
    public CursorView cursorView;
    //PlayerUI
    public float radius;
    [SerializeField] private Image cursor;
    public Slider healthSlider;
    public Text rnTimer;
    public Text setTimer;

    public Text maxAmmoTx;
    public Text ammoTx;

    //Menu
    public GameObject menuPanel;
    public bool menu;
    //Respawn Screen
    public GameObject RespawnPanel;
    public Text repsawnText;
    public Text respawnReason;
    private float rtimer;

    private void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, player.health / 100f, Time.deltaTime * 10f);
        var logic = FindObjectOfType<GameLogic>();
        setTimer.text = logic.time.ToString2();
        rnTimer.text = logic.timer.ToString2();

        ammoTx.text = Weapon.Singleton.ammo.ToString();
        maxAmmoTx.text = Weapon.Singleton.MaxAmmo.ToString();

        if (Input.GetKeyDown(InputManager.cancell))
        {
            menu = !menu;
            if (menu) menuPanel.SetActive(true); else menuPanel.SetActive(false);
            MouseRules.escape = menu;
        }

    }
    public void FixedUpdate()
    {
        //cursor.transform.localScale = Vector3.one * radius * 2;
        if (RespawnPanel.activeSelf) { 
            rtimer -= Time.deltaTime;
            repsawnText.text = "Respawning in " + ((int)Math.Ceiling(rtimer)).ToString() + " sec";
        }
    }

    public void Death(short r)
    {
        MouseRules.respawn = true;
        RespawnPanel.SetActive(true);
        respawnReason.text = (r == -1 ? "Fallen From Map" : "Killed By Player '" + Player.list[(ushort)r].username + "'").ToLowerInvariant();
        rtimer = 3;
    }

    public void Respawn()
    {
        MouseRules.respawn = false;
        RespawnPanel.SetActive(false);
    }

    public void Disconnect()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            NetworkManager.Singleton.Client.Disconnect();
        MouseRules.escape = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
