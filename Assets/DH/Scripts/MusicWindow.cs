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
                    HttpRequest rq = new HttpBuilder()
                    .Host(info.imageFileUrl)
                    .Success((down) => {
                        //���⼭ �̹��� ����. ������ ���� ��
                        
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
                songInfo.set("title:��" + "\n" + "Artist:����",null, new FileDTO());
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
        // Texture2D ����
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes); // ����Ʈ �����͸� Texture2D�� �ε�

        // Sprite ����
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        return sprite;
    }
}
