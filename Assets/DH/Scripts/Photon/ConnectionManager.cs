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
        Debug.Log("Connected to Photon Server");
    }

    // ���� ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
    }


}
