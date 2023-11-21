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
        hash.Add("name", "�赵��");
        hash.Add("age", "12");
        TestHash<string> test = new TestHash<string>("�ȳ�","�ϼ�","du");
        test.list.Add("DD");
        test.list.Add("SS");
        string s = JsonUtility.ToJson(hash);
        Debug.Log(s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //��ū üŷ
    public void Checking() {
        string token = TokenManager.Token;
        //��û�ϱ�.
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
        //��ū ����.. ���� ����...
        Debug.Log("��ū ����.." + download.text);
    }
    public void GoBack(DownloadHandler download) {
        Debug.Log("��ū ����..");
    }
}
