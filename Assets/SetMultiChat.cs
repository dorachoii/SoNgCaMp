using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class SetMultiChat : MonoBehaviour
{
    public MultiChatManager multiChat;
    public string[] chatID = new string[] { "다람쥐" ,"너구리","염소" ,"코뿔소" };
    private void Awake()
    {
        //채팅을 시작하기 전에 ID 설정하기.
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/users/byToken")
            .Data(TokenManager.Token)
            .Success((down) => {
                //자기 캐릭터 생성하기..
                ResponseDTO<LoginDTO2> responce = JsonUtility.FromJson<ResponseDTO<LoginDTO2>>(down.text);
                multiChat.ConnectToChatServer(responce.results.authority.userNickname);
            })
            .Failure((down) => {
                //유저 정보가 없다? 그렇담.. 
                multiChat.ConnectToChatServer(chatID[Random.Range(0,chatID.Length)]);
            })
            .build();
        StartCoroutine(SendRequest(rq));
    }

}
