using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

//https://jsonplaceholder.typicode.com

[Serializable]
public class JsonList<T>
{
    public List<T> data;
}

[Serializable]
public struct CommentInfo
{
    public int postId;
    public int id;
    public string name;
    public string email;
    public string body;
}

[Serializable]
public struct SignUpInfo
{
    public string userName;
    public string birthday;
    public int age;
}



public enum RequestType
{
    GET,
    POST,
    PUT,
    DELETE,
    TEXTURE
}

//웹 통신하기 위한 정보
public class HttpInfo
{
    public RequestType requestType;
    public string url = "";
    public string body;
    public Action<DownloadHandler> onReceive;

    public void Set(
        RequestType type,
        string u,
        Action<DownloadHandler> callback,
        bool useDefaultUrl = true)
    {
        requestType = type;
        if (useDefaultUrl) url = "https://jsonplaceholder.typicode.com";
        url += u;
        onReceive = callback;
    }
}


public class HttpManager : MonoBehaviour
{
    static HttpManager instance;

    public static HttpManager Get()
    {
        if (instance == null)
        {
            //게임 오브젝트 만든다
            GameObject go = new GameObject("HttpStudy");
            //만들어진 게임 오브젝트에 HttpManager 컴포넌트 붙이자
            go.AddComponent<HttpManager>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    //서버에게 REST API 요청 (GET, POST, PUT, DELETE)
    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    IEnumerator CoSendRequest(HttpInfo httpInfo)
    {
        UnityWebRequest req = null;

        //POST, GET, PUT, DELETE 분기
        switch (httpInfo.requestType)
        {
            case RequestType.GET:
                //Get방식으로 req 에 정보 셋팅
                req = UnityWebRequest.Get(httpInfo.url);
                break;
            case RequestType.POST:
                req = UnityWebRequest.Post(httpInfo.url, httpInfo.body);
                byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
                req.uploadHandler = new UploadHandlerRaw(byteBody);
                //헤더 추가
                req.SetRequestHeader("Content-Type", "application/json");

                break;
            case RequestType.PUT:
                req = UnityWebRequest.Put(httpInfo.url, httpInfo.body);
                break;
            case RequestType.DELETE:
                req = UnityWebRequest.Delete(httpInfo.url);
                break;
            case RequestType.TEXTURE:
                req = UnityWebRequestTexture.GetTexture(httpInfo.url);
                break;
        }

        //서버에 요청을 보내고 응답이 올때까지 양보한다.
        yield return req.SendWebRequest();

        //만약에 응답이 성공했다면
        if (req.result == UnityWebRequest.Result.Success)
        {
            //print("네트워크 응답 : " + req.downloadHandler.text);

            if (httpInfo.onReceive != null)
            {
                httpInfo.onReceive(req.downloadHandler);
            }
        }
        //통신 실패
        else
        {
            print("네트워크 에레 : " + req.error);
        }
    }
}