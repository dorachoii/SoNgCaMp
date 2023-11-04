using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReqManager : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ReqTest());

        Info info = new Info();
        info.age = 18;
        info.name = "�Ф�";

        Dictionary<string, Info> data = new Dictionary<string, Info>();
        data.Add("data1",info);

        string js = JsonUtility.ToJson(data);

        Dictionary<string, Info> data2 = JsonUtility.FromJson<Dictionary<string, Info>>(js);
        Debug.Log(data2.Count);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator ReqTest()
    {
        string url = "http://localhost:80/test1234/testInfo"; // URL�� "http://"�� �߰��ؾ� �մϴ�.

        // Info ��ü ���� �� JSON���� ��ȯ
        Info info = new Info();
        info.name = "Kid";
        info.age = 20;

        // Info ��ü�� JSON ���ڿ��� ����ȭ
        string jsonData = JsonUtility.ToJson(info);


        // UnityWebRequest ����
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            // ��û�� ���������� �Ϸ���� ���� ó��
            Debug.Log("Request successfully sent!");
            Response info2 = JsonUtility.FromJson<Response>(req.downloadHandler.text);

            Debug.Log(info2.data);
            //Debug.Log(info2.name);
        }
        else
        {
            // ��û ���� ���� ó��
            Debug.Log("Request failed: " + req.error);
        }
    }

    [System.Serializable]
    public struct Info {
        public string name;
        public int age;

    }
    [System.Serializable]
    public struct Response {
        public string httpState;
        public string message;
        public Dictionary<string, object> data;
    }
    //��û ��� �� ������
}
