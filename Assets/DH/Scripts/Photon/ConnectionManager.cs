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

    // ���� ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server");
    }

    // ���� ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
    }


}
