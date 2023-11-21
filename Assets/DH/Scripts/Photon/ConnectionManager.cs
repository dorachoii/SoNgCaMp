using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    
    // Start is called before the first frame update
    void Start()
    {
        ConnectToPhoton();
    }

    // Update is called once per frame
    void Update()
    {
        
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
