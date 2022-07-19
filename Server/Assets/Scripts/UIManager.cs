using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class UILogMessage
{
    public string message;
    public Color color;

    public UILogMessage(string m, Color c)
    {
        this.message = m;
        this.color = c;
    }
}
public class UIManager : MonoBehaviour
{
    private static UIManager S => Singleton;
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
    public bool startAutomatic;
    [Header("Start")]
    public GameObject startPannel;
    public InputField countInp;
    public InputField portInp;
    public InputField timeInp;
    public InputField scoreInp;

    [Header("Server")]
    public GameObject serverPannel;
    public GameObject loggFolder;
    public GameObject loggDuplicate;


    private bool isrunning;
    private bool needUdate;
    private List<UILogMessage> loggs = new List<UILogMessage>();


    public void Awake()
    {
        Singleton = this;
        if (startAutomatic)
        {
            StartServer();
        }
    }
    void Update()
    {
        isrunning = NetworkManager.Singleton.isrunning;
        if (!isrunning)
        {
            StopServer(false);
        }

        if (needUdate)
        {
            foreach (Transform child in loggFolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in loggs)
            {
                var duplicated = Instantiate(loggDuplicate, loggFolder.transform);
                duplicated.SetActive(true);

                var d = duplicated.GetComponent<Text>();
                d.text = item.message;
                d.color = item.color;
            }
            needUdate = false;
        }
    }
    public static void Log(string message)
    {
        S.Log(message, Color.black);
    }
    public void Log(string message, Color color)
    {
        loggs.Add(new UILogMessage(message, color));
        needUdate = true;
    }
    public static void LogWarning(string message)
    {
        S.Log(message, Color.yellow);
    }
    public static void LogError(string message)
    {
        S.Log(message, Color.red);
    }

    public void StartServer()
    {
        startPannel.SetActive(false);
        serverPannel.SetActive(true);

        ushort count = ushort.Parse(countInp.text);
        ushort port = ushort.Parse(portInp.text);

        portInp.interactable = false;
        countInp.interactable = false;

        isrunning = NetworkManager.Singleton.StartServer(port, count);
        if (!startAutomatic)
            GameLogic.Singleton.Initialise(MatchMode.deathmatch , float.Parse(timeInp.text) * 60, int.Parse(scoreInp.text));
        else
            GameLogic.Singleton.Initialise(MatchMode.deathmatch, 60*60, 30);

    }
    public void StopServer(bool serverstop = true)
    {
        startPannel.SetActive(true);
        serverPannel.SetActive(false);

        portInp.interactable = true;
        countInp.interactable = true;

        if (serverstop)
            isrunning = NetworkManager.Singleton.StopServer();

        foreach(Transform child in loggFolder.transform) {
            Destroy(child.gameObject);
        }

        Application.Quit();
    }

    //Server Player List:
    public Transform lParent;
    public GameObject lDuplicate;
    public Dictionary<ushort, GameObject> lList = new Dictionary<ushort, GameObject>();

    public static void ListUpdate() => Singleton.lUpdate();
    public void lUpdate()
    {
        List<ushort> ids = new List<ushort>();
        if (GameLogic.Singleton.started)  
            foreach (var item in Player.list)
            {
                ids.Add(item.Key);
                if (!lList.ContainsKey(item.Key))
                    lAdd(item.Value.name, item.Key);
            }
        else foreach (var item in Player.waiting)
            {
                ids.Add(item.Key);
                if (!lList.ContainsKey(item.Key))
                    lAdd(item.Value.name, item.Key);
            }
        foreach (var item in ids)
        {
            if (!lList.ContainsKey(item))
            {
                lRemove(item);
            }
        }
        
    }
    private void lAdd(string name, ushort id)
    {
        Debug.Log(name);
        GameObject oby = Instantiate(lDuplicate, lParent);
        oby.GetComponentInChildren<Text>().text = name;
        lList.Add(id, oby);
        oby.GetComponentInChildren<Button>().onClick.AddListener(delegate { NetworkManager.Singleton.Kick(id); });
        oby.SetActive(true);
    }
    private void lRemove(ushort id)
    {
        Destroy(lList[id]);
        lList.Remove(id);
    }

    //Server Stop Function:
    public void StopServer()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {

    }

}
