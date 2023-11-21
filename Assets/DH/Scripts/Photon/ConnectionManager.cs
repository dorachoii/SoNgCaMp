using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
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
        Debug.Log("Connected to Photon Server");
    }

    // 게임 서버에 연결되었을 때 호출되는 메서드
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
    }


}
