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
            //없을시
            GameObject conn = new GameObject("ConnectionManager");
            DontDestroyOnLoad(conn);
            //인스턴스 생성하기
            instance = conn.AddComponent<ConnectionManager>();
            return instance;
        
        }
    }
    public void ConnectToPhoton() { 
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 연결되었을 때 호출되는 메서드
    public override void OnConnectedToMaster()
    {
        CreateOrJoinRoom("DEFAULT_ROOM");
        Debug.Log("Connected to Photon Server");

        onConnected?.Invoke();
    }

    // 게임 서버에 연결되었을 때 호출되는 메서드
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        onJoinRoom?.Invoke();
    }
    public void CreateOrJoinRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // 최대 플레이어 수 설정
            IsVisible = true, // 룸이 목록에 표시되는지 여부 설정
            IsOpen = true // 룸이 열려 있는지 여부 설정
        };

        // 룸이 존재하면 참여하고, 없으면 생성
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
        //로그인 씬으로 이동.
        //시연떄는 이렇게 하면 곤란함.
        //SceneController.StartLoadSceneAsync(this,false,2,null);
    }
}
