using RiptideNetworking;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReadyState
{
    public string playername;
    public bool isready;
    public Color ReadyColor { get { return isready? Color.green: Color.white; } }
    public ReadyState(string n, bool i)
    {
        playername = n;
        isready = i;
    }
}
[System.Serializable]
public class MouseRules
{
    static public bool scoreboard;
    static public bool escape;
    static public bool lobby;
    static public bool main;
    static public bool respawn;
    static private bool triggerd => scoreboard || escape || lobby || main || respawn;

    static public bool Focus { get { return Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false; } }
    static public void Update()
    {
        Cursor.visible = !triggerd ? false : true;
        Cursor.lockState = !triggerd ? CursorLockMode.Locked : CursorLockMode.None;
    }

    internal static void ActiveMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [Header("Connect")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField connectionField;
    
    [Header("Lobby")]
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Text timerLobby;

    public GameObject lPlayersFolder;
    public GameObject lPlayerDuplicate;

    public bool isClientReady;
    public Dictionary<ushort,ReadyState> readys = new Dictionary<ushort, ReadyState>();
    private void Awake()
    {
        Singleton = this;
    }
    public void GameStarting()
    {
        lobbyUI.SetActive(false);
        MouseRules.lobby = false;
        FindObjectOfType<GameLogic>().GameStarting();
    }
    public void DebugReadys()
    {
        MouseRules.lobby = true;
        foreach (Transform child in lPlayersFolder.transform)
            Destroy(child.gameObject);

        foreach (var r in readys.Values)
        {
            GameObject di = Instantiate(lPlayerDuplicate, lPlayersFolder.transform);
            di.SetActive(true);

            Text dtext = di.GetComponent<Text>();
            dtext.text = r.playername;
            dtext.color = r.ReadyColor;
        }
    }

    [MessageHandler((ushort)ServerToClientId.gameStarting)]
    public static void GameStarting(Message message)
    {
        Singleton.GameStarting();
    }
    public void AddReady(ushort _id, string _name, bool _ready)
    {
        readys.Add(_id, new ReadyState(_name, _ready));
        DebugReadys();
    }
    [MessageHandler((ushort)ServerToClientId.ready)]
    public static void Ready(Message message)
    {
        Singleton.UpdateReady(message.GetUShort(), message.GetBool());
    }
    [MessageHandler((ushort)ServerToClientId.clientJoined)]
    public static void ClientJoined(Message message)
    {
        Singleton.AddReady(message.GetUShort(), message.GetString(), message.GetBool());
    }

    public Text modeData;
    public void DisplayMode(int score, MatchMode gameMode, float time)
    {
        modeData.text = "Deathmatch: " + gameMode.ToString() + "\nScore: " + score.ToString() + "\nTime: " + time.ToString();
    }

    public void UpdateReady(ushort _id, bool _ready)
    {
        readys[_id].isready =  _ready;
        CheckReadys();
        DebugReadys();
    }
    private float timer;
    private bool cantimer;
    public void CheckReadys()
    {
        bool r = true;
        foreach (var p in readys)
        {
            if (!p.Value.isready)
                r = false;
        }

        timerLobby.text = "";
        if (r)
        {
            timer = 3;
            cantimer = true;
            timerLobby.text = "STARTING IN " + Mathf.CeilToInt(timer).ToString() + " SECONDS!";
        }
        else
        {
            timer = 0;
            cantimer = false;
            timerLobby.text = "";
        }
    }
    [MessageHandler((ushort)ServerToClientId.gameWin)]
    public static void GameWin(Message message)
    {
        Dictionary<int, string> p = new Dictionary<int, string>();
        p.Add(1, message.GetString());
        p.Add(2, message.GetString()); 
        p.Add(3, message.GetString());
        WinManager.Inisiate(p);
        SceneManager.LoadScene("WinScene"); // Wins
    }

    public void ReadyClicked()
    {
        isClientReady = !isClientReady;
        readys[NetworkManager.Singleton.Client.Id].isready = isClientReady; 
        DebugReadys();

        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.ready);
        message.AddBool(isClientReady);
        NetworkManager.Singleton.Client.Send(message);
    }
    public void ConnectClicked()
    {
        usernameField.interactable = false;
        connectionField.interactable = false;

        string connection = connectionField.text;

        string ip = connection.Split(':')[0];
        ushort port = ushort.Parse(connection.Split(':')[1]);

        NetworkManager.Singleton.Connect(ip, port);
    }

    public GameObject errorBox;
    public IEnumerator SetError(string errorMessage)
    {
        errorBox.SetActive(true);
        errorBox.GetComponentInChildren<Text>().text = errorMessage;
        yield return new WaitForSeconds(5);
        errorBox.SetActive(false);
    }
    public void BackToMain(string s = null)
    {
        errorBox.SetActive(false);
        if (!string.IsNullOrEmpty(s)) 
        {
            StartCoroutine(SetError(s));
            
        }
        usernameField.interactable = true;
        connectUI.SetActive(true);

        lobbyUI.SetActive(false);
        readys = new Dictionary<ushort, ReadyState>();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ErrorMessage(string error)
    {
        BackToMain(error);
    }

    public void SendName()
    {
        lobbyUI.SetActive(true);
        connectUI.SetActive(false);
        readys = new Dictionary<ushort, ReadyState>();
        DebugReadys();

        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.name);
        message.AddString( string.IsNullOrEmpty(usernameField.text) ? "Guest" + Random.Range(0, 100).ToString() : usernameField.text);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    private void Update()
    {
        if (connectUI.activeSelf)
            MouseRules.lobby = true;
        MouseRules.Update();
        if (cantimer) { 
            timer -= Time.deltaTime;
            timerLobby.text = "STARTING IN " + Mathf.CeilToInt(timer).ToString() + " SECONDS!";
        }
    }
}
