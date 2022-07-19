using RiptideNetworking;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class PlayerWeapon : MonoBehaviour
{
    public static Dictionary<ushort, PlayerWeapon> Singleton { get
        {
            Dictionary<ushort, PlayerWeapon> n = new Dictionary<ushort, PlayerWeapon>();
            foreach (var item in Player.list)
            {
                n.Add(item.Key, item.Value.GetComponent<PlayerWeapon>());
            }
            return n;
        } }
    public ushort MaxAmmo;
    public float ShootingTime;
    public float ReloadingTime;

    public int Pellets;
    public float ShootRadius;

    public int HitDamage;

    [HideInInspector] public ushort ammo;
    private bool reloading, shooting;
    private bool actable { get { return !reloading && !shooting; } }
    public ushort id { get { return GetComponent<Player>().Id; } }


    IEnumerator Shoot(Vector3 camPosition, Vector3 position, Vector3 forward, Vector3 right, Vector3 up, float accuracy)
    {
        shooting = true;
        ammo--;
        UpdateWeapon();
        Vector3 currForward = forward;
        for (int i = 0; i < Pellets; i++)
        {
            var pointInCircle = UnityEngine.Random.insideUnitCircle * ShootRadius * accuracy;
            currForward += pointInCircle.y * up + pointInCircle.x * right;
            if (Physics.Raycast(camPosition, currForward, out RaycastHit hit))
            {
                Bullet.Summon(HitDamage, position, (hit.point - position).normalized, id);
            }
            else
                Bullet.Summon(HitDamage, position, currForward, id);

            currForward = forward;
        }
        
        yield return new WaitForSeconds(ShootingTime);
        shooting = false;
    }
    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(ReloadingTime);
        ammo = MaxAmmo;
        UpdateWeapon();
        reloading = false;
    }

    public void UpdateWeapon()
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.weaponTrigger);
        message.Add(ammo);
        NetworkManager.Singleton.Server.Send(message, GetComponent<Player>().Id);
    }
    [MessageHandler((ushort)ClientToServerId.weaponInisiate)]
    public static void WeaponInisiate(ushort fromClientId, Message message)
    {
        Singleton[fromClientId].weaponInisiate(message.GetUShort(), message.GetInt(), message.GetFloat(), message.GetFloat(), message.GetInt(), message.GetFloat());
    }
    public void weaponInisiate(ushort maxAmmo, int damage, float shootTime, float reloadTime, int pellets, float radius)
    {
        MaxAmmo = ammo = maxAmmo;
        HitDamage = damage;
        ShootingTime = shootTime;
        ReloadingTime = reloadTime;
        Pellets = pellets;
        ShootRadius = radius;
    }


    [MessageHandler((ushort)ClientToServerId.weaponShoot)]
    public static void WeaponShoot(ushort fromClientId, Message message)
    {
        Singleton[fromClientId].weaponShoot(message.GetVector3(), message.GetVector3(), message.GetVector3(), message.GetVector3(), message.GetVector3(), message.GetFloat());
    }
    public void weaponShoot(Vector3 cam_position, Vector3 position, Vector3 forward, Vector3 right, Vector3 up, float accuracy)
    {
        if (actable && ammo > 0)
        {
            position += GetComponent<Rigidbody>().velocity * Time.deltaTime * 2f;
            StartCoroutine(Shoot(cam_position, position , forward, right, up, accuracy));
        }

    }

    [MessageHandler((ushort)ClientToServerId.weaponReload)]
    public static void WeaponReload(ushort fromClientId, Message message)
    {
        Singleton[fromClientId].weaponReload();
    }
    public void weaponReload()
    {
        if (actable && ammo < MaxAmmo)
        {
            StartCoroutine(Reload());
        }
    }
}
