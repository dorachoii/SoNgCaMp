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
            .Success((down) => {
                Debug.Log(down.text);
                FileListDTO dto = JsonUtility.FromJson<FileListDTO>(down.text);
                Setlist(dto);

                //Debug.Log(dtoList);

            })
            .Failure((down) => {
                //요청 실패시...
                //기본 생성 
                FileListDTO dto = new FileListDTO();
                dto.files = new List<FileDTO>();
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT 생상
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT",needSession = "DEFAULT" ,songArtist = "DEFAULT" }); //DEFAULT 생상
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT 생상
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT 생상
                dto.files.Add(new FileDTO() { songTitle = "DEFAULT", needSession = "DEFAULT", songArtist = "DEFAULT" }); //DEFAULT 생상
                Setlist(dto);
            })
            .build();



        StartCoroutine(SendRequest(rq));
        
    }

    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject songImgObj;

    public int selectScene;
    public void Setlist(FileListDTO dto)
    {
        int count = 0;
        dto.files.ForEach((info)=> {
            //Img 생성하기
            GameObject go = Instantiate(songImgObj, content);
            go.SetActive(false);
            SongImg img =  go.GetComponent<SongImg>();
            img.index = count;
            //info의 img url이 null이면
            if (info.imageFileUrl == null) {
                
                SetFaildList(info, img,count);
            }

            //이미지 요청
            HttpRequest rq = new HttpBuilder()
            .Host(info.imageFileUrl)
            .Type(ReqType.GET)
            .Success((down)=> {
                //Byte to sprite
                Sprite sprite = LoadSpriteFromBytes(down.data);
                //텍스트 세팅
                img.Set(info.songTitle, info.songArtist, info.needSession, "?",sprite,info, selectScene);
                go.SetActive(true);
            })
            .Failure((down)=> {
                //Sprite sprite = LoadSpriteFromBytes(down.data);
                //이미지 요청 실패. 
                //img.Set(info.songTitle, info.songArtist, info.needSession, "?", null, info, selectScene);
                //go.SetActive(true);
            })
            .build();

            
            StartCoroutine(SendRequest(rq));
            count++;
        });
    }

    //faild list
    [SerializeField]
    Sprite[] faildImgList;
    public void SetFaildList(FileDTO info, SongImg img, int count) {
        img.isDefault = true;
        //요청 안하고 바로
            img.Set(info.songTitle, info.songArtist, info.needSession, "?", faildImgList[count], info, selectScene);
            img.gameObject.SetActive(true);
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
