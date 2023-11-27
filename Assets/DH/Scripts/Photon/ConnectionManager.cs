using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks
{

    public Action onConnected;
    public Action onJoinRoom;
    static ConnectionManager instance;
    public static ConnectionManager Get
    {
        get {
            if (instance != null)
                return instance;
            //������
            GameObject conn = new GameObject("ConnectionManager");
            DontDestroyOnLoad(conn);
            //�ν��Ͻ� �����ϱ�
            instance = conn.AddComponent<ConnectionManager>();
            return instance;
        
        }
    }
    public void ConnectToPhoton() { 
        PhotonNetwork.ConnectUsingSettings();
    }

    // ���� ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
    public override void OnConnectedToMaster()
    {
        CreateOrJoinRoom("DEFAULT_ROOM");
        Debug.Log("Connected to Photon Server");

        onConnected?.Invoke();
    }

    // ���� ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        onJoinRoom?.Invoke();
    }
    public void CreateOrJoinRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // �ִ� �÷��̾� �� ����
            IsVisible = true, // ���� ��Ͽ� ǥ�õǴ��� ���� ����
            IsOpen = true // ���� ���� �ִ��� ���� ����
        };

        // ���� �����ϸ� �����ϰ�, ������ ����
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Already in a Room");
        }
        else
        {
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        //�α��� ������ �̵�.
        //�ÿ����� �̷��� �ϸ� �����.
        //SceneController.StartLoadSceneAsync(this,false,2,null);
    }
}
