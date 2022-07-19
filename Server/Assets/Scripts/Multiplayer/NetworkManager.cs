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

    public Server Server { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;
    public bool isrunning;
    public bool gamestarted;
    public static bool GameStarted { get { return Singleton.gamestarted; } }

    private void Awake()
    {
        Singleton = this;
    }
    private void Start()
    {
        RiptideLogger.Initialize(UIManager.Log, UIManager.Log, UIManager.LogWarning, UIManager.LogError, true);
    }
    private void Update()
    {
        if (!gamestarted) {
            Player.Update();
        }
    }
    public bool StartServer(ushort port, ushort maxClientCount)
    {
        Application.targetFrameRate = 120;

        Server = new Server();

        this.port = port;
        this.maxClientCount = maxClientCount;

        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
        Server.ClientConnected += PlayerJoined;

        isrunning = true;
        return true;
    }

    private void PlayerJoined(object sender, ServerClientConnectedEventArgs e)
    {
        ushort id = e.Client.Id;
        int score =  GameLogic.Singleton.score;
        ushort gameMode = (ushort)GameLogic.Singleton.gameMode;
        float time = GameLogic.Singleton.time;

        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.sendData);
        message.AddInt(score);
        message.AddUShort(gameMode);
        message.AddFloat(time);
        NetworkManager.Singleton.Server.Send(message, id);

    }

    private void FixedUpdate()
    {
        if (isrunning)
        Server.Tick();
    }

    private void OnApplicationQuit()
    {
        if (isrunning)
            Server.Stop();
        isrunning = false;
    }

    public bool StopServer()
    {
        if (isrunning)
            Server.Stop();
        isrunning = false;
        return false;
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (Player.list.TryGetValue(e.Id, out Player player)) { 
            Destroy(player.gameObject);
            Player.list.Remove(player.Id);
            gamestarted = Player.list.Count == 0 ? false : gamestarted;
            if (!gamestarted)
                UIManager.Log(ServerSign() + $"Game Stopped");
        }
        if (Player.waiting.TryGetValue(e.Id, out WaitingClient waiter))
        {
            Player.waiting.Remove(waiter.id);
        }
        UIManager.Log(ServerSign() + $"Client {e.Id} has left the server...");
        Player.isAllReady();
        UIManager.ListUpdate();
    }

    public void StartGame()
    {
        
        gamestarted = true;
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.gameStarting);
        NetworkManager.Singleton.Server.SendToAll(message);

        GameLogic.Singleton.GameStarted();

    }

    public static bool GameAlreadyStarted(ushort id)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.error);
        message.AddString("The game has already started, please try again as soon as it's over");

        NetworkManager.Singleton.Server.Send(message, id);
        return true;
    }
    public static string ServerSign()
    {
        return "["+DTime.ToString(DateTime.Now)+"] (SERVER): "; 
    }

    public void Kick(ushort id)
    {
        Message m1 = Message.Create(MessageSendMode.unreliable, ServerToClientId.kick);
        Server.Send(m1, id);
    }
}
