using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using System;

public class ChatMessage
{
    public ushort client;
    public string message;
    public string time;
    public ChatMessage(ushort client, string message, string time)
    {
        this.client = client;
        this.message = message;
        this.time = time;
    }
    public override string ToString()
    {
        return "[" + this.time + "] " + Player.list[this.client].username + ": " + this.message;
    }
}
public class ChatManager : MonoBehaviour
{
    public static Dictionary<uint, ChatMessage> list = new Dictionary<uint, ChatMessage>();
    private static ChatManager _singleton;
    public Transform folder;
    public GameObject duplicate;
    public static ChatManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ChatManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    void Awake()
    {
        Singleton = this;
    }
    public void sendMessage(string message)
    {
        Message m = Message.Create(MessageSendMode.reliable, ClientToServerId.chat);
        m.AddString(message);
        NetworkManager.Singleton.Client.Send(m);
        Debug.Log("sent");
    }
    [MessageHandler((ushort)ServerToClientId.chat)]
    public static void Chat(Message message)
    {
        uint id = message.GetUInt();
        ChatMessage m = new ChatMessage(message.GetUShort(), message.GetString(), message.GetString());
        list.Add(id, m);
        Singleton.GetMessage(id, m);
    }

    private void GetMessage(uint id, ChatMessage message)
    {
        var i = Instantiate(duplicate, folder);
        i.GetText().text = message.ToString();
        i.name = id.ToString();
        i.SetActive(true);
        Debug.Log("getonefromserv");
    }
}
