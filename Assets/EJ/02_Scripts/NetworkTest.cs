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
        
        httpInfo.Set(RequestType.POST, "api/v1/authentication/login", (DownloadHandler downHandler) => {
            print(downHandler.text);
            //���������� ��û�޾��� �� ���� ����Ʈ
        }, true);
        //httpInfo.body = JsonUtility.ToJson(userInfo);
        httpInfo.body = "{\r\n  \"userEmail\": \"hellodoradora7@gmail.com\",\r\n  \"userPwd\": \"1234abcd!\"\r\n}";
        HttpManager.Get().SendRequest(httpInfo);


        //�̹�����, ��

    }

    #region �����ڵ�
    ////01. GET: API�� ����Ǿ� �ִ� ������ �޾ƿ��� ��
    //IEnumerator UnityWebRequestGET()
    //{
        
    //    string apikey = "�߱޹��� APIŰ�� �ִ´�.";
    //    //string url = "http://api.neople.co.kr/df/servers?apikey=" + apikey;
    //    //string url = "GET ����� �� API �ּ�" + apikey;
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

    //// GET: �̹��� �ٿ�ε�
    //IEnumerator UnityWebRequestGET_texture()
    //{
    //    string url = "GET ����� �� API �ּ�";
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
    ////    //string url = "POST ����� ����� ���� �ּҸ� �Է�";
    ////    string url = "http://192.168.0.24:8080/api/v1/users";
    ////    WWWForm form = new WWWForm();


    ////    string id = "hellodoradora7@gmail.com";
    ////    string pw = "1234abcd!";

    ////    string nickName = "�ź���";

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
    //    //string url = "POST ����� ����� ���� �ּҸ� �Է�";
    //    //string url = "http://192.168.0.24:8080/api/v1/users";
    //    string url = "https://en29jxisoaoti.x.pipedream.net";
    //    WWWForm form = new WWWForm();


    //    string id = "hellodoradora7@gmail.com";
    //    string pw = "1234abcd!";
    //    string nickName = "�ź���";
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

    //// POST �̹���

    //IEnumerator UnityWebRequestPOST_multi()
    //{
    //    List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

    //    //�����Ϳ� ���� ����?
    //    //�̹��� �ּҰ� �;� ��? ���� �ּҰ� ���� �ʰ������� �𸣰ڴ�.
    //    formData.Add(new MultipartFormDataSection("???"));

    //    formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

    //    UnityWebRequest www = UnityWebRequest.Post("�ּ�", formData);

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
