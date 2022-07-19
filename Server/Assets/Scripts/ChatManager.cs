using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class DTime
{
    public ushort hour, minute, second;
    public DTime(ushort hour, ushort minute, ushort second)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }
    public override string ToString()
    {
        return this.hour.ToString("00") + ":" + this.minute.ToString("00") + ":" + this.second.ToString("00");
    }

    public static DTime Get(System.DateTime now)
    {
        return new DTime((ushort)now.Hour, (ushort)now.Minute, (ushort)now.Second);
    }
    public static string ToString(System.DateTime now)
    {
        return now.Hour.ToString("00") + ":" +  now.Minute.ToString("00") + ":" + now.Second.ToString("00");
    }
}
public class ChatMessage
{
    public ushort client;
    public string message;
    public DTime time;
    public ChatMessage(ushort client, string message, DTime time)
    {
        this.client = client;
        this.message = message;
        this.time = time;
    }
}
public class ChatManager : MonoBehaviour
{
    public static Dictionary<uint, ChatMessage> list = new Dictionary<uint, ChatMessage>();
    static uint nextId = 0;

    [MessageHandler((ushort)ClientToServerId.chat)]
    public static void Chat(ushort fromClientId, Message message)
    {
        GetMessage(fromClientId, message.GetString());
    }
    public static void GetMessage(ushort client, string message)
    {
        nextId++;
        ChatMessage m = new ChatMessage(client, message, DTime.Get(System.DateTime.Now));
        
        list.Add(nextId, m);
        SendMessage(nextId,m);
    }
    private static void SendMessage(uint id, ChatMessage m)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.chat);
        message.AddUInt(id);
        message.AddUShort(m.client);
        message.AddString(m.message);
        message.AddString(m.time.ToString());
        UIManager.Log("[" + m.time.ToString() + "] (" + Player.list[m.client].Username + "): " + m.message);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

}
