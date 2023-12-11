using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static HttpController;

public class UploadTest : MonoBehaviour
{
    public SongDTO dto = new SongDTO();
    public void click() {
        
        string js_dto = JsonUtility.ToJson(dto);


        HttpRequest rq = new HttpBuilder()
            .Host("http://192.168.26.109:8080")
            .Uri("/api/v1/fileUpload")
            .Success((down) => { Debug.Log("성공!!");  })
            .Failure((down) => {  })
            .build();

        string path = Application.persistentDataPath + "/";
        //png,mp3,mid
        byte[] pngData = File.ReadAllBytes(path + "file1.png");
        byte[] mp3Data = File.ReadAllBytes(path + "file2.mp3");
        byte[] midData = File.ReadAllBytes(path + "file3.mid");


        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("fileList", pngData, "file1.png", "multipart/form-data"));  //png //이건 선택받아야 하는대 모바일에서... 일단 나중
        formData.Add(new MultipartFormFileSection("fileList", mp3Data, "file2.mp3", "multipart/form-data"));   //mp3 //이건 지원 X
        formData.Add(new MultipartFormFileSection("fileList", midData, "fiel3.mid", "multipart/form-data"));   //midi //이건 파일에서 뽑자

        formData.Add(new MultipartFormDataSection("fileRequest", js_dto, "application/json")); 

        StartCoroutine(SendRequest(rq, formData));
    }


}
