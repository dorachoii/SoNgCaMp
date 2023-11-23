using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

#region �������
public enum RequestType
{
    GET,
    POST,
    PUT,
    DELETE,
    TEXTURE
}

//  httpInfo: ���� ����
public class HttpInfo
{
    //Variables: �Ű������� ���� ��
    public RequestType requestType;
    public string url = "";
    public string body;
    public Action<DownloadHandler> onReceive;

    //Set �Լ�: 1) ��û Ÿ�� 2) url 3) return�� ���� Action 4) default url ���� �� ����
    public void Set(

        //1) ��û Ÿ��
        RequestType type,
        //2) url
        string u,
        //3) return�� ���� Action
        Action<DownloadHandler> callback,
        //4) default url ���� �� ����
        bool useDefaultUrl = true)
    {

        requestType = type;
        
        if (useDefaultUrl) url = "http://192.168.0.45:8080/";

        //  default url�� ���ٸ�, �ٲ�� �κи� �߰� �Է�
        url += u;

        //  download ���� �� print�� �Լ��� ���� Action
        onReceive = callback;
    }
}
#endregion

#region ȸ������
//  01. ȸ������
[System.Serializable]
public struct UserInfo_register
{
    public string userEmail;        
    public string userPwd;          // ���� + ���� + Ư������ 7�ڸ� �̻�

    public string userNickname;
    public int sessionType;         //  0: None, 1~n: Instruments
    public int genre;               //  0: None, 1~n: Genre
    public int mood;                //  0: None, 1~n: Mood
    public bool musician;           

    // new ���� ��, �Ű����� �޾Ƽ� �ٷ� �������ִ� ����
    public UserInfo_register(string userEmail, string userPwd, string userNickname, int sessionType, int genre, int mood, bool musician)
    {
        this.userEmail = userEmail;
        this.userPwd = userPwd;

        this.userNickname = userNickname;
        this.sessionType = sessionType;
        this.genre = genre;
        this.mood = mood;
        this.musician = musician;
    }
}

public struct UserInfo_login
{
    public string userEmail;
    public string userPwd;

    public UserInfo_login(string userEmail, string userPwd)
    {
        this.userEmail = userEmail;
        this.userPwd = userPwd;
    }
}

public struct UserInfo_customizing
{
    public int characterType;

    // Colors
    public string hexStringCloth;
    public string hexStringFace;
    public string hexStringRibbon;
    public string hexStringSkin;

    //Items
    public bool isBagOn;
    public bool isCapOn;
    public bool isCrownOn;
    public bool isGlassOn;

    //ȸ�� ��ȣ
    public int userNo;


    // new ���� ��, �Ű����� �޾Ƽ� �ٷ� �������ִ� ����
    public UserInfo_customizing( int characterType, string hexStringCloth, string hexStringFace, string hexStringRibbon, string hexStringSkin, bool isBagOn, bool isCapOn, bool isCrownOn, bool isGlassOn, int userNo)
    {
        this.characterType = characterType;

        this.hexStringCloth = hexStringCloth;
        this.hexStringFace = hexStringFace;
        this.hexStringRibbon = hexStringRibbon;
        this.hexStringSkin = hexStringSkin;

        this.isBagOn = isBagOn;
        this.isCapOn = isCapOn;
        this.isCrownOn = isCrownOn;
        this.isGlassOn = isGlassOn;

        this.userNo = userNo;
               
    }
}

#endregion

#region ������
//  02. �� ����
public struct SongInfo
{
    public string songTitle;
    //���� �߰�
}
#endregion


public class HttpManager : MonoBehaviour
{
    static HttpManager instance;

