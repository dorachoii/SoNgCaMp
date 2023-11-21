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
        Checking();
        HashMap<string, string> hash = new HashMap<string, string>();
        hash.Add("name", "김도현");
        hash.Add("age", "12");
        TestHash<string> test = new TestHash<string>("안녕","하세","du");
        test.list.Add("DD");
        test.list.Add("SS");
        string s = JsonUtility.ToJson(hash);
        Debug.Log(s);
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
        Debug.Log("토큰 있음.." + download.text);
    }
    public void GoBack(DownloadHandler download) {
        Debug.Log("토큰 없음..");
    }
}
