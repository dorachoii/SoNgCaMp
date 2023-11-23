using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

#region 통신정보
public enum RequestType
{
    GET,
    POST,
    PUT,
    DELETE,
    TEXTURE
}

//  httpInfo: 서버 정보
public class HttpInfo
{
    //Variables: 매개변수로 받을 값
    public RequestType requestType;
    public string url = "";
    public string body;
    public Action<DownloadHandler> onReceive;

    //Set 함수: 1) 요청 타입 2) url 3) return값 받을 Action 4) default url 쓸지 안 쓸지
    public void Set(

        //1) 요청 타입
        RequestType type,
        //2) url
        string u,
        //3) return값 받을 Action
        Action<DownloadHandler> callback,
        //4) default url 쓸지 안 쓸지
        bool useDefaultUrl = true)
    {

        requestType = type;
        
        if (useDefaultUrl) url = "http://192.168.0.45:8080/";

        //  default url을 쓴다면, 바뀌는 부분만 추가 입력
        url += u;

        //  download 받은 값 print할 함수를 담을 Action
        onReceive = callback;
    }
}
#endregion

#region 회원정보
//  01. 회원정보
[System.Serializable]
public struct UserInfo_register
{
    public string userEmail;        
    public string userPwd;          // 영문 + 숫자 + 특수문자 7자리 이상

    public string userNickname;
    public int sessionType;         //  0: None, 1~n: Instruments
    public int genre;               //  0: None, 1~n: Genre
    public int mood;                //  0: None, 1~n: Mood
    public bool musician;           

    // new 생성 시, 매개변수 받아서 바로 세팅해주는 역할
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

    //회원 번호
    public int userNo;


    // new 생성 시, 매개변수 받아서 바로 세팅해주는 역할
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

#region 곡정보
//  02. 곡 정보
public struct SongInfo
{
    public string songTitle;
    //이후 추가
}
#endregion


public class HttpManager : MonoBehaviour
{
    static HttpManager instance;

    private void Start()
    {
        //***이 부분을 input값을 받아와서 넣는 방법 고안
        string id = "hellodoradora7@gmail.com";
        string pw = "1234abcd!";
        string nickName = "거북이";
        int sessionType = 0;
        int genre = 0;
        int mood = 0;
        bool musician = true;

        UserInfo_register userInfo = new UserInfo_register(id, pw, nickName, sessionType, genre, mood, musician);

        //"api/v1/users"
        HttpInfo httpInfo = new HttpInfo();

        httpInfo.Set(RequestType.POST, /*"api/v1/authentication/login"*/"api/v1/users", (DownloadHandler downHandler) => {
            print(downHandler.text);
            //정상적으로 요청받았을 때 값을 프린트
        }, true);

        //httpInfo.body = JsonUtility.ToJson(userInfo);
        httpInfo.body = "{\r\n  \"userEmail\": \"hellodoradora7@gmail.com\",\r\n  \"userPwd\": \"1234abcd!\"\r\n}";
        //HttpManager.Get().SendRequest(httpInfo);
    }

    //register값에 넣으면 셋팅 되게끔??
    public UserInfo_register Resister(string id, string pw, string nickname, int sessionType, int genre, int mood, bool musician)
    {
        UserInfo_register userInfo = new UserInfo_register(id, pw, nickname, sessionType, genre, mood, musician);
        return (userInfo);
    }


    //Get 함수: 
    public static HttpManager Get()
    {
        //Hierarchy창에 httpManager가 하나도 안 붙어있다면 통신을 위해 만들어주는 것
        if (instance == null)
        {
            GameObject go = new GameObject("HttpManager");
            go.AddComponent<HttpManager>();
        }
        return instance;
    }

    private void Awake()
    {
        //가장 먼저 생성된 httpManager 오직 하나만을 살려두기 위한 것
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

    //서버에게 REST API 요청 (GET, POST, PUT, DELETE)
    IEnumerator CoSendRequest(HttpInfo httpInfo)
    {
        if (httpInfo.requestType == RequestType.POST)
        {
            //  Memory leak 방지
            //  매개변수: 리소스 변수 선언
            using (UnityWebRequest req = UnityWebRequest.Post(httpInfo.url, httpInfo.body))
            {
                //body를 UTF8로 인코딩해준다.
                byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
                //upload를 한다.
                req.uploadHandler = new UploadHandlerRaw(byteBody);
                //header설정을 한다.
                req.SetRequestHeader("Content-Type", "application/json");

                //요청 성공할 때 까지 잠깐 양보
                yield return req.SendWebRequest();

                //성공했다면 내용 print, 실패했다면 error 띄우는 함수 만들었다.
                RequestResult(req, httpInfo);

                //실행 후 자동으로 dispose 처리 해준다.
            }
        }
        else
        {
            // 메모리 초기화?
            UnityWebRequest req = null;

            // GET, PUT, DELETE, TEXTURE 분기
            switch (httpInfo.requestType)
            {
                case RequestType.GET:                    
                    req = UnityWebRequest.Get(httpInfo.url);
                    break;

                case RequestType.PUT:       // 수정
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

            RequestResult(req, httpInfo);
        }
    }

    //요청 보낼 때 실행시킬 함수
    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    void RequestResult(UnityWebRequest req, HttpInfo httpInfo)
    {
        //만약에 응답이 성공했다면
        if (req.result == UnityWebRequest.Result.Success)
        {
            //print("네트워크 응답 : " + req.downloadHandler.text);
            //onReceive에 무언가 담겼다 = 서버로 부터 정보를 받았다.
            if (httpInfo.onReceive != null)
            {
                //매개변수로 DownloadHandler
                httpInfo.onReceive(req.downloadHandler);

                print(req.downloadHandler);
            }
        }
        //통신 실패
        else
        {
            print("네트워크 에러 : " + req.error);
        }
    }
}