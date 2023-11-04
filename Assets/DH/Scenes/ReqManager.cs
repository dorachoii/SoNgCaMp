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
        info.name = "ㅠㅠ";

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
        string url = "http://localhost:80/test1234/testInfo"; // URL에 "http://"를 추가해야 합니다.

        // Info 객체 생성 및 JSON으로 변환
        Info info = new Info();
        info.name = "Kid";
        info.age = 20;

        // Info 객체를 JSON 문자열로 직렬화
        string jsonData = JsonUtility.ToJson(info);


        // UnityWebRequest 생성
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            // 요청이 성공적으로 완료됐을 때의 처리
            Debug.Log("Request successfully sent!");
            Response info2 = JsonUtility.FromJson<Response>(req.downloadHandler.text);

            Debug.Log(info2.data);
            //Debug.Log(info2.name);
        }
        else
        {
            // 요청 실패 시의 처리
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
    //요청 결과 및 데이터
}
