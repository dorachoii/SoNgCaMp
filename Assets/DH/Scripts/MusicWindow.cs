using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class MusicWindow : MonoBehaviour
{
    public Transform content;
    public GameObject SongImgObject;
    void Start()
    {
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/findAllfiles")
            .Type(ReqType.GET)
            .Success((down) =>
            {
                Debug.Log(down.text);
                FileListDTO dto = JsonUtility.FromJson<FileListDTO>(down.text);
                //Ÿ��Ʋ ��Ƽ��Ʈ ����.
                dto.files.ForEach((info) =>
                {
                    //���⼭ �̹��� ����. ������ ���� ��
                    GameObject go = Instantiate(SongImgObject,content);
                    SongImg2 songInfo = go.GetComponent<SongImg2>();
                    songInfo.set(info.songArtist,info.songTitle,info.musicFileUrl);
                });

            })
            .Failure((downm)=> {
                GameObject go = Instantiate(SongImgObject, content);
                SongImg2 songInfo = go.GetComponent<SongImg2>();
                //songInfo.set(info.songArtist, info.songTitle, info.musicFileUrl);
            })
            .build();

        StartCoroutine(SendRequest(rq)); 
    }

    [SerializeField] GameObject wind;
    public void window() {
        wind.SetActive(!wind.activeSelf);
    }

    public void set() { 
    
    }
}
