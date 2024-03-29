using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;

public enum ServerToClientId : ushort
{
    sendData = 1,
    clientJoined,
    playerSpawned,
    ready,
    gameStarting,
    playerMovement,
    playerDeath,
    playerRespawned,
    updateStats,
    error,
    weaponTrigger,
    bulletSpawned,
    bulletMovement,
    bulletDestroy,
    gameWin,
    chat,
    kick
}

public enum ClientToServerId : ushort
{
    name = 1,
    ready,
    input,
    weaponShoot,
    weaponReload,
    weaponInisiate,
    chat
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect(string ip, ushort port)
    {
        this.ip = ip;
        this.port = port;
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.SendName();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player))
            Destroy(player.gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
        foreach (Player player in Player.list.Values)
            Destroy(player.gameObject);
    }
    [MessageHandler((ushort)ServerToClientId.error)]
    public static void Error(Message message)
    {
        UIManager.Singleton.ErrorMessage(message.GetString());
        Singleton.Client.Disconnect();
    }

    [MessageHandler((ushort)ServerToClientId.kick)]
    public static void Kick(Message message)
    {
        Singleton.Client.Disconnect();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
