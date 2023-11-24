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

    }
    //��ư�� ������ ���ε�

    public void UploadMidi() {
        //v

        SongDTO dto = new SongDTO();

        dto.needSession = "Guitar";
        dto.songArtist = "Doss";
        dto.songTitle = "Winter With Me";

        string js_dto = JsonUtility.ToJson(dto);

        //mp3�� �����ϱ� ����
        mp3Data = Getfile("file2.mp3");
        midData = Getfile("compose.mid");
        //�ϴ� ������ �����ߴٰ� ġ��



        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/fileUpload")
            .Success((down)=> { Success(); })
            .Failure((down)=> { Failed(); })
            .build();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("fileList", pngData, "file1.png", "multipart/form-data") );  //png //�̰� ���ù޾ƾ� �ϴ´� ����Ͽ���... �ϴ� ����
        formData.Add(new MultipartFormFileSection("fileList", mp3Data, "file2.mp3", "multipart/form-data"));   //mp3 //�̰� ���� X
        formData.Add(new MultipartFormFileSection("fileList", midData, "fiel3.mid", "multipart/form-data"));   //midi //�̰� ���Ͽ��� ����
        
        formData.Add(new MultipartFormDataSection("fileRequest",js_dto, "application/json"));   //

        StartCoroutine(SendRequest(rq,formData));
    }

    //���ε� ����
    public void Success() {
        SceneController.StartLoadSceneAsync(this,true,4,null);
    }
    //���ε� ����
    public void Failed() {
        Debug.Log("����");
    }


    public byte[] Getfile(string path) {
        string defaultpath = Application.persistentDataPath;
        return File.ReadAllBytes(defaultpath + "/files/" + path);

    }


    public byte[] pngData;
    byte[] mp3Data;
    public byte[] midData;
    public void OnclickImageLoad()
    {
        NativeGallery.GetImageFromGallery((file) =>
        {
            FileInfo selected = new FileInfo(file);

            StartCoroutine(LoadImage(file));
        });
    }
    public RawImage image;
    IEnumerator LoadImage(string path)
    {
        yield return null;
        byte[] fileData = File.ReadAllBytes(path);
        pngData = fileData;
        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(fileData);
        image.texture = tex;

    }
}
