using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class SetMultiChat : MonoBehaviour
{
    public MultiChatManager multiChat;
    public string[] chatID = new string[] { "�ٶ���" ,"�ʱ���","����" ,"�ڻԼ�" };
    private void Awake()
    {
        //ä���� �����ϱ� ���� ID �����ϱ�.
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/users/byToken")
            .Data(TokenManager.Token)
            .Success((down) => {
                //�ڱ� ĳ���� �����ϱ�..
                ResponseDTO<LoginDTO2> responce = JsonUtility.FromJson<ResponseDTO<LoginDTO2>>(down.text);
                multiChat.ConnectToChatServer(responce.results.authority.userNickname);
            })
            .Failure((down) => {
                //���� ������ ����? �׷���.. 
                multiChat.ConnectToChatServer(chatID[Random.Range(0,chatID.Length)]);
            })
            .build();
        StartCoroutine(SendRequest(rq));
    }

}
