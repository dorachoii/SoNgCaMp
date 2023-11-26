using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChatManager : MonoBehaviour,IChatClientListener
{
    private ChatClient chatClient;
    public ChatService chatService;
    public string channel = "DEFAULT";

    public string[] names = new string[] { "Dora", "Messi", "Sally" };

    void Start()
    {
        
    }
    void Update()
    {
        if(chatClient != null)
            chatClient.Service();
    }

    public void ConnectToChatServer(string id) {
        ChatAppSettings chatset = new ChatAppSettings();
        chatClient = new ChatClient(this);
        chatset.AppIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        //chatClient.ConnectUsingSettings(chatset);
        chatClient.Connect(chatset.AppIdChat,chatset.AppVersion,new AuthenticationValues(id));
    }
    public void Send(string message)
    {
        chatClient.PublishMessage(channel, message);
    }

    public void SendPrivate(string target,string message) {
        chatClient.SendPrivateMessage(target,message);
    }

    public void ChangeChannell(string channel) {
        this.channel = channel;
    }

    public void SetChannel(string channel) {
        chatClient.Subscribe(channel);
    }

    public void OnChatMessage(string channelName, string sender, object message) {
        Debug.Log("message" + message);
    }
    public void OnConnected()
    {
        Debug.Log("Connect");
        //use Default Channel
        chatClient.Subscribe(channel);
        
        Debug.Log("My NickName " + chatClient.UserId);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            //string sender = senders[i];
            string sender = names[Random.Range(0, names.Length)];
            

            object messageObject = messages[i];

            // Message °´Ã¼¸¦ ¹®ÀÚ¿­·Î º¯È¯
            string messageString = messageObject.ToString();

            chatService.CreateChat(sender + " : " + messageString);
            
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnDisconnected()
    {
        Debug.Log("disconnect");
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("±Ó¼Ó¸»ÀÌ ¿Ô½¿." + sender + " : " + message);

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }
    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }

}
