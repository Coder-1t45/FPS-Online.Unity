using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }
    public int health;
    public PlayerStats stats = new PlayerStats();
    public Transform camTransform;

    public string username;
    
    private void OnDestroy()
    {
        list.Remove(Id);
    }

    private void Move(int health, Vector3 camForward, ushort animeState, Vector3 newPosition, Vector3 forward, bool wallrunning, int wallside, float jumpCounter, bool sliding)
    {
        transform.position = newPosition;
        this.health = health;
        if (!IsLocal)
        {
            //UnLocal player
            transform.forward = forward;
            GetComponent<PlayerAnimationManager>().state = (PlayerAnim)animeState;
            
            GetComponent<LocalPlayer>().camForward = camForward;
            GetComponent<LocalPlayer>().wallrunning = wallrunning;
            GetComponent<LocalPlayer>().side = wallside;
        }
        else
        {
            //Local player
            GetComponent<PlayerController>().jumpCounter = jumpCounter;
            camTransform.GetComponent<CameraController>().wallrunning = wallrunning;
            camTransform.GetComponent<CameraController>().wallside = wallside;
            camTransform.GetComponent<CameraController>().sliding = sliding;
        }
    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        Player player;
        if (id == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} (username)";
        player.Id = id;
        player.username = username;
        player.health = 100;

        list.Add(id, player);
    }

    #region Messages
    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        var id = message.GetUShort();
        if (list.ContainsKey(id)) return;

        Spawn(id, message.GetString(), message.GetVector3());
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetInt(), message.GetVector3(), message.GetUShort(), message.GetVector3(), message.GetVector3(), message.GetBool(), message.GetInt(), message.GetFloat(), message.GetBool());
    }

    [MessageHandler((ushort)ServerToClientId.updateStats)]
    private static void UpdateStats(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.stats.Collect(message.GetInt(), message.GetInt());
    }

    [MessageHandler((ushort)ServerToClientId.playerDeath)]
    private static void PlayerDeath(Message message)
    {
        // 
        if (list.TryGetValue(message.GetUShort(), out Player player)) {
            short reason = message.GetShort(); // -1 ? fall from map : players
            player.Death(reason);
        }
    }

    private void Death(short reason)
    {
        //alert ActionsTab
        if (reason >= 0) ActionTab.PlayerKill((ushort)reason, this.Id);
        else if (reason == -1) ActionTab.PlayerFall(this.Id);
        if (IsLocal)
        {
            var ui = GetComponentInChildren<PlayerUI>();
            ui.Death(reason);
        }
        else
        {
            LocalPlayer local = GetComponent<LocalPlayer>();
            local.Death();
        }
    }

    [MessageHandler((ushort)ServerToClientId.playerRespawned)]
    private static void PlayerRespawned(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Respawn();
    }

    private void Respawn()
    {
        if (IsLocal)
        {
            var ui = GetComponentInChildren<PlayerUI>();
            ui.Respawn();
        }
        else
        {
            LocalPlayer local = GetComponent<LocalPlayer>();
            local.Respawn();
        }
    }


    #endregion
}
