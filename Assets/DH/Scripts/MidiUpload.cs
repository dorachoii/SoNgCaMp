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
    //��ư�� ������ ���ε�

    public void UploadMidi() {
        //v

        SongDTO dto = new SongDTO();

        dto.needSession = "test";
        dto.songArtist = "�赵��";
        dto.songTitle = "������ ��";

        string js_dto = JsonUtility.ToJson(dto);

        byte[] png = Getfile("file1.png");
        byte[] mp3 = Getfile("file2.mp3");
        byte[] mid = Getfile("file3.mid");
        //�ϴ� ������ �����ߴٰ� ġ��



        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/fileUpload")
            .Success((down)=> { Success(); })
            .Failure((down)=> { Failed(); })
            .build();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("fileList", png,"file1.png", "multipart/form-data") );  //png //�̰� ���ù޾ƾ� �ϴ´� ����Ͽ���... �ϴ� ����
        formData.Add(new MultipartFormFileSection("fileList", mp3,"file2.mp3", "multipart/form-data"));   //mp3 //�̰� ���� X
        formData.Add(new MultipartFormFileSection("fileList", mid,"fiel3.mid", "multipart/form-data"));   //midi //�̰� ���Ͽ��� ����
        
        formData.Add(new MultipartFormDataSection("fileRequest",js_dto, "application/json"));   //

        StartCoroutine(SendRequest(rq,formData));
    }

    //���ε� ����
    public void Success() {
        Debug.Log("����");
    }
    //���ε� ����
    public void Failed() {
        Debug.Log("����");
    }


    public byte[] Getfile(string path) {
        string defaultpath = Application.persistentDataPath;
        return File.ReadAllBytes(defaultpath + "/files/" + path);

    }
}
