using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static HttpController;
using UnityEngine.Networking;
using System.IO;

public class MidiUpload : MonoBehaviour
{
    [SerializeField]
    TMP_InputField titleField;

    private void Start()
    {
        UploadMidi();
    }
    //버튼을 누르면 업로드

    public void UploadMidi() {
        //v

        SongDTO dto = new SongDTO();

        dto.needSession = "test";
        dto.songArtist = "김도현";
        dto.songTitle = "마리오 곡";

        string js_dto = JsonUtility.ToJson(dto);

        byte[] png = Getfile("file1.png");
        byte[] mp3 = Getfile("file2.mp3");
        byte[] mid = Getfile("file3.mid");
        //일단 파일을 선택했다고 치고



        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/fileUpload")
            .Success((down)=> { Success(); })
            .Failure((down)=> { Failed(); })
            .build();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("fileList", png,"file1.png", "multipart/form-data") );  //png //이건 선택받아야 하는대 모바일에서... 일단 나중
        formData.Add(new MultipartFormFileSection("fileList", mp3,"file2.mp3", "multipart/form-data"));   //mp3 //이건 지원 X
        formData.Add(new MultipartFormFileSection("fileList", mid,"fiel3.mid", "multipart/form-data"));   //midi //이건 파일에서 뽑자
        
        formData.Add(new MultipartFormDataSection("fileRequest",js_dto, "application/json"));   //

        StartCoroutine(SendRequest(rq,formData));
    }

    //업로드 성공
    public void Success() {
        Debug.Log("성공");
    }
    //업로드 실패
    public void Failed() {
        Debug.Log("실패");
    }


    public byte[] Getfile(string path) {
        string defaultpath = Application.persistentDataPath;
        return File.ReadAllBytes(defaultpath + "/files/" + path);

    }
}
