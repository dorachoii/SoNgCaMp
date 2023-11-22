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
        //�ε带 �Ѵ�
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
            //Img �����ϱ�
            GameObject go = Instantiate(songImgObj, content);
            SongImg img =  go.GetComponent<SongImg>();

            //�̹��� ��û
            HttpRequest rq = new HttpBuilder()
            .Host(info.imageFileUrl)
            .Type(ReqType.GET)
            .Success((down)=> {
                //Byte to sprite
                Sprite sprite = LoadSpriteFromBytes(down.data);
                //�ؽ�Ʈ ����
                img.Set(info.songTitle, info.songArtist, info.needSession, "?",sprite);
            })
            .Failure((down)=> {
                Sprite sprite = LoadSpriteFromBytes(down.data);
                //�̹��� ��û ����. 
                img.Set(info.songTitle, info.songArtist, info.needSession, "?", null);
            })
            .build();

            img.Set(info.songTitle, info.songArtist, info.needSession, "?", null);
            StartCoroutine(SendRequest(rq));
          
        });
    }


    public Sprite LoadSpriteFromBytes(byte[] imageBytes)
    {
        // Texture2D ����
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes); // ����Ʈ �����͸� Texture2D�� �ε�

        // Sprite ����
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }

}
