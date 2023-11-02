using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class UserInfo
{
    public bool isMusician;
    public string name;
    public int session;
    public int genre;
    public int mood;
}

public enum sessionType
{
    Piano,
    Guitar,
    Base,
    Drum,
    etc
}

public enum genre
{
    //bandcamp genre + artist genre (23)
    Electronic,
    Rock,
    Metal,
    Alternative,
    Hiphop,
    Experimental,
    Punk,
    Folk,
    Pop,
    Ambient,
    Jazz,
    Acoustic,
    Funk,
    RnB_soul,
    Classical,
    Raggae,
    Country,
    Blues,
    Latin,
    Cinematic,
    Indie,
    LofiNChill,
    Lounge,
    etc
}

public enum mood
{
    Uplifting,
    Exciting,
    Happy,
    Funny,
    Hopeful,
    Groovy,
    Sexy,
    Peaceful,
    Mysterious,
    Serious,
    Tense,
    Sad,
    Dark,
    Energetic,
    Frantic,
    Depression,
    Calm,
}

public enum EJRequestType
{
    GET,
    POST,
    PUT,
    DELETE,
    TEXTURE
}

public class EJHttpInfo
{
    public EJRequestType EJRequestType2;
    public string url = "";
    public string body;

    //API로부터 다운로드를 받은 것이 있는지 없는지 판단
    public Action<DownloadHandler> onReceive;

    //??? 이 Set함수를 실행시켜주는 곳이 어디지?
    public void Set(
            EJRequestType type,
            string u,
            Action<DownloadHandler> callback,
            bool useDefaultUrl = true)
    {
        EJRequestType2 = type;
        if (useDefaultUrl) url = "http://172.16.16.228:3000";

        url += u;
        onReceive = callback;
    }
}

public class EJHttpManager : MonoBehaviour
{
    static EJHttpManager instance;

    public static EJHttpManager Get()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("EJlog");
            go.AddComponent<EJHttpManager>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoSendRequest(EJHttpInfo httpInfo)
    {
        UnityWebRequest req = null;

        switch (httpInfo.EJRequestType2)
        {
            case EJRequestType.GET:
                req = UnityWebRequest.Post(httpInfo.url, httpInfo.body);
                byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
                req.uploadHandler = new UploadHandlerRaw(byteBody);
                break;

            case EJRequestType.PUT:
                req = UnityWebRequest.Put(httpInfo.url, httpInfo.body);
                break;

            case EJRequestType.DELETE:
                req = UnityWebRequest.Delete(httpInfo.url);
                break;

            //이미지 파일
            case EJRequestType.TEXTURE:
                req = UnityWebRequestTexture.GetTexture(httpInfo.url);
                break;
        }

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            if (httpInfo.onReceive != null)
            {
                httpInfo.onReceive(req.downloadHandler);
            }
        }else
        {
            print("네트워크 에러: " + req.error);
        }
    }
}