    private void Start()
    {
        //***�� �κ��� input���� �޾ƿͼ� �ִ� ��� ���
        string id = "hellodoradora7@gmail.com";
        string pw = "1234abcd!";
        string nickName = "�ź���";
        int sessionType = 0;
        int genre = 0;
        int mood = 0;
        bool musician = true;

        UserInfo_register userInfo = new UserInfo_register(id, pw, nickName, sessionType, genre, mood, musician);

        //"api/v1/users"
        HttpInfo httpInfo = new HttpInfo();

        httpInfo.Set(RequestType.POST, /*"api/v1/authentication/login"*/"api/v1/users", (DownloadHandler downHandler) => {
            print(downHandler.text);
            //���������� ��û�޾��� �� ���� ����Ʈ
        }, true);

        //httpInfo.body = JsonUtility.ToJson(userInfo);
        httpInfo.body = "{\r\n  \"userEmail\": \"hellodoradora7@gmail.com\",\r\n  \"userPwd\": \"1234abcd!\"\r\n}";
        //HttpManager.Get().SendRequest(httpInfo);
    }

    //register���� ������ ���� �ǰԲ�??
    public UserInfo_register Resister(string id, string pw, string nickname, int sessionType, int genre, int mood, bool musician)
    {
        UserInfo_register userInfo = new UserInfo_register(id, pw, nickname, sessionType, genre, mood, musician);
        return (userInfo);
    }


    //Get �Լ�: 
    public static HttpManager Get()
    {
        //Hierarchyâ�� httpManager�� �ϳ��� �� �پ��ִٸ� ����� ���� ������ִ� ��
        if (instance == null)
        {
            GameObject go = new GameObject("HttpManager");
            go.AddComponent<HttpManager>();
        }
        return instance;
    }

    private void Awake()
    {
        //���� ���� ������ httpManager ���� �ϳ����� ����α� ���� ��
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

    //�������� REST API ��û (GET, POST, PUT, DELETE)
    IEnumerator CoSendRequest(HttpInfo httpInfo)
    {
        if (httpInfo.requestType == RequestType.POST)
        {
            //  Memory leak ����
            //  �Ű�����: ���ҽ� ���� ����
            using (UnityWebRequest req = UnityWebRequest.Post(httpInfo.url, httpInfo.body))
            {
                //body�� UTF8�� ���ڵ����ش�.
                byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
                //upload�� �Ѵ�.
                req.uploadHandler = new UploadHandlerRaw(byteBody);
                //header������ �Ѵ�.
                req.SetRequestHeader("Content-Type", "application/json");

                //��û ������ �� ���� ��� �纸
                yield return req.SendWebRequest();

                //�����ߴٸ� ���� print, �����ߴٸ� error ���� �Լ� �������.
                RequestResult(req, httpInfo);

                //���� �� �ڵ����� dispose ó�� ���ش�.
            }
        }
        else
        {
            // �޸� �ʱ�ȭ?
            UnityWebRequest req = null;

            // GET, PUT, DELETE, TEXTURE �б�
            switch (httpInfo.requestType)
            {
                case RequestType.GET:                    
                    req = UnityWebRequest.Get(httpInfo.url);
                    break;

                case RequestType.PUT:       // ����
                    req = UnityWebRequest.Put(httpInfo.url, httpInfo.body);
                    break;

                case RequestType.DELETE:
                    req = UnityWebRequest.Delete(httpInfo.url);
                    break;

                case RequestType.TEXTURE:
                    req = UnityWebRequestTexture.GetTexture(httpInfo.url);
                    break;                    
            }

            //������ ��û�� ������ ������ �ö����� �纸�Ѵ�.
            yield return req.SendWebRequest();

            RequestResult(req, httpInfo);
        }
    }

    //��û ���� �� �����ų �Լ�
    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    void RequestResult(UnityWebRequest req, HttpInfo httpInfo)
    {
        //���࿡ ������ �����ߴٸ�
        if (req.result == UnityWebRequest.Result.Success)
        {
            //print("��Ʈ��ũ ���� : " + req.downloadHandler.text);
            //onReceive�� ���� ���� = ������ ���� ������ �޾Ҵ�.
            if (httpInfo.onReceive != null)
            {
                //�Ű������� DownloadHandler
                httpInfo.onReceive(req.downloadHandler);

                print(req.downloadHandler);
            }
        }
        //��� ����
        else
        {
            print("��Ʈ��ũ ���� : " + req.error);
        }
    }
}