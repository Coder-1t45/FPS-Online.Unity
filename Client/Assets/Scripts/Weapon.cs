using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Weapon : MonoBehaviour
{
    public Transform point;
    public static Weapon Singleton;
    public float acuuracy => PlayerUI.Singleton.cursorView.accuracyValue / 20;
    public Transform camera { get
        {
            Transform p = transform.parent;

            while (p.parent != null)
                p = p.parent;
            return p.GetComponent<Player>().camTransform;
        } }
    public PlayerUI ui { get
        {
            Transform p = transform.parent;

            while (p.parent != null)
                p = p.parent;
            return p.GetComponent<PlayerController>().playerUI;
        } }

    public bool IsAutomatic;
    public ushort MaxAmmo;
    public float ShootingTime;
    public float ReloadingTime;
    public int Damage;
    public int pellets;
    public float radius;
    public ushort ammo;

    public void Inisiate()
    {
        //Inisiate weapon data;
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.weaponInisiate);
        message.AddUShort(MaxAmmo);
        message.AddInt(Damage);
        message.AddFloat(ShootingTime);
        message.AddFloat(ReloadingTime);
        message.AddInt(pellets);
        message.AddFloat(radius);

        ui.radius = radius;

        NetworkManager.Singleton.Client.Send(message);
    }
    [MessageHandler((ushort)ServerToClientId.weaponTrigger)]
    public static void WeaponTrigger(Message message) 
    {
        Singleton.ammo = message.GetUShort();
    }
    public void Update()
    {
        if (!MouseRules.Focus)
            return;

        if (IsAutomatic)
        {
            if (Input.GetKey(InputManager.fire))
                Shoot();

            if (Input.GetKey(InputManager.reload))
                Reload();
        }
        else
        {
            if (Input.GetKeyDown(InputManager.fire))
                Shoot();

            if (Input.GetKeyDown(InputManager.reload))
                Reload();
        }
    }
    public void Shoot()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.weaponShoot);
        message.AddVector3(camera.position);
        message.AddVector3(point.position);
        message.AddVector3(camera.forward);
        message.AddVector3(camera.right);
        message.AddVector3(camera.up);
        message.AddFloat(acuuracy);
        NetworkManager.Singleton.Client.Send(message);
    }
    public void Reload()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.weaponReload);
        NetworkManager.Singleton.Client.Send(message);
    }
}
