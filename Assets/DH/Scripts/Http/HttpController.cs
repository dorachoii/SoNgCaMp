using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpController : MonoBehaviour
{
    //IP, PORT Number
    public static string default_host = "http://192.168.0.45:8080";
    //Manager 


    //요청 타입
    //GET, POST, PUT, DELETE
    //enctype.
    public static IEnumerator Upload()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //string path = Application.persistentDataPath + "/" + "example.mid.txt";
        //byte[] bytes = File.ReadAllBytes(path);


        formData.Add(new MultipartFormFileSection("fileList", new byte[] { 0, 0, 0, 0 }, "example.png", "multipart/form-data"));
        formData.Add(new MultipartFormFileSection("fileList", new byte[] { 0, 0, 0, 0 }, "example.mp3", "multipart/form-data"));
        //formData.Add(new MultipartFormFileSection("fileList", bytes, "example.mid", "multipart/form-data"));


        List<SongUserDTO> testlist = new List<SongUserDTO>();
        testlist.Add(new SongUserDTO() { sessionType = 1, userNo = 1234 });
        testlist.Add(new SongUserDTO() { sessionType = 2, userNo = 1242 });

        string s = JsonUtility.ToJson(new SongDTO() { needSession = "4", participantList = new List<SongUserDTO>(), songArtist = "TestArtist", songTitle = "Test" });
        formData.Add(new MultipartFormDataSection("fileRequest", s, "application/json"));

        //WWWForm wwwForm = new WWWForm();
        //wwwForm.AddBinaryData("file", bytes, "example.mid");
        //wwwForm.AddBinaryData("fileList", new byte[] { 0, 0, 0, 0 });
        //wwwForm.AddBinaryData("fileList", new byte[] { 0, 0, 0, 0 });
        //wwwForm.AddBinaryData("fileRequest", Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Info2() { needSession = "4", particlpantList = new List<Info3>(), songArtist = "TestArtist", songTitle = "Test" })), "data.json", "application/json");
        //wwwForm.AddField("fileRequest", JsonUtility.ToJson(new Info2() { needSession = "4", particlpantList = new List<Info3>(), songArtist = "TestArtist", songTitle = "Test" }));

        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.33:8080/api/v1/fileUpload", formData))
        {

            yield return www.SendWebRequest();
            if (www.error != null)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("F");
            }
        }

    }


    //GET에 딕셔너리를 사용해버리면.. POST에 JSOn은?

    //타입, 주소, 데이터, 행동


    //단순 요청
    public static IEnumerator SendRequest(HttpRequest info) {
        UnityWebRequest www = null;
        switch (info.type) {
            case ReqType.GET:
                string data = string.IsNullOrEmpty(info.data) ? "" : "/" + info.data;
                www = UnityWebRequest.Get(info.host + info.uri + data); //1234, 
                www.timeout = 5;
                //Debug.Log(info.host + info.uri + );
                break;
            case ReqType.POST:
                Debug.Log(info);
                www = UnityWebRequest.Post(info.host + info.uri,info.data);
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(info.data);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.SetRequestHeader("Content-Type", "application/json");
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                break;
            case ReqType.PUT:
                //www = UnityWebRequest.Put(info.url);
                break;
            case ReqType.DELETE:
                www = UnityWebRequest.Delete(info.uri);
                break;
            case ReqType.Audio:
                www = UnityWebRequestMultimedia.GetAudioClip(info.host + info.uri, AudioType.MPEG);
                    break;

        }
        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log("요청 성공");
            info.OnSuccessCallback?.Invoke(www.downloadHandler);

        }
        if (www.error != null)
        {
            Debug.LogError("Request Error!!");
            Debug.LogError(www.error);
            info.OnFailureCallback?.Invoke(www.downloadHandler);
        }

    }

    //딕셔너리 사용
    public static IEnumerator SendRequest(HttpRequest info,Dictionary<string,string> data) {
        UnityWebRequest www = null;
        switch (info.type) {
            case ReqType.GET:
                string url = info.host + info.uri + GenerateQuaryString(data);
                Debug.Log(url);
                www = UnityWebRequest.Get(url);

                break;
            case ReqType.POST:
                Debug.Log(data["age"]);
                Debug.Log(data["name"]);
                www = UnityWebRequest.Post(info.host + info.uri,data);

                break;
        }

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log("요청 성공");
            info.OnSuccessCallback?.Invoke(www.downloadHandler);

        }
        if(www.error != null) {
            Debug.LogError("Request Error!!");
            Debug.LogError(www.error);
            info.OnFailureCallback?.Invoke(www.downloadHandler);
        }
    }

    
    //멀티파트 사용
    public static IEnumerator SendRequest(HttpRequest info, List<IMultipartFormSection> formData) {

        Debug.Log(info.host + info.uri);

        UnityWebRequest www = UnityWebRequest.Post(info.host + info.uri, formData);
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            Debug.Log("요청 성공");
            info.OnSuccessCallback?.Invoke(www.downloadHandler);

        }
        if (www.error != null)
        {
            Debug.LogError("Request Error!!");
            Debug.LogError(www.error);
            info.OnFailureCallback?.Invoke(www.downloadHandler);
        }
    }


    public static IEnumerator LoadAudioFromURL(HttpRequest info,Action<AudioClip> action) 
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(info.host + info.uri, AudioType.MPEG))
        {
            Debug.Log(info.host + info.uri);
            yield return www.SendWebRequest();

            if (www.error == null) {
                Debug.Log("요청 성공");
                info.OnSuccessCallback?.Invoke(www.downloadHandler);
                
                // AudioClip으로 변환
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                action(audioClip);
            }
            if(www.error != null)
            {
                Debug.Log("요청실패" + www.error);
                info.OnFailureCallback?.Invoke(www.downloadHandler);

            }
        }
    }

    /// <summary>
    /// One parameter Or JSON
    /// </summary>
    ///<returns>Start Request, Coroutine</returns>
    public Coroutine Send(HttpRequest info) {
        return StartCoroutine(SendRequest(info));
    }

    /// <summary>
    /// key , value parameter
    /// </summary>
    ///<returns>Start Request, Coroutine</returns>
    public Coroutine Send(HttpRequest info,Dictionary<string,string> data) {
        return StartCoroutine(SendRequest(info,data));
    }
    /// <summary>
    /// multipart - form request
    /// </summary>
    ///<returns>Start Request, Coroutine</returns>
    public Coroutine Send(HttpRequest info, List<IMultipartFormSection> formData) {
        return StartCoroutine(SendRequest(info,formData));
    }
    public enum ReqType { 
        GET,
        POST,
        PUT,
        DELETE,
        Texture,
        Audio
    }

    public class HttpRequest : System.Object
    {
        public ReqType type;
        public string host;
        public string uri;
        public WWWForm test;
        public string data;
        public Action<DownloadHandler> OnSuccessCallback;
        public Action<DownloadHandler> OnFailureCallback;

        public HttpRequest(ReqType type,string host, string url, string data, Action<DownloadHandler> successCallback,Action<DownloadHandler> failureCallback) {
            this.host = host;
            this.type = type;
            this.uri = url;
            this.data = data;
            this.OnSuccessCallback = successCallback;
            this.OnFailureCallback = failureCallback;
        }

        public override string ToString()
        {
            return "HTTP - REQUEST : " + " type : " + type + " url : " + uri + " data : " + data;
        }

    }

    public class HttpBuilder
    {
        private ReqType type;
        private string host;
        private string uri;
        private string url;
        private string data;
        private Action<DownloadHandler> OnSuccessCallback;
        private Action<DownloadHandler> OnFailureCallback;

        public HttpBuilder Type(ReqType type)
        {
            this.type = type;
            return this;
        }
        public HttpBuilder Host(string host)
        {
            this.host = host ;
            return this;
        }

        public HttpBuilder Uri(string uri)
        {
            this.uri = uri;
            return this;
        }
        public HttpBuilder Data(string data)
        {
            this.data = data;
            return this;
        }
        public HttpBuilder Success(Action<DownloadHandler> action)
        {
            this.OnSuccessCallback = action;
            return this;
        }

        public HttpBuilder Failure(Action<DownloadHandler> action)
        {
            this.OnFailureCallback = action;
            return this;
        }

        public HttpBuilder Url(string url)
        {
            this.url = url;
            return this;
        }
        public HttpRequest build()
        {
            //호스트 설정 안할시 기본 호스트로 설정.
            if (host == null)
                host = default_host;
            
            return new HttpRequest(type,host,uri, data, OnSuccessCallback,OnFailureCallback);
        }


    }

    public static string GenerateQuaryString(Dictionary<string, string> data) {
        StringBuilder stringBuilder = new StringBuilder();

        if (data.Count > 0)
            stringBuilder.Append("?");

        int count = 0;
        foreach (var v in data) {
            
            count++;
            stringBuilder.Append(v.Key).Append("=").Append(v.Value);
            if (count < data.Count)
                stringBuilder.Append("&");
        }
        return stringBuilder.ToString();
    }

 


    [System.Serializable]
    public class HashMap<KEY,VALUE> {
        public List<HashData<KEY,VALUE>> data = new List<HashData<KEY,VALUE>>();


        public void Add(KEY key,VALUE value) {
            data.Add(new HashData<KEY, VALUE>() { key = key, value = value }) ;
        }
    }
    [System.Serializable]
    public class HashData<KEY, VALUE> {
        public KEY key;
        public VALUE value;
    }

    [System.Serializable]
    public class TestHash<T> {
        public string data;
        public string ss;
        public T sss;
        public List<T> list = new List<T>();
        public TestHash(string data,string ss,T sss) {
            this.data = data;
            this.ss = ss;
            this.sss = sss;
        }
    }



}
