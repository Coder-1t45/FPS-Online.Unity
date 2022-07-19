using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Bullet : MonoBehaviour
{
    static public Dictionary<int, Bullet> list = new Dictionary<int, Bullet>();
    public int Id;

    public static void Spawn(int _id, Vector3 _position, Vector3 _forward)
    {
        Bullet bullet = Instantiate(GameLogic.Singleton.bulletPrefab, _position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Id = _id;
        bullet.transform.forward = _forward;

        if (!list.ContainsKey(_id))
            list.Add(_id, bullet);
    }

    [MessageHandler((ushort)ServerToClientId.bulletSpawned)]
    public static void BulletSpawned(Message message)
    {
        ushort playerID = message.GetUShort();
        if (Player.list[playerID].TryGetComponent(out PlayerAnimationManager anim))
        {
            anim.Shoot();
        }
        Spawn(message.GetInt(), message.GetVector3(), message.GetVector3());
    }

    [MessageHandler((ushort)ServerToClientId.bulletMovement)]
    public static void BulletMovement(Message message)
    {
        list[message.GetInt()].transform.position = message.GetVector3();
    }

    [MessageHandler((ushort)ServerToClientId.bulletDestroy)]
    public static void BulletDestroy(Message message)
    {
        ushort id = message.GetUShort();

        if (!list.ContainsKey(id))
            return;
        Destroy(list[id].gameObject);
        list.Remove(id);
    }
}
