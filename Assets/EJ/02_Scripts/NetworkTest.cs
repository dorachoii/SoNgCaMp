using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkTest: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(UnityWebRequestGET());
        //StartCoroutine(UnityWebRequestGET_texture());
        //StartCoroutine(UnityWebRequestPOST());

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
        
        httpInfo.Set(RequestType.POST, "api/v1/authentication/login", (DownloadHandler downHandler) => {
            print(downHandler.text);
            //정상적으로 요청받았을 때 값을 프린트
        }, true);
        //httpInfo.body = JsonUtility.ToJson(userInfo);
        httpInfo.body = "{\r\n  \"userEmail\": \"hellodoradora7@gmail.com\",\r\n  \"userPwd\": \"1234abcd!\"\r\n}";
        HttpManager.Get().SendRequest(httpInfo);


        //이미지와, 곡

    }

    #region 은정코드
    ////01. GET: API에 저장되어 있는 정보를 받아오는 것
    //IEnumerator UnityWebRequestGET()
    //{
        
    //    string apikey = "발급받은 API키를 넣는다.";
    //    //string url = "http://api.neople.co.kr/df/servers?apikey=" + apikey;
    //    //string url = "GET 통신을 할 API 주소" + apikey;
    //    string url = "https://en29jxisoaoti.x.pipedream.net";

    //    UnityWebRequest www = UnityWebRequest.Get(url);

    //    yield return www.SendWebRequest();

    //    if (www.error == null)
    //    {
    //        Debug.Log(www.downloadHandler.text);
    //    }else
    //    {
    //        Debug.Log("GET ERROR");
    //    }
    //}

    //// GET: 이미지 다운로드
    //IEnumerator UnityWebRequestGET_texture()
    //{
    //    string url = "GET 통신을 할 API 주소";
    //    UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

    //    yield return www.SendWebRequest();

    //    if (www.error == null) 
    //    {
    //        Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //    }
    //    else
    //    {
    //        Debug.Log("GET_texture ERROR");
    //    }
    //}

    

    ////IEnumerator UnityWebRequestPOSTXX()
    ////{
    ////    //string url = "POST 통신을 사용할 서버 주소를 입력";
    ////    string url = "http://192.168.0.24:8080/api/v1/users";
    ////    WWWForm form = new WWWForm();


    ////    string id = "hellodoradora7@gmail.com";
    ////    string pw = "1234abcd!";

    ////    string nickName = "거북이";

    ////    int sessionType = 0;
    ////    int genre = 0;
    ////    int mood = 0;
    ////    bool musician = true;
        
    ////    form.AddField("userEmail", id);
    ////    form.AddField ("userPwd", pw);

    ////    form.AddField("userNickname", nickName);
    ////    form.AddField("sessionType", sessionType);
    ////    form.AddField("genre", genre);
    ////    form.AddField("mood", mood);
    ////    form.AddField("musician", musician.ToString());
                
    ////    UnityWebRequest www = UnityWebRequest.Post(url, form);

    ////    www.SetRequestHeader("Content-Type", "application/json");

    ////    yield return www.SendWebRequest();

    ////    if (www.error == null)
    ////    {
    ////        Debug.Log(www.downloadHandler.text);
    ////    }else
    ////    {
    ////        Debug.Log("POST ERROR");
    ////        Debug.Log(www.error);
    ////    }
    ////}

    //IEnumerator UnityWebRequestPOST()
    //{
    //    //string url = "POST 통신을 사용할 서버 주소를 입력";
    //    //string url = "http://192.168.0.24:8080/api/v1/users";
    //    string url = "https://en29jxisoaoti.x.pipedream.net";
    //    WWWForm form = new WWWForm();


    //    string id = "hellodoradora7@gmail.com";
    //    string pw = "1234abcd!";
    //    string nickName = "거북이";
    //    int sessionType = 0;
    //    int genre = 0;
    //    int mood = 0;
    //    bool musician = true;

    //    UserInfo userInfo= new UserInfo(id,pw,nickName,sessionType,genre,mood,musician); 


    //    string userInfoJSON = JsonUtility.ToJson(userInfo);
    //    byte[] byteBody = Encoding.UTF8.GetBytes(userInfoJSON);

    //    UnityWebRequest www = UnityWebRequest.Post(url, byteBody.ToString());

    //    www.SetRequestHeader("Content-Type", "application/json");

    //    yield return www.SendWebRequest();

    //    if (www.error == null)
    //    {
    //        Debug.Log(www.downloadHandler.text);
    //    }
    //    else
    //    {
    //        Debug.Log("POST ERROR");
    //        Debug.Log(www.error);
    //    }
    //}

    //// POST 이미지

    //IEnumerator UnityWebRequestPOST_multi()
    //{
    //    List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

    //    //데이터와 파일 섹션?
    //    //이미지 주소가 와야 함? 로컬 주소가 오지 않고형식을 모르겠다.
    //    formData.Add(new MultipartFormDataSection("???"));

    //    formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

    //    UnityWebRequest www = UnityWebRequest.Post("주소", formData);

    //    yield return www.SendWebRequest();

    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(www.error);
    //    }else
    //    {
    //        Debug.Log("Form upload complete!");
    //    }
    //}
    #endregion

}
