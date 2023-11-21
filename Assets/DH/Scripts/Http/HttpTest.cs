using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpTest : MonoBehaviour
{

    //Manager 


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Upload());


        //HttpRequest rq = new HttpBuilder().Type(Re).Url().Data().Action().build;


        string data = JsonUtility.ToJson(new Info() { age = 10, name = "name" });
        //Debug.LogError(data);
        //StartCoroutine(SendRequest(new HttpRequest(ReqType.POST, "192.168.0.86/test1234/test", data, (i) => { Debug.Log(i.text); })));

        //HttpRequest rq = new HttpBuilder().Type(ReqType.POST)
        //    .Url("192.168.0.86/test1234/test")
        //    .Data(data)
        //    .Action((i) => { Debug.Log(i.text); })
        //    .build();

        //Send(rq);


        //rq = new HttpBuilder().Url("192.168.0.86/test1234/test").Type(ReqType.GET).Data("Hello!").build();
        //WWWForm test = new WWWForm();
        //test.AddField("data", "1234");
        //rq.test = test;

        //SendRequest(rq,new List<IMultipartFormSection>());


        //StartCoroutine(Upload());

        HttpRequest rq = new HttpBuilder().Url("192.168.56.1/test1234/test").Type(ReqType.POST).build();
        Dictionary<string, string> datas = new Dictionary<string, string>();
        datas.Add("age", "10");
        datas.Add("name", "안녕하시오");
        datas.Add("title", "몰라");
        StartCoroutine(SendRequest(rq, datas));



        rq = new HttpBuilder().Url("192.168.56.1/test1234/download")
            .Type(ReqType.GET)
            .Action((i) => {
                File.WriteAllBytes("C:\\test\\test.txt",i.data); 
                
            })
            .build();

        rq = new HttpBuilder().Url("192.168.56.1/test1234/midi")
            .Type(ReqType.GET)
            .Action((i) => {
                Info2 info = JsonUtility.FromJson<Info2>(i.text);
                Debug.Log(info.songArtist);
                Debug.Log(info.songTitle);
                Debug.Log(info.needSession);
            })
            .build();

        StartCoroutine(SendRequest(rq));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Send(HttpRequest info)
    {
        Debug.Log("요청징행중" + info);
        StartCoroutine(SendRequest(info));

    }
    //요청 타입
    //GET, POST, PUT, DELETE
    //enctype.
    IEnumerator Upload() {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        string path = Application.persistentDataPath + "/" + "example.mid.txt";
        byte[] bytes = File.ReadAllBytes(path);


        formData.Add(new MultipartFormFileSection("fileList", new byte[] { 0, 0, 0, 0 }, "example.png", "multipart/form-data"));
        formData.Add(new MultipartFormFileSection("fileList", new byte[] {0,0,0,0 }, "example.mp3", "multipart/form-data"));
        formData.Add(new MultipartFormFileSection("fileList", bytes, "example.mid", "multipart/form-data")  );


        List< Info3 > testlist =  new List<Info3>();
        testlist.Add(new Info3() { sessionType = 1, userNo = 1234 });
        testlist.Add(new Info3() { sessionType = 2, userNo = 1242 });

        string s = JsonUtility.ToJson(new Info2() { needSession = "4", participantList = new List<Info3>(), songArtist = "TestArtist", songTitle = "Test" });
        formData.Add(new MultipartFormDataSection("fileRequest", s , "application/json"));

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddBinaryData("file", bytes, "example.mid");
        //wwwForm.AddBinaryData("fileList", new byte[] { 0, 0, 0, 0 });
        //wwwForm.AddBinaryData("fileList", new byte[] { 0, 0, 0, 0 });
        //wwwForm.AddBinaryData("fileRequest", Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Info2() { needSession = "4", particlpantList = new List<Info3>(), songArtist = "TestArtist", songTitle = "Test" })), "data.json", "application/json");
        //wwwForm.AddField("fileRequest", JsonUtility.ToJson(new Info2() { needSession = "4", particlpantList = new List<Info3>(), songArtist = "TestArtist", songTitle = "Test" }));

        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.4:8080/api/v1/fileUpload", formData))
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
    public IEnumerator SendRequest(HttpRequest info) {
        UnityWebRequest www = null;
        switch (info.type) {
            case ReqType.GET:
                www = UnityWebRequest.Get(info.url); //1234, 

                break;
            case ReqType.POST:
                Debug.Log(info);
                www = UnityWebRequest.Post(info.url,info.data);
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(info.data);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.SetRequestHeader("Content-Type", "application/json");
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                break;
            case ReqType.PUT:
                //www = UnityWebRequest.Put(info.url);
                break;
            case ReqType.DELETE:
                www = UnityWebRequest.Delete(info.url);
                break;
        }
        yield return www.SendWebRequest();

        if (www.error == null) {
            Debug.Log("요청이 성공적으로 진행되었습니다.");

            if(info.action != null)
                info.action(www.downloadHandler);
        }
        else {
            Debug.LogWarning("요청 오류!");
            Debug.LogWarning(www.error);
        }

    }

    //딕셔너리 사용
    public IEnumerator SendRequest(HttpRequest info,Dictionary<string,string> data) {
        UnityWebRequest www = null;
        switch (info.type) {
            case ReqType.GET:
                string url = info.url + GenerateQuaryString(data);
                Debug.Log(url);
                www = UnityWebRequest.Get(url);

                break;
            case ReqType.POST:
                Debug.Log(data["age"]);
                Debug.Log(data["name"]);
                www = UnityWebRequest.Post(info.url,data);

                break;
        }

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log("요청 성공");
            if (info.action != null) { 
                info.action(www.downloadHandler);
            
            }

        }
        if(www.error != null) {
            Debug.LogError("Request Error!!");
            Debug.LogError(www.error);
        }
    }
    //멀티파트 사용
    public IEnumerator SendRequest(HttpRequest info, List<IMultipartFormSection> formData) {
        UnityWebRequest www = UnityWebRequest.Post(info.url, formData);
        yield return www.SendWebRequest();
        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (info.action != null) {
                info.action(www.downloadHandler);
            }
        }
    }

    public enum ReqType { 
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpRequest : System.Object
    {
        public ReqType type;
        public string url;
        public WWWForm test;
        public string data;
        public Action<DownloadHandler> action;

        public HttpRequest(ReqType type, string url, string data, Action<DownloadHandler> action) {
            this.type = type;
            this.url = url;
            this.data = data;
            this.action = action;
        }

        public override string ToString()
        {
            return "HTTP - REQUEST : " + " type : " + type + " url : " + url + " data : " + data;
        }

       
    }

    public class HttpBuilder
    {
        private ReqType type;
        private string url;
        private string data;
        private Action<DownloadHandler> action;

        public HttpBuilder Type(ReqType type)
        {
            this.type = type;
            return this;
        }

        public HttpBuilder Url(string url)
        {
            this.url = url;
            return this;
        }
        public HttpBuilder Data(string data)
        {
            this.data = data;
            return this;
        }
        public HttpBuilder Action(Action<DownloadHandler> action)
        {
            this.action = action;
            return this;
        }

        public HttpRequest build()
        {
            return new HttpRequest(type, url, data, action);
        }
    }

    public string GenerateQuaryString(Dictionary<string, string> data) {
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
    struct Info {
        public int age;
        public string name;
    }


    [System.Serializable]
    struct Info2
    {
        
        public string needSession;
        public List<Info3> participantList;
        public string songArtist;
        public string songTitle;
    }
    [System.Serializable]
    struct Info3 {
        public int sessionType;
        public int userNo;
    }
}
