using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UnityWebRequestGET());
        StartCoroutine(UnityWebRequestGET_texture());

        StartCoroutine(UnityWebRequestPOST());
    }

    //01. GET: API에 저장되어 있는 정보를 받아오는 것
    IEnumerator UnityWebRequestGET()
    {
        
        string apikey = "발급받은 API키를 넣는다.";
        //string url = "http://api.neople.co.kr/df/servers?apikey=" + apikey;
        string url = "GET 통신을 할 API 주소" + apikey;

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text);
        }else
        {
            Debug.Log("GET ERROR");
        }
    }

    // GET: 이미지 다운로드
    IEnumerator UnityWebRequestGET_texture()
    {
        string url = "GET 통신을 할 API 주소";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.error == null) 
        {
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        else
        {
            Debug.Log("GET_texture ERROR");
        }
    }

    //02. POST: API에 정보를 저장하는 것

    IEnumerator UnityWebRequestPOST()
    {
        string url = "POST 통신을 사용할 서버 주소를 입력";
        WWWForm form = new WWWForm();

        string id = "아이디";
        string pw = "비밀번호";

        string nickName = "거북이";
        int sessionType = 0;
        int genre = 0;
        int mood = 0;
        
        form.AddField("Username", id);
        form.AddField ("Password", pw);

        form.AddField("nickName", nickName);
        form.AddField("mainPosition", sessionType);
        form.AddField("genre", genre);
        form.AddField("mood", mood);
                
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text);
        }else
        {
            Debug.Log("POST ERROR");
        }
    }

    // POST 이미지

    IEnumerator UnityWebRequestPOST_multi()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //데이터와 파일 섹션?
        //이미지 주소가 와야 함? 로컬 주소가 오지 않고형식을 모르겠다.
        formData.Add(new MultipartFormDataSection("???"));

        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post("주소", formData);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }else
        {
            Debug.Log("Form upload complete!");
        }
    }

}
