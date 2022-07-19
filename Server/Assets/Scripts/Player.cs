using RiptideNetworking;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class WaitingClient
{
    public ushort id;
    public string name;
    public bool ready;

    public WaitingClient(ushort id, string name, bool ready)
    {
        this.id = id;
        this.name = name;
        this.ready = ready;
    }
    public void SendSpawned(ushort id)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.clientJoined)), id);
    }
    public void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.clientJoined)));
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(id);
        message.AddString(name);
        message.AddBool(ready);
        return message;
    }

}
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public static Dictionary<ushort, WaitingClient> waiting = new Dictionary<ushort, WaitingClient>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    public PlayerMovement Movement => movement;
    public Rigidbody rb => GetComponent<Rigidbody>();

    public int health;
    public const int MaxHealth = 100;

    public int kills;
    public int death;

    public bool respawning;

    [SerializeField] private PlayerMovement movement;

    private void OnDestroy()
    {
        list.Remove(Id);
    }
    private void FixedUpdate()
    {
        if ((transform.position.y < -100 || health <= 0) && !respawning)
            StartCoroutine(Respawn(-1));
    }
    public static void RestartServer()
    {
        waiting = new Dictionary<ushort, WaitingClient>();
    }
    public static void Spawn(ushort id, string username)
    {
        foreach (Player otherPlayer in list.Values)
            otherPlayer.SendSpawned(id);

        SpawnPoint spawn = SpawnPoint.GetSpawnPoint();
        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, spawn.pos, spawn.rot).GetComponent<Player>();

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;

        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;
        player.health = MaxHealth;

        player.SendSpawned();
        list.Add(id, player);
    }
    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)));
    }
    public void SendSpawned(ushort id)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)), id);
    }

    public static void AllSpawn()
    {
        foreach (var item in waiting)
        {
            Spawn(item.Key, item.Value.name);
        }
    }
    public static void SpawnWaiting(WaitingClient w)
    {
        foreach (WaitingClient otherPlayer in waiting.Values) {
            otherPlayer.SendSpawned(w.id);
        }

        waiting.Add(w.id, w);
        w.SendSpawned();
    }

    public void UpdateStats()
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.updateStats);
        message.AddUShort(Id);
        message.AddInt(kills);
        message.AddInt(death);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public IEnumerator Respawn(short state)
    {
        //Send Death
        Message message1 = Message.Create(MessageSendMode.reliable, ServerToClientId.playerDeath);
        message1.AddUShort(Id);
        message1.AddShort(state);
        NetworkManager.Singleton.Server.SendToAll(message1);

        //Reset Movement keymap
        movement.enabled = false;
        respawning = true;
        death++;
        UpdateStats();

        //Reset Physics
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        //Waiting 3 seconds before reviving..
        yield return new WaitForSeconds(3);

        SpawnPoint spawn = SpawnPoint.GetClearestPoint();
        transform.position = spawn.pos;
        transform.rotation = spawn.rot;

        //Giving back our player physics
        rb.useGravity = true;
        movement.enabled = true;

        //Restoring health
        health = MaxHealth;
        respawning = false;

        //Send Respawend
        Message message2 = Message.Create(MessageSendMode.reliable, ServerToClientId.playerRespawned);
        message2.AddUShort(Id);
        NetworkManager.Singleton.Server.SendToAll(message2);

    }
    public void TakeDamage(int damage, ushort playerID)
    {
        if (respawning) return;
        health -= damage;

        if (health <= 0)
        {
            StartCoroutine(Respawn((short)playerID));
        }
    }
    #region Messages

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        if (NetworkManager.GameStarted) {
            NetworkManager.GameAlreadyStarted(fromClientId); return;
        }
        SpawnWaiting(new WaitingClient(fromClientId, message.GetString(), false));
        UIManager.ListUpdate();
    }

    [MessageHandler((ushort)ClientToServerId.ready)]
    private static void Ready(ushort fromClientId, Message message)
    {
        waiting[fromClientId].ready = message.GetBool();

        Message m = Message.Create(MessageSendMode.reliable, ServerToClientId.ready);

        m.AddUShort(fromClientId);
        m.AddBool(waiting[fromClientId].ready);

        NetworkManager.Singleton.Server.SendToAll(m);

        isAllReady();
    }
    static float timer = 3;
    static bool isallreadys;
    public static void isAllReady()
    {
        timer = 3;
        isallreadys = waiting.Count > 0;
        foreach (var p in waiting)
        {
            if (!p.Value.ready)
                isallreadys = false;
        }
    }
    public static void Update()
    {
        if (isallreadys) {
            timer -= Time.deltaTime;


            if (timer <= 0)
            {
                UIManager.Log(NetworkManager.ServerSign() + "Game Starting..");
                AllSpawn();

                NetworkManager.Singleton.StartGame();
            }
        }
    }

    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
            player.Movement.SetInput(message.GetBools(7), message.GetVector3(), message.GetVector3());
    }
    #endregion
}
