using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchMode:ushort
{
    deathmatch = 1,
    teamDeathmatch
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

    public GameObject PlayerPrefab => playerPrefab;


    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    public GameObject bulletPrefab;

    [Header("Match")]
    public MatchMode gameMode;
    public float time;
    public int score;

    private float timer;
    public bool WIN;

    [HideInInspector]public bool started = false;
    public void Initialise(MatchMode mode, float time, int score)
    {
        this.gameMode = mode;
        this.time = time;
        this.score = score;
    }
    public void GameStarted()
    {
        started = true;
        UIManager.Log( NetworkManager.ServerSign()+ "Game Started");
    }
    public void Update()
    {
        if (!GetComponent<NetworkManager>().gamestarted)
            timer = 0;

        if (!started)
            return;
        
        timer += Time.deltaTime;

        switch (gameMode)
        {
            case MatchMode.deathmatch:
                Deathmatch(time, score);
                break;
            case MatchMode.teamDeathmatch:
                break;
            default:
                break;
        }
        
    }
    private void Deathmatch(float time, int score)
    {
        if (WIN) StopMatch();
        if (score > 0) { 
            var p = Player.list;
            foreach (var item in p)
            {
                if (item.Value.kills >= score)
                {
                    StopMatch();
                }
            }
        }
        if (time > 0)
        {
            if (time <= timer)
            {
                StopMatch();
            }
        }
    }

    private void StopMatch()
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.gameWin);
        
        Dictionary<ushort, Player> player = new Dictionary<ushort, Player>(Player.list);
        for (int timei = 0; timei < 3; timei++)
        {
            ushort index = 0;
            int value = int.MinValue;
            string name = "";
            foreach (var p in player)
            {
                if (p.Value.kills > value) {
                    index = p.Key;
                    value = p.Value.kills;
                    name = p.Value.Username;
                }
            }
            player.Remove(index);
            message.AddString(name);
            
        }
        NetworkManager.Singleton.Server.SendToAll(message);
        started = false;
    }

    private void Awake()
    {
        Singleton = this;
    }
}
