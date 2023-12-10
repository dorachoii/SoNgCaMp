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
                //타이틀 아티스트 사진.
                dto.files.ForEach((info) =>
                {
                    HttpRequest rq = new HttpBuilder()
                    .Host(info.imageFileUrl)
                    .Success((down) => {
                        //여기서 이미지 결정. 지금은 안함 ㅋ
                        
                        GameObject go = Instantiate(SongImgObject, content);
                        SongImg2 songInfo = go.GetComponent<SongImg2>();
                        Sprite sp = LoadSpriteFromBytes(down.data);
                        string text = "Title : " + info.songTitle + "\n" + "Artist : " + info.songArtist;
                        songInfo.set(text,sp,info);
                    })
                    .build();
                    StartCoroutine(SendRequest(rq));




                });

            })
            .Failure((downm)=> {
                GameObject go = Instantiate(SongImgObject, content);
                SongImg2 songInfo = go.GetComponent<SongImg2>();
                songInfo.set("title:개" + "\n" + "Artist:나리",null, new FileDTO());
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
   
    public Sprite LoadSpriteFromBytes(byte[] imageBytes)
    {
        // Texture2D 생성
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes); // 바이트 데이터를 Texture2D로 로드

        // Sprite 생성
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }
}
