using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class LoadSong : MonoBehaviour
{

    private void Start()
    {
        Load();
    }
    public void Load() {
        //로드를 한다
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/findAllfiles")
            .Type(ReqType.GET)
            .Success((down)=> {
                Debug.Log(down.text);
                FileListDTO dto = JsonUtility.FromJson<FileListDTO>(down.text);
                Setlist(dto);

                //Debug.Log(dtoList);
                
            })
            .Failure((down) => { })
            .build();



        StartCoroutine(SendRequest(rq));
        
    }

    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject songImgObj;
    public void Setlist(FileListDTO dto)
    {
        dto.files.ForEach((info)=> {
            //Img 생성하기
            GameObject go = Instantiate(songImgObj, content);
            SongImg img =  go.GetComponent<SongImg>();

            //이미지 요청
            HttpRequest rq = new HttpBuilder()
            .Host(info.imageFileUrl)
            .Type(ReqType.GET)
            .Success((down)=> {
                //Byte to sprite
                Sprite sprite = LoadSpriteFromBytes(down.data);
                //텍스트 세팅
                img.Set(info.songTitle, info.songArtist, info.needSession, "?",sprite);
            })
            .Failure((down)=> {
                Sprite sprite = LoadSpriteFromBytes(down.data);
                //이미지 요청 실패. 
                img.Set(info.songTitle, info.songArtist, info.needSession, "?", null);
            })
            .build();

            img.Set(info.songTitle, info.songArtist, info.needSession, "?", null);
            StartCoroutine(SendRequest(rq));
          
        });
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
