using UnityEngine;
using System.Collections.Generic;
using RiptideNetworking;
using System;

public enum MatchMode : ushort
{
    deathmatch = 1,
    teamDeathmatch
}

[System.Serializable]
public class PlayerStats
{
    public int kills;
    public int deaths;
    public float kOd { get { return deaths != 0 ? kills / deaths : kills; } }
    public PlayerStats(int k, int d)
    {
        this.kills = k;
        this.deaths = d;
    }
    public PlayerStats()
    {
        this.kills = 0;
        this.deaths = 0;
    }
    public PlayerStats(PlayerStats other)
    {
        this.kills = other.kills;
        this.deaths = other.deaths;
    }

    public void Collect(int k, int d)
    {
        this.kills = k;
        this.deaths = d;
    }
}
public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public GameObject LocalPlayerPrefab => localPlayerPrefab;
    public GameObject PlayerPrefab => playerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;

    

    [SerializeField] public GameObject bulletPrefab;

    [Header("Match")]
    public MatchMode gameMode;
    public float time;
    public int score;


    [HideInInspector] public float timer;
    private bool started;

    public Dictionary<ushort, PlayerStats> playerStats { get
        {
            Dictionary<ushort, PlayerStats> l = new Dictionary<ushort, PlayerStats>();
            foreach (var item in Player.list.Values)
            {
                l.Add(item.Id, item.stats);
            }
            return l;
        } 
    }
    private void Update()
    {
        if (started)
            timer += Time.deltaTime;
    }

    public void GameStarting()
    {
        started = true;
    }

    private void Awake()
    {
        Singleton = this;
    }

    [MessageHandler((ushort)ServerToClientId.sendData)]
    private static void SendData(Message message)
    {
        Singleton.GetData(message.GetInt(), message.GetUShort(), message.GetFloat());
    }
    public void GetData(int score, ushort gamemode, float time)
    {
        this.score = score;
        this.gameMode = (MatchMode)gamemode;
        this.time = time;
        UIManager.Singleton.DisplayMode(this.score, this.gameMode, this.time);
    }

    
}
