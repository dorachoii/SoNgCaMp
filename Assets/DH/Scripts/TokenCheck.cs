using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static HttpController;

public class TokenCheck : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        HttpRequest rq = new HttpBuilder().Uri("/test1234/test1234")
            .Type(ReqType.GET)
            .Success((download)=> {
                Debug.Log(download.text);
                ResponceDTO<Info2> dto = JsonUtility.FromJson<ResponceDTO<Info2>>(download.text);
                string s = dto.results.toString();
                Debug.Log(s);
            })
            .build();
            
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //토큰 체킹
    public void Checking() {
        string token = TokenManager.Token;
        //요청하기.
        HttpRequest rq = new HttpBuilder().Uri("/api/v1/users/byToken")
            .Data(token)
            .Success((download)=> {
                Access(download);
            })
            .Failure((download)=> {
                GoBack(download);
            })
            .build();
        StartCoroutine(SendRequest(rq));
    }

    public void Access(DownloadHandler download) {
        //토큰 있음.. 입장 가능...
        ResponceDTO<LoginResponseDTO> responce = JsonUtility.FromJson<ResponceDTO<LoginResponseDTO>>(download.text);
        Debug.Log(responce);
    }
    public void GoBack(DownloadHandler download) {
        Debug.Log("토큰 없음..");
    }
}
