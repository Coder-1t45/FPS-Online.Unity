using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Bullet : MonoBehaviour
{
    static public Dictionary<int, Bullet> list = new Dictionary<int, Bullet>();
    static public int NextId = 1;
    public const float speed = 80f;
    public const float time = 7f;

    public int Id;
    public ushort PlayerId;
    public int damage;
    public Vector3 previusPosition;

    private float timer;
    public static void Summon(int damage,Vector3 _position, Vector3 _forward, ushort pid)
    {
        Bullet bullet = Instantiate(GameLogic.Singleton.bulletPrefab, _position, Quaternion.identity).AddComponent<Bullet>();
        bullet.Id = Bullet.NextId;
        NextId++;
        bullet.damage = damage;
        bullet.PlayerId = pid;

        bullet.transform.forward = _forward;
        bullet.transform.position = bullet.previusPosition = _position;

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.bulletSpawned);
        message.AddUShort(bullet.PlayerId);
        message.AddInt(bullet.Id);
        message.AddVector3(_position);
        message.AddVector3(_forward);
        NetworkManager.Singleton.Server.SendToAll(message);

        list.Add(bullet.Id, bullet);
    }
    public void FixedUpdate()
    {
        previusPosition = transform.position;
        transform.position += transform.forward * speed * Time.fixedDeltaTime;

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.bulletMovement);
        message.AddInt(this.Id);
        message.AddVector3(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);

        CollisionDetector();
        
        timer += Time.fixedDeltaTime;
        if (timer > time)
            Destroy();
    }

    public void Destroy()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.bulletDestroy);
        message.AddInt(this.Id);
        NetworkManager.Singleton.Server.SendToAll(message);


        list.Remove(this.Id);
        Destroy(gameObject);
    }

    private void CollisionDetector()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Ray(previusPosition, (transform.position - previusPosition).normalized),
            (transform.position - previusPosition).magnitude);

        for (int i = 0; i < hits.Length; i++)
        {
            CollisionEnter(hits[i].collider, hits[i].normal);
        }
    }
    private void CollisionEnter(Collider collider, Vector3 normal)
    {
        if (collider.TryGetComponent<Player>(out Player player) && (player.Id != PlayerId))
        {
            if (player.health > 0 && player.health - damage <= 0) { 
                Player.list[PlayerId].kills++;
                Player.list[PlayerId].UpdateStats();
            }

            player.TakeDamage(damage, PlayerId);
        }
        Destroy();
    }
}
