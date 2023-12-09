using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerChatManager : MonoBehaviourPun
{
    private void Start()
    {
        //LoungeChat.instance.onSendMessage += (s) => { photonView.RPC(nameof(SendChat), RpcTarget.All, s); };
    }

    //채팅 창.
    [SerializeField]
    Transform board;

    [SerializeField]
    TMP_InputField inputField;

    //챗 생성 
    [SerializeField] GameObject TextField;
    [SerializeField] int maxChatCount = 3;

    [PunRPC]
    public void SendChat(string s) {
        if (board.childCount > maxChatCount)
        {
            Destroy(board.GetChild(0).gameObject);
        }
        //inputField.text
        GameObject go = Instantiate(TextField,board);
        ChatText txt = go.GetComponent<ChatText>();
        txt.SetText(s);

    }

    
    public void Send() {
        photonView.RPC(nameof(SendChat),RpcTarget.All,inputField.text);
        inputField.text = "";
    }
}
